using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.Actually;
using GPLX.Core.Contracts.CostEstimate;
using GPLX.Core.Contracts.Statuses;
using GPLX.Core.Contracts.Unit;
using GPLX.Core.DTO.Request.CostEstimate;
using GPLX.Core.DTO.Response;
using GPLX.Core.DTO.Response.CostEstimate;
using GPLX.Core.DTO.Response.CostEstimateItem;
using GPLX.Core.DTO.Response.Notify;
using GPLX.Core.Enum;
using GPLX.Core.Extensions;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace GPLX.Core.Data.CostEstimate
{
    public class CostEstimateRepository : ICostEstimateRepository
    {
        private readonly ILogger<CostEstimateRepository> _logger;
        private readonly Context _ctx;
        private readonly IActionLogsRepository _actionLogsRepository;
        private readonly IActuallySpentRepository _actuallySpentRepository;
        private readonly ICostStatusesRepository _costStatusesRepository;
        private readonly IUnitRepository _unitRepository;

        private readonly IMapper _mapper;

        public CostEstimateRepository(ILogger<CostEstimateRepository> logger, Context ctx, IMapper mapper, IActionLogsRepository logsRepository, IActuallySpentRepository actuallySpentRepository, ICostStatusesRepository costStatusesRepository, IUnitRepository unitRepository)
        {
            _logger = logger;
            _ctx = ctx;
            _mapper = mapper;
            _actionLogsRepository = logsRepository;
            _actuallySpentRepository = actuallySpentRepository;
            _costStatusesRepository = costStatusesRepository;
            _unitRepository = unitRepository;
        }


        public async Task<Database.Models.CostEstimate> GetByIdAsync(Guid id)
        {
            try
            {
                return await _ctx.CostEstimates.FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }

        public async Task<IList<CostEstimateLogs>> GetCostEstimateLogs(Guid id)
        {
            try
            {
                var q = await _ctx.CostEstimateLogs.Where(c => c.CostEstimateId == id).ToListAsync();
                return q;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }


        public async Task<SearchCostEstimateResponse> SearchAsync(SearchCostEstimateRequest request, int skip, int length, string unitType)
        {
            try
            {
                var response = new SearchCostEstimateResponse();
                var next = new NextStatExtension(await _costStatusesRepository.GetAll(), request.StatusAllowsSeen, GlobalEnums.CostStatusesElement, GlobalEnums.Week, "Dự trù");


                var query = _ctx.CostEstimates.
                    Join(_ctx.Units, c => c.UnitId, y => y.Id, (x, y) => new
                    {
                        CostEstimates = x,
                        y.OfficesSub,
                        y.OfficesCode
                    })
                    .Where(x => x.CostEstimates.Status != (int)GlobalEnums.StatusDefaultEnum.Temporary).AsQueryable();

                query = query.Where(x => x.CostEstimates.CostEstimateType == request.CostEstimateTypeId);

                var subStatusFilter = request.StatusAllowsSeen.Filter(next.All, true, request.Status);
                var unitStatusFilter = request.StatusAllowsSeen.Filter(next.All, false, request.Status);


                if (request.UnitsManages != null && request.UnitsManages.Count > 0)
                {
                    var unitIds = request.UnitsManages.Select(x => x.OfficeId);
                    query = query.Where(x => unitIds.Contains(x.CostEstimates.UnitId));
                }

                if (request.ReportForWeek > 0)
                    query = query.Where(x => x.CostEstimates.ReportForWeek == request.ReportForWeek);
                if (request.UserUnit != (int)GlobalEnums.StatusDefaultEnum.All)
                    query = query.Where(x => x.CostEstimates.UnitId == request.UserUnit);

                var data = (await query.OrderByDescending(c => c.CostEstimates.CreatedDate).ToListAsync()).Select(c => new
                {
                    c.OfficesCode,
                    c.CostEstimates,
                    OfficesSub = c.OfficesSub.Equals("YT", StringComparison.CurrentCultureIgnoreCase) ? GlobalEnums.UnitIn : GlobalEnums.UnitOut
                }).ToList();


                var latest = Extensions.Extensions.CreateList(data.FirstOrDefault());
                latest.Clear();
                // filter theo trạng thái được cấu hình
                foreach (var g in data)
                {
                    if (g.CostEstimates.IsSub && subStatusFilter.Count > 0)
                    {
                        var subFil = subStatusFilter.FirstOrDefault(c => c.UnitType.Equals(GlobalEnums.ObjectSub, StringComparison.CurrentCultureIgnoreCase));
                        if (subFil?.Values.Any(c => c == g.CostEstimates.Status) == true)
                            latest.Add(g);
                    }
                    else
                    {
                        if (unitStatusFilter.Count > 0)
                        {
                            foreach (var activatorResult in unitStatusFilter)
                            {
                                if (g.OfficesSub?.Equals(activatorResult.UnitType) == true &&
                                    activatorResult.Values.Contains(g.CostEstimates.Status))
                                {
                                    latest.Add(g);
                                    break;
                                }
                            }
                        }
                    }
                }

                var dataResponse = new List<SearchCostEstimateResponseData>();
                foreach (var costEstimate in latest.Skip(skip).Take(length))
                {


                    var item = _mapper.Map<SearchCostEstimateResponseData>(costEstimate.CostEstimates);
                    item.Status = costEstimate.CostEstimates.StatusName;
                    item.Record = costEstimate.CostEstimates.Id.ToString().StringAesEncryption(request.PageRequest);


                    int minLevel = next._minLevel(costEstimate.CostEstimates.IsSub, costEstimate.OfficesSub, string.Empty);

                    item.Type = costEstimate.CostEstimates.CostEstimateType;


                    item.Viewable = true;
                    item.Editable = next._visible(costEstimate.CostEstimates.Status, NextAction.EDIT, request.PermissionEdit, costEstimate.CostEstimates.IsSub, costEstimate.OfficesSub, string.Empty);
                    item.ApproveAble = next._visible(costEstimate.CostEstimates.Status, NextAction.APPROVED, request.PermissionApprove, costEstimate.CostEstimates.IsSub, costEstimate.OfficesSub, string.Empty);
                    item.DeclineAble = next._visible(costEstimate.CostEstimates.Status, NextAction.DECLINE, request.PermissionApprove, costEstimate.CostEstimates.IsSub, costEstimate.OfficesSub, string.Empty);

                    item.PathPdf = $"{request.HostFileView}{item.PathPdf}";
                    item.UnitName = costEstimate.CostEstimates.UnitName;


                    if (request.Status != (int)GlobalEnums.StatusDefaultEnum.All)
                    {
                        if (request.Status == (int)GlobalEnums.StatusDefaultEnum.Activator || request.Status == (int)GlobalEnums.StatusDefaultEnum.Active
                                                                                           || request.Status == (int)GlobalEnums.StatusDefaultEnum.Decline)
                            if (!item.ApproveAble && !item.DeclineAble)
                                dataResponse.Add(item);

                        if (request.Status == (int)GlobalEnums.StatusDefaultEnum.InActive)
                            if (item.ApproveAble || item.DeclineAble || minLevel == costEstimate.CostEstimates.Status)
                                dataResponse.Add(item);
                    }
                    else
                        dataResponse.Add(item);
                }

                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Draw = request.Draw;
                response.RecordsFiltered = dataResponse.Count;
                response.RecordsTotal = dataResponse.Count;
                response.Data = dataResponse;
                return response;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);

                return new SearchCostEstimateResponse
                {
                    Message = "Không tìm thấy dữ liệu yêu cầu!",
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Draw = request.Draw
                };
            }
        }

        public async Task<SearchCostEstimateResponse> SearchManage(SearchManageRequest request, int skip, int length)
        {
            try
            {
                var response = new SearchCostEstimateResponse();
                var allStats = await _costStatusesRepository.GetAll();
                var cp = Extensions.Extensions.CreateList(allStats.ToArray());
                var query = _ctx.CostEstimates.
                    Join(_ctx.Units, c => c.UnitId, y => y.Id, (x, y) => new
                    {
                        CostEstimates = x,
                        y.OfficesSub,
                        y.OfficesCode
                    })
                    .Where(x => x.CostEstimates.Status != (int)GlobalEnums.StatusDefaultEnum.Temporary).AsQueryable();

                query = query.Where(x => x.CostEstimates.CostEstimateType == (int)GlobalEnums.StatusDefaultType.Weekly);
                var subStatusFilter = Filter(allStats, cp, true, request.Status);
                var unitStatusFilter = Filter(allStats, cp, false, request.Status);

                if (request.Year != (int)GlobalEnums.StatusDefaultEnum.All)
                    query = query.Where(x => x.CostEstimates.CreatedDate.Year == request.Year);

                if (request.UnitId != null && request.UnitId.Count > 0 && request.UnitId.All(c => c != (int)GlobalEnums.StatusDefaultEnum.All))
                    query = query.Where(x => request.UnitId.Contains(x.CostEstimates.UnitId));

                if (request.Weeks.Count > 0 && request.Weeks.All(c => c != (int)GlobalEnums.StatusDefaultEnum.All))
                    query = query.Where(x => request.Weeks.Contains(x.CostEstimates.ReportForWeek));


                var data = (await query.OrderByDescending(c => c.CostEstimates.CreatedDate).ToListAsync()).Select(c => new
                {
                    c.OfficesCode,
                    c.CostEstimates,
                    OfficesSub = c.OfficesSub.Equals("YT", StringComparison.CurrentCultureIgnoreCase) ? GlobalEnums.UnitIn : GlobalEnums.UnitOut
                }).ToList();


                var latest = Extensions.Extensions.CreateList(data.FirstOrDefault());
                latest.Clear();
                // filter theo trạng thái được cấu hình
                foreach (var g in data)
                {
                    if (g.CostEstimates.IsSub && subStatusFilter.Count > 0)
                    {
                        var subFil = subStatusFilter.FirstOrDefault(c => c.UnitType.Equals(GlobalEnums.ObjectSub, StringComparison.CurrentCultureIgnoreCase));
                        if (subFil?.Values.Any(c => c == g.CostEstimates.Status) == true)
                            latest.Add(g);
                    }
                    else
                    {
                        if (unitStatusFilter.Count > 0)
                        {
                            foreach (var activatorResult in unitStatusFilter)
                            {
                                if (g.OfficesSub?.Equals(activatorResult.UnitType) == true &&
                                    activatorResult.Values.Contains(g.CostEstimates.Status))
                                {
                                    latest.Add(g);
                                    break;
                                }
                            }
                        }
                    }
                }

                var dataResponse = new List<SearchCostEstimateResponseData>();
                foreach (var costEstimate in latest.Skip(skip).Take(length))
                {

                    var item = _mapper.Map<SearchCostEstimateResponseData>(costEstimate.CostEstimates);
                    item.Status = costEstimate.CostEstimates.StatusName;
                    item.Record = costEstimate.CostEstimates.Id.ToString().StringAesEncryption("cost-element");

                    item.Type = costEstimate.CostEstimates.CostEstimateType;
                    item.Viewable = true;
                    item.PathPdf = $"{request.HostFileView}{item.PathPdf}";
                    item.UnitName = costEstimate.CostEstimates.UnitName;

                    
                    dataResponse.Add(item);
                }

                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Draw = request.Draw;
                response.RecordsFiltered = dataResponse.Count;
                response.RecordsTotal = dataResponse.Count;
                response.Data = dataResponse;
                return response;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);

                return new SearchCostEstimateResponse
                {
                    Message = "Không tìm thấy dữ liệu yêu cầu!",
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Draw = request.Draw
                };
            }
        }

        public static List<ActivatorResult> Filter(IEnumerable<CostStatuses> value, IList<CostStatuses> allStatus, bool sub, int? statusFilter)
        {
            var valueList = value?.Where(cc => cc.Type.Equals(GlobalEnums.CostStatusesElement, StringComparison.CurrentCultureIgnoreCase)).ToList();
            var rt = new List<ActivatorResult>();
            // nhóm theo từng loại

            if (valueList == null || !valueList.Any())
                return null;
            var dataGroups = valueList.GroupBy(c => c.StatusForSubject, (x, y) => new
            {
                For = x,
                Data = y.ToList()
            }).ToList();

            foreach (var dataGroup in dataGroups)
            {
                var dataFor = dataGroup.Data;
                if (statusFilter != null && statusFilter.Value != (int)GlobalEnums.StatusDefaultEnum.All)
                {
                    // trường hợp request vào từ trang phê duyệt
                    // bao gồm trạng thái (phê duyệt - từ chối)

                    dataFor = dataFor.Where(p =>
                            statusFilter.Value == (int)GlobalEnums.StatusDefaultEnum.Activator ||
                            p.IsApprove == (statusFilter == (int)GlobalEnums.StatusDefaultEnum.Active ? 1 : 0))
                        .ToList();

                    var minOrder = allStatus.Where(c => c.StatusForSubject == dataGroup.For).Min(c => c.Order);

                    // loại bỏ tất cả các trạng thái
                    // khác ở vị trí min
                    // chờ duyệt
                    if (statusFilter.Value == (int)GlobalEnums.StatusDefaultEnum.Activator)
                        dataFor = dataFor.Where(c => c.Order > minOrder).ToList();
                    if (statusFilter == (int)GlobalEnums.StatusDefaultEnum.InActive)
                        dataFor = dataFor.Where(x => x.Order == minOrder).ToList();
                    else if (statusFilter == (int)GlobalEnums.StatusDefaultEnum.Decline)
                        dataFor = dataFor.Where(x => x.Order != minOrder).ToList();
                }
                var rtQuery = dataFor.AsQueryable();
                // nếu là SUB --> chỉ lấy của sub
                // nếu khác SUB --> lấy của cả đơn vị yte - ngoài y tế
                rtQuery = sub
                    ? rtQuery.Where(x =>
                        x.StatusForSubject.Equals(GlobalEnums.ObjectSub,
                            StringComparison.CurrentCultureIgnoreCase))
                    : rtQuery.Where(x =>
                        !x.StatusForSubject.Equals(GlobalEnums.ObjectSub,
                            StringComparison.CurrentCultureIgnoreCase));


                var rtList = rtQuery.Select(x => x.Value).ToList();
                if (!rtList.Any())
                    rtList = new List<int> { -1000 };

                rt.Add(new ActivatorResult { UnitType = dataGroup.For, Values = rtList.Distinct().ToList() });
            }

            return rt;
        }

        public async Task<IList<NotifyData>> CheckUnitNotCreateYet()
        {
            try
            {
                var rt = new List<NotifyData>();
                var listUnitNotCreateYet = new List<Units>();
                var wiy = DateTime.Now.Year.DbWeekInYear();
                var cWeek = wiy.Where(c => c.weekStart >= DateTime.Now).Min(c => c.weekNum);

                var yearPair = DateTime.Now.Year;
                var weekPair = cWeek + 1;
                // tuần cuối của năm
                if (weekPair > wiy.Max(c => c.weekNum))
                {
                    yearPair += 1;
                    weekPair = 1;
                }

                var allUnits = await _unitRepository.GetAllAsync("", 0, 1000);
                foreach (var un in allUnits)
                {
                    var fCostUnit = await _ctx.CostEstimates.FirstOrDefaultAsync(c =>
                        c.UnitId == un.Id && c.ReportForWeek == weekPair && c.CreatedDate.Year == yearPair &&
                        c.Status != (int)GlobalEnums.StatusDefaultEnum.Temporary);
                    if (fCostUnit == null)
                        listUnitNotCreateYet.Add(un);
                }

                if (listUnitNotCreateYet.Count > 0)
                {
                    var listGroups = new List<string> { "chiefaccountant", "chiefaccountantmg" };
                    var listUnitIds = listUnitNotCreateYet.Select(c => c.Id).ToList();
                    var qJoinUser = from u in _ctx.Users
                                    join ug in _ctx.UserGroups on u.Id equals ug.UserId
                                    join g in _ctx.Groups on ug.GroupId equals g.Id
                                    where listUnitIds.Contains(u.UnitId ?? 0) && listGroups.Contains(g.GroupCode.ToLower())
                                    select new
                                    {
                                        Users = u
                                    };
                    var qJoinUserData = qJoinUser.Distinct().ToList();

                    var usersPerUnit = qJoinUserData.GroupBy(c => c.Users.UnitId ?? 0, (x, y) => new
                    {
                        Unit = x,
                        Users = y.ToList()
                    }).ToList();

                    usersPerUnit.ForEach(c =>
                    {
                        var notify = new NotifyData
                        {
                            Message = $"Đơn vị chưa lập dự trù tuần {weekPair}",
                            TimeCheck = DateTime.Now,
                            Receiver = c.Users.Select(cc => cc.Users.UserCode).ToList()
                        };

                        rt.Add(notify);
                    });
                }

                return rt;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }

        public async Task<IList<NotifyData>> CheckUnitNotApproveYet()
        {
            try
            {
                var rt = new List<NotifyData>();
                var listUnitNotApproveYet = new List<Units>();
                var wiy = DateTime.Now.Year.DbWeekInYear();
                var cWeek = wiy.Where(c => c.weekStart >= DateTime.Now).Min(c => c.weekNum);

                var yearPair = DateTime.Now.Year;
                var weekPair = cWeek + 1;

                var allUnits = await _unitRepository.GetAllAsync("", 0, 1000);
                foreach (var un in allUnits)
                {
                    var fCostUnit = await _ctx.CostEstimates.FirstOrDefaultAsync(c =>
                        c.UnitId == un.Id && c.ReportForWeek == weekPair && c.CreatedDate.Year == yearPair &&
                        c.Status != (int)GlobalEnums.StatusDefaultEnum.Temporary);
                    if (fCostUnit != null)
                    {
                        if (fCostUnit.Status == (int)GlobalEnums.StatusDefaultEnum.InActive)
                            listUnitNotApproveYet.Add(un);
                    }
                }

                if (listUnitNotApproveYet.Count > 0)
                {
                    var listGroups = new List<string> { "unitmanager", "generalmanager" };
                    var listUnitIds = listUnitNotApproveYet.Select(c => c.Id).ToList();
                    var qJoinUser = from u in _ctx.Users
                                    join ug in _ctx.UserGroups on u.Id equals ug.UserId
                                    join g in _ctx.Groups on ug.GroupId equals g.Id
                                    where listUnitIds.Contains(u.UnitId ?? 0) && listGroups.Contains(g.GroupCode.ToLower())
                                    select new
                                    {
                                        Users = u,
                                        g.GroupCode
                                    };
                    var qJoinUserData = qJoinUser.Distinct().ToList();

                    var usersPerUnit = qJoinUserData.GroupBy(c => c.Users.UnitId ?? 0, (x, y) => new
                    {
                        Unit = x,
                        Users = y.ToList()
                    }).ToList();


                    usersPerUnit.ForEach(c =>
                    {
                        var isSub = listUnitNotApproveYet.First(cc => cc.Id == c.Unit).OfficesSub.Equals("SUB", StringComparison.CurrentCultureIgnoreCase);
                        if (isSub)
                        {
                            var general = c.Users.Where(m =>
                                    m.GroupCode.Equals("GeneralManager", StringComparison.CurrentCultureIgnoreCase))
                                .ToList();
                            if (general.Any())
                                rt.Add(new NotifyData
                                {
                                    Message = "Có dự trù của đơn vị chưa được phê duyệt",
                                    TimeCheck = DateTime.Now,
                                    Receiver = general.Select(cc => cc.Users.UserCode).ToList()
                                });
                        }
                        else
                        {
                            var unitManager = c.Users.Where(m =>
                                    m.GroupCode.Equals("UnitManager", StringComparison.CurrentCultureIgnoreCase))
                                .ToList();
                            if (unitManager.Any())
                                rt.Add(new NotifyData
                                {
                                    Message = "Có dự trù của đơn vị chưa được phê duyệt",
                                    TimeCheck = DateTime.Now,
                                    Receiver = unitManager.Select(cc => cc.Users.UserCode).ToList()
                                });
                        }
                    });

                }

                return rt;

            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }

        public async Task<bool> CanCreate(int unit, bool isSub, int reportWeek)
        {
            try
            {
                var status = await _costStatusesRepository.GetAll();
                var notDecline = status.Where(c =>
                        c.Type == GlobalEnums.CostStatusesElement && c.StatusForSubject == (isSub ? "sub" : "unit") && (c.Value == 0 || c.IsApprove == 1))
                    .Select(c => c.Value).ToList();
                var countAsync = await _ctx.CostEstimates.CountAsync(c =>
                    c.UnitId == unit && notDecline.Contains(c.Status) && c.CreatedDate.Year == DateTime.Now.Year &&
                    c.ReportForWeek == reportWeek);
                return countAsync == 0;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return false;
            }
        }

        // tự động xóa các yc không thuộc dự trù
        // điều kiện tuần lập > N - 2
        // Chạy check hàng ngày 00:00
        public async Task<bool> DeleteUnUsed()
        {
            try
            {
                string guid = Guid.NewGuid().ToString("N");
                Log.Information("{0:dd-MM-yyyy HH:mm:ss} {1} Begin ~ deleting cost estimate items unused", DateTime.Now, guid);
                await using (var transaction = _ctx.Database.BeginTransaction())
                {
                    try
                    {
                        transaction.CreateSavepoint("Before");
                        var cWeek = DateTime.Now.Year.DbWeekInYear().OrderBy(c => c.weekStart >= DateTime.Now)
                            .Min(c => c.weekNum);
                        // các y/c nhỏ hơn 2 tuần trở l
                        // c.PayWeek > cWeek - 5 => giới hạn để tránh việc query toàn bộ dữ liệu
                        //todo: kiểm tra tháng đầu tiên của năm
                        var year = DateTime.Now.Year;
                        var items = await _ctx.CostEstimateItems.Where(c => (c.PayWeek < cWeek - 2) && (c.PayWeek > cWeek - 5)).ToListAsync();
                        if (items.Count > 0)
                        {
                            var listItemIds = items.Select(c => c.Id);
                            var qOnMaps =
                                await _ctx.CostEstimateMapItems.Where(cc => listItemIds.Contains(cc.CostEstimateItemId)).ToListAsync();

                            var notUsed = items.Where(cc => qOnMaps.All(mm => mm.CostEstimateItemId != cc.Id)).ToList();
                            if (notUsed.Count > 0)
                            {
                                _ctx.RemoveRange(notUsed);
                                await _ctx.SaveChangesAsync();
                                await transaction.CommitAsync();
                                Log.Information("{0:dd-MM-yyyy HH:mm:ss} {1} End ~ commit transaction delete ({2}) items", DateTime.Now, guid, notUsed.Count);
                            }
                        }
                        else
                        {
                            Log.Information("{0:dd-MM-yyyy HH:mm:ss} {1} End ~ Not found unused items", DateTime.Now, guid);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Error");
                        await transaction.RollbackToSavepointAsync("Before");
                    }

                }
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return false;
            }
        }

        public async Task<CostEstimateCreateResponse> Create(CostEstimateCreateRequest request)
        {
            try
            {

                var response = new CostEstimateCreateResponse();
                // Điều kiện để tạo dự trù
                // Phải tạo bc thực chi tuần hiện tại
                // i.e: Dự trù tuần 30 -> phải có bc thực chi tuần 29
                //var spentCreateAtWeek = await _actuallySpentRepository.GetActuallySpentApprovedByWeek(request.ReportForWeek - 1, request.MaxFollowStats);
                //if (spentCreateAtWeek == null)
                //{
                //    return new CostEstimateCreateResponse
                //    {
                //        Message = $"Chưa có báo cáo thực chi tuần số {request.ReportForWeek - 1}, không thể tạo dự trù!",
                //        Code = (int)GlobalEnums.ResponseCodeEnum.Error
                //    };
                //}
                await using var transaction = _ctx.Database.BeginTransaction();

                try
                {
                    var listCostItemId = new List<Guid>();

                    request.Data.ForEach(x =>
                    {
                        listCostItemId.Add(Guid.TryParse(x.Id.StringAesDecryption(request.PageRequest), out var g) ? g : Guid.Empty);
                    });

                    var queryGetItems = await _ctx.CostEstimateItems.Where(x =>
                            listCostItemId.Contains(x.Id) && x.Status == (int)GlobalEnums.StatusDefaultEnum.Active)
                        .ToListAsync();

                    if (queryGetItems.Count != request.Data.Count)
                        return new CostEstimateCreateResponse
                        {
                            Message = "Danh sách dữ liệu yêu cầu không hợp lệ!",
                            Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                        };

                    transaction.CreateSavepoint("onCreatePrimaryData");

                    if (!string.IsNullOrEmpty(request.Record))
                    {
                        if (request.CheckApprove == null)
                        {
                            bool canEdit = request.PermissionEdit &&
                                           request.OlderPrimary.Status == (int)GlobalEnums.StatusDefaultEnum.InActive;

                            if (!canEdit)
                            {
                                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                                response.Message = "Không có quyền chỉnh sửa dự trù này!";
                                return response;
                            }
                        }


                        request.Primary.UpdaterId = request.UserId;
                        request.Primary.UpdaterName = request.UserName;
                        request.Primary.UpdatedDate = DateTime.Now;

                        //_ctx.CostEstimates.Update(item);
                        _ctx.Entry(request.OlderPrimary).CurrentValues.SetValues(request.Primary);
                        await _ctx.SaveChangesAsync();

                        if (request.CheckApprove?.IsApproval == true)
                        {
                            var queryMaps = await _ctx.CostEstimateMapItems.Where(x => x.CostEstimateId == request.Primary.Id).ToListAsync();
                            if (queryMaps.Count > 0)
                                _ctx.CostEstimateMapItems.RemoveRange(queryMaps);

                            foreach (var map in request.Data)
                            {
                                await _ctx.CostEstimateMapItems.AddAsync(new CostEstimateMapItems
                                {
                                    Status = (int)GlobalEnums.StatusDefaultEnum.Active,
                                    Id = Guid.NewGuid(),
                                    CostEstimateItemId = Guid.TryParse(map.Id.StringAesDecryption(request.PageRequest), out var g) ? g : Guid.Empty,
                                    CostEstimateId = request.Primary.Id,
                                    CreatorId = request.UserId,
                                    CreatorName = request.UserName,
                                    UpdatedDate = map.IsDeleted == 1 && map.Updater == 0 ? DateTime.Now :
                                        ToDateTime(map.UpdatedDate, "dd/MM/yyyy HH:mm", new CultureInfo("vi-VN")),
                                    Updater = map.IsDeleted == 1 && map.Updater == 0 ? request.UserId : map.Updater,
                                    IsDeleted = map.IsDeleted,
                                    RequestCode = map.RequestCode,
                                    UpdaterName = map.IsDeleted == 1 && map.Updater == 0 ? request.UserName : map.UpdaterName
                                });
                            }
                            await _ctx.SaveChangesAsync();
                        }


                        await _actionLogsRepository.AddLogAsync(new ActionLogs
                        {
                            Action = "Edit",
                            Content = JsonConvert.SerializeObject(request),
                            CreatedDate = DateTime.Now,
                            FunctionUnique = "Edit",
                            UserId = request.UserId,
                            UserName = request.UserName
                        });

                        if (request.CheckApprove != null)
                        {
                            await _ctx.CostEstimateLogs.AddAsync(new CostEstimateLogs
                            {
                                Id = Guid.NewGuid(),
                                CreatedDate = DateTime.Now,
                                UserId = request.UserId,
                                UserName = request.UserName,
                                PositionId = request.CheckApprove.Position.Id,
                                Reason = request.Reason,
                                PositionName = request.CheckApprove.Position.Name,
                                CostEstimateId = request.Primary.Id,
                                ToStatus = request.Primary.Status,
                                ToStatusName = request.Primary.StatusName,
                                FromStatus = request.OlderPrimary.Status
                            });
                            await _ctx.SaveChangesAsync();
                        }

                        await transaction.CommitAsync();

                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = request.CheckApprove == null ? "Cập nhật dự trù thành công!" : request.IsApproval ? "Phê duyệt dự trù thành công!" : "Từ chối dự trù thành công!";
                        response.Record = request.Primary.Id.ToString().StringAesEncryption(request.PageRequest);
                        return response;
                    }


                    #region Đẩy dữ liệu vào bảng chính

                    await _ctx.CostEstimates.AddAsync(request.Primary);
                    await _ctx.SaveChangesAsync();

                    #endregion

                    #region Đẩy dữ liệu vào bảng link

                    foreach (var item in listCostItemId)
                    {
                        await _ctx.CostEstimateMapItems.AddAsync(new CostEstimateMapItems
                        {
                            Status = (int)GlobalEnums.StatusDefaultEnum.Active,
                            Id = Guid.NewGuid(),
                            CostEstimateItemId = item,
                            CostEstimateId = request.Primary.Id,
                            CreatorId = request.UserId,
                            CreatorName = request.UserName
                        });
                        await _ctx.SaveChangesAsync();
                    }

                    #endregion


                    //lưu thêm action logs
                    await _actionLogsRepository.AddLogAsync(new ActionLogs
                    {
                        Action = "Create",
                        Content = JsonConvert.SerializeObject(request),
                        CreatedDate = DateTime.Now,
                        FunctionUnique = "Create",
                        UserId = request.UserId,
                        UserName = request.UserName
                    });

                    await transaction.CommitAsync();

                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Tạo mới dự trù thành công!";
                    response.Record = request.Primary.Id.ToString().StringAesEncryption(request.PageRequest);
                }
                catch (Exception e)
                {
                    Log.Error(e, e.Message);
                    transaction.ReleaseSavepoint("onCreatePrimaryData");
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    response.Message = "Tạo dự trù không thành công!";
                }

                return response;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);

                return new CostEstimateCreateResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = GlobalEnums.ErrorMessage
                };
            }
        }

        /// <summary>
        /// Phê duyệt - Từ chối
        /// </summary>
        /// <param name="request"></param>
        /// <param name="record"></param>
        /// <param name="sigCheck"></param>
        /// <returns></returns>
        public async Task<ApproveRequestOnCostEstimateResponse> Approve(CostEstimateApproveRequest request,
            Database.Models.CostEstimate record, SignatureCheckResponse sigCheck)
        {
            try
            {
                var response = new ApproveRequestOnCostEstimateResponse();

                var queryCostEstimate = record;
                var oNext = sigCheck.Granted;

                request.Status = oNext.Value;
                request.StatusName = oNext.Name;

                await using var transaction = _ctx.Database.BeginTransaction();
                try
                {
                    await transaction.CreateSavepointAsync("Before");
                    await _ctx.CostEstimateLogs.AddAsync(new CostEstimateLogs
                    {
                        Id = Guid.NewGuid(),
                        UserName = request.UserName,
                        CreatedDate = DateTime.Now,
                        FromStatus = queryCostEstimate.Status,
                        UserId = request.UserId,
                        Reason = request.Reason,
                        CostEstimateId = queryCostEstimate.Id,
                        PositionName = sigCheck.Position.Name,
                        ToStatusName = oNext.Name,
                        PositionId = sigCheck.Position.Id,
                        ToStatus = oNext.Value
                    });
                    await _ctx.SaveChangesAsync();

                    queryCostEstimate.Status = request.Status;
                    queryCostEstimate.StatusName = request.StatusName;
                    queryCostEstimate.PathPdf = record.PathPdf;

                    _ctx.CostEstimates.Update(queryCostEstimate);
                    await _ctx.SaveChangesAsync();
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = request.IsApproval ? "Phê duyệt dự trù thành công!" : "Từ chối dự trù thành công!";

                    var item = _mapper.Map<SearchCostEstimateResponseData>(queryCostEstimate);

                    item.Status = queryCostEstimate.StatusName;
                    item.Record = request.Record;

                    // sau khi phê duyệt hoặc từ chối
                    // chỉ còn view được --> tất cả các action khác đều bị vô hiệu

                    item.Viewable = true;
                    item.Editable = false;
                    item.ApproveAble = false;
                    item.DeclineAble = false;
                    item.PathPdf = $"{request.HostFileView}{item.PathPdf}";
                    response.Data = item;

                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Error, e, e.Message);
                    transaction.RollbackToSavepoint("Before");
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    response.Message = "Phê duyệt dự trù không thành công!";
                }

                return response;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);

                return new ApproveRequestOnCostEstimateResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = "Phê duyệt dự trù không thành công!"
                };
            }
        }
        /// <summary>
        /// Danh sách các dự trù trong phiếu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CostEstimateViewResponse> LoadCostEstimateItemsData(CostEstimateViewRequest request)
        {
            try
            {
                var response = new CostEstimateViewResponse();
                var record = await _ctx.CostEstimates.FirstOrDefaultAsync(x => x.Id == request.RawId);
                if (record == null)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.Message = "Không tìm thấy thông tin dự trù!";
                }
                else
                {
                    var mapItems = await _ctx.CostEstimateItems.Join(_ctx.CostEstimateMapItems, x => x.Id,
                            w => w.CostEstimateItemId, (x, y) => new
                            {
                                CostEstimateItems = x,
                                CostEstimateMapItems = y
                            }).Where(p =>
                            p.CostEstimateMapItems.CostEstimateId == request.RawId)
                        .ToListAsync();


                    var dataRt = new List<CostEstimateItemSearchResponseData>();
                    mapItems.ForEach(p =>
                    {
                        var item = _mapper.Map<CostEstimateItemSearchResponseData>(p.CostEstimateItems);
                        item.CreatedDate = p.CostEstimateItems.CreatedDate.ToString("dd/MM/yyyy");
                        item.Id = p.CostEstimateItems.Id.ToString().StringAesEncryption(request.PageRequest);
                        item.StatusName = GlobalEnums.DefaultStatusNames[p.CostEstimateItems.Status];

                        item.IsDeleted = p.CostEstimateMapItems.IsDeleted;

                        item.Updater = p.CostEstimateMapItems.Updater;
                        item.UpdatedDate = p.CostEstimateMapItems.UpdatedDate.ToString("dd/MM/yyyy HH:mm");
                        item.UpdaterName = p.CostEstimateMapItems.UpdaterName;
                        dataRt.Add(item);
                    });


                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Data = dataRt;
                    response.ReportForWeek = record.ReportForWeek;
                    response.RequestType = record.CostEstimateType;
                    response.CostEstimate = record;
                }

                return response;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);

                return new CostEstimateViewResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                    Message = "Không tìm thấy dự trù!"
                };
            }
        }

        public async Task<SignatureCheckResponse> CheckPermissionApprove(CostEstimateCreateRequest request, Database.Models.CostEstimate record)
        {
            try
            {
                var costEstimate = record;
                if (costEstimate == null)
                    return new SignatureCheckResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                        Message = "Không tìm thấy dữ liệu yêu cầu!"
                    };
                // lấy ra mã trạng thái hiện tại
                var statusData = request.StatusAllowsSeen.FirstOrDefault(x => x.Value == costEstimate.Status);
                int intCurrentStats = statusData?.Value ?? -5000;

                if (statusData == null || intCurrentStats == -5000)
                {
                    // không tìm thấy trạng thái hiện tại -> lỗi
                    return new SignatureCheckResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.UnAuthor,
                        Message = "Bạn không có quyền thực hiện thao tác, vui lòng liên hệ với quản trị viên!"
                    };
                }
                var oUnit = await _unitRepository.GetByIdAsync(record.UnitId);
                string rcUnit = oUnit.OfficesSub.Equals("YT", StringComparison.CurrentCultureIgnoreCase)
                    ? GlobalEnums.UnitIn
                    : GlobalEnums.UnitOut;

                var next = new NextStatExtension(await _costStatusesRepository.GetAll(),
                    request.StatusAllowsSeen, GlobalEnums.CostStatusesElement, GlobalEnums.Week, "Dự trù tuần");

                var oNext = next._getNext(costEstimate.Status, request.IsApproval, costEstimate.IsSub, rcUnit, string.Empty);
                if (!oNext.NextValid)
                    return new SignatureCheckResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.UnAuthor,
                        Message = "Bạn không có quyền thực hiện thao tác, vui lòng liên hệ với quản trị viên!"
                    };
                var oGroup = await _costStatusesRepository.GetUsedByGroup(oNext.Next.Id);
                int maxOfStepOrder = request.StatusAllowsSeen.Max(c => c.Order);
                return new SignatureCheckResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Success,
                    IsSignature = oNext.Next.Sign,
                    IsApproval = oNext.Next.IsApprove == 1,
                    Granted = oNext.Next,
                    Position = oGroup,
                    IsMaxStep = maxOfStepOrder == oNext.Next.Order && oNext.Next.IsApprove == 1
                };
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);

                return new SignatureCheckResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = GlobalEnums.ErrorMessage
                };
            }
        }


        DateTime ToDateTime(string value, string format, CultureInfo culture)
            => DateTime.TryParseExact(value, format, culture, DateTimeStyles.None, out var result) ? result : default;
    }
}
