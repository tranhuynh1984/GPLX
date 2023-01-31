using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.Actually;
using GPLX.Core.Contracts.CashFollow;
using GPLX.Core.Contracts.Dashboard;
using GPLX.Core.Contracts.Payment;
using GPLX.Core.Contracts.Statuses;
using GPLX.Core.Contracts.Unit;
using GPLX.Core.DTO.Request.CashFollow;
using GPLX.Core.DTO.Request.Notify;
using GPLX.Core.DTO.Response;
using GPLX.Core.DTO.Response.CashFollow;
using GPLX.Core.DTO.Response.CostEstimate;
using GPLX.Core.DTO.Response.CostStatus;
using GPLX.Core.Enum;
using GPLX.Core.Extensions;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using Functions = GPLX.Core.Contants.Functions;

namespace GPLX.Core.Data.CashFollow
{
    public class CashFollowRepository : ICashFollowRepository
    {
        private readonly Context _ctx;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IActuallySpentRepository _actuallySpentRepository;
        private readonly ICostStatusesRepository _costStatusesRepository;
        private readonly IMapper _mapper;
        private readonly IActionLogsRepository _actionLogsRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IDashboardRepository _dashboardRepository;

        private const string CashReceiveBusiness = "Tiền thu được từ hoạt động kinh doanh trong kỳ";
        private static readonly string CashSpentPeriod = "Tiền chi trong kỳ";
        //public static readonly string CashEquivalentsBeginningPeriod = "Tiền và tương đương tiền đầu kỳ";
        //public static readonly string CashReceivePeriod = "Tiền thu được trong kỳ";

        public CashFollowRepository(Context ctx, IPaymentRepository paymentRepository, IActuallySpentRepository actuallySpentRepository, ICostStatusesRepository costStatusesRepository, IMapper mapper, IActionLogsRepository actionLogsRepository, IUnitRepository unitRepository, IDashboardRepository dashboardRepository)
        {
            _ctx = ctx;
            _paymentRepository = paymentRepository;
            _actuallySpentRepository = actuallySpentRepository;
            _costStatusesRepository = costStatusesRepository;
            _mapper = mapper;
            _actionLogsRepository = actionLogsRepository;
            _unitRepository = unitRepository;
            _dashboardRepository = dashboardRepository;
        }
        public async Task<IList<CashFollowGroup>> GetListCastFollowTypes(string subject)
        {
            try
            {
                var query = await _ctx.CashFollowGroups.Where(x => x.ForSubject.ToLower().Equals(subject.ToLower())).ToListAsync();
                return query;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }
        public async Task<IList<CashFollowGroup>> GetAllCastFollowTypes()
        {
            try
            {
                var query = await _ctx.CashFollowGroups.ToListAsync();
                return query;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }

        public async Task<CreateCashFollowResponse> CreateAsync(CashFollowCreateRequest request)
        {
            var response = new CreateCashFollowResponse();
            try
            {
                await using var transaction = _ctx.Database.BeginTransaction();
                try
                {
                    var primary = request.CashFollow;
                    primary.Id = Guid.NewGuid();
                    primary.UnitId = request.UnitId;
                    primary.UnitName = request.UnitName;
                    primary.Creator = request.Creator;
                    primary.CreatorName = request.CreatorName;
                    primary.IsSub = request.IsSub;

                    primary.Status = (int)GlobalEnums.StatusDefaultEnum.InActive;
                    primary.StatusName = GlobalEnums.DefaultStatusNames[(int)GlobalEnums.StatusDefaultEnum.InActive];
                    primary.CreatedDate = DateTime.Now;


                    await transaction.CreateSavepointAsync("Before");
                    await _ctx.CashFollow.AddAsync(primary);
                    await _ctx.SaveChangesAsync();

                    var listAggregates = new List<CashFollowAggregates>();
                    foreach (var requestCashFollowAggregateExcel in request.CashFollowAggregateExcels)
                    {
                        var agg = _mapper.Map<CashFollowAggregates>(requestCashFollowAggregateExcel);
                        agg.Id = Guid.NewGuid();
                        agg.CashFollowId = primary.Id;
                        listAggregates.Add(agg);
                    }

                    await _ctx.CashFollowAggregates.AddRangeAsync(listAggregates);
                    await _ctx.SaveChangesAsync();

                    var listDetails = new List<CashFollowItem>();
                    foreach (var cashFollowItemExcel in request.CashFollowItemExcels)
                    {
                        var i = _mapper.Map<CashFollowItem>(cashFollowItemExcel);
                        i.Id = Guid.NewGuid();
                        i.CashFollowId = primary.Id;
                        listDetails.Add(i);
                    }
                    await _ctx.CashFollowItem.AddRangeAsync(listDetails);
                    await _ctx.SaveChangesAsync();

                    transaction.Commit();
                    await _dashboardRepository.SendCreateNotify(GlobalEnums.CashFollow, request.UnitId, primary.IsSub ? "sub" : primary.CashFollowType, primary.Year);
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Record = primary.Id.ToString().StringAesEncryption(request.RequestPage);
                }
                catch (Exception e)
                {
                    await transaction.RollbackToSavepointAsync("Before");
                    Log.Error(e, "Error");
                    return new CreateCashFollowResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                        Message = "Tạo mới kế hoạch dòng tiền không thành công!"
                    };
                }

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return new CreateCashFollowResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = "Tạo mới kế hoạch dòng tiền không thành công!"
                };
            }
        }

        public async Task<Database.Models.CashFollow> GetRawById(Guid g)
        {
            try
            {
                var primaryQuery = await _ctx.CashFollow.FirstOrDefaultAsync(x => x.Id == g);
                return primaryQuery;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }

        public async Task<CashFollowExcelResponse> GetViewCashFollow(Guid g)
        {
            try
            {
                var primary = await _ctx.CashFollow.FirstOrDefaultAsync(x => x.Id == g);
                if (primary == null)
                    return null;

                var cashFollowGroups = await GetListCastFollowTypes(primary.CashFollowType);


                var response = new CashFollowExcelResponse
                {
                    CashFollow = primary
                };
                var aggregates = await _ctx.CashFollowAggregates.Where(x => x.CashFollowId == g).ToListAsync();
                var aggregateExcels = new List<CashFollowAggregateExcel>();


                foreach (var agg in aggregates)
                {
                    var aggregate = _mapper.Map<CashFollowAggregateExcel>(agg);
                    aggregateExcels.Add(aggregate);
                }



                response.CashFollowAggregateExcels = aggregateExcels.OrderBy(x => x.No).ToList();

                var details = new List<CashFollowItemExcel>();
                var cashFollowDetails = await _ctx.CashFollowItem.Where(x => x.CashFollowId == g).ToListAsync();

                foreach (var cashFollowGroup in cashFollowGroups.Where(x => x.ParentId == 0).OrderBy(x => x.Position))
                {
                    var ofG = cashFollowDetails.Where(x => x.CashFollowGroupId == cashFollowGroup.Id).ToList();
                    ofG.ForEach(x =>
                    {
                        var d = _mapper.Map<CashFollowItemExcel>(x);
                        d.Style = cashFollowGroup.ParentId == 0 ? "bold" : string.Empty;
                        details.Add(d);
                    });
                    details.AddRange(_loop(cashFollowDetails, cashFollowGroups.ToList(), cashFollowGroup.Id));
                }

                response.CashFollowItemExcels = details;
                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", g);
                return null;
            }
        }

        IList<CashFollowItemExcel> _loop(List<CashFollowItem> all, List<CashFollowGroup> groups, int parent)
        {
            var subG = groups.Where(x => x.ParentId == parent).ToList();
            var details = new List<CashFollowItemExcel>();
            foreach (var sub in subG.OrderBy(x => x.Position))
            {
                var ofG = all.Where(x => x.CashFollowGroupId == sub.Id).ToList();
                ofG.ForEach(x =>
                {
                    var d = _mapper.Map<CashFollowItemExcel>(x);
                    details.Add(d);
                });
                details.AddRange(_loop(all, groups, sub.Id));
            }

            return details;
        }

        public async Task<CashFollowResponse> SearchAsync(CashFollowSearchRequest request, int start, int length, string unitType)
        {
            try
            {
                var response = new CashFollowResponse();
                var dataView = new List<CashFollowResponseData>();
                var next = new NextStatExtension(await _costStatusesRepository.GetAll(), request.StatusAllowsSeen, GlobalEnums.CashFollow, GlobalEnums.Year, "Kế hoạch dòng tiền");
                var query = _ctx.CashFollow.
                    Join(_ctx.Units, u => u.UnitId, v => v.Id, (x, y) => new
                    {
                        CashFollow = x,
                        y.OfficesSub,
                        y.OfficesCode
                    }).
                    Where(x => (x.CashFollow.UnitId == request.UserUnit || request.UserUnit == (int)GlobalEnums.StatusDefaultEnum.All));

                var subStatusFilter = request.StatusAllowsSeen.Filter(next.All, true, request.Status);
                var unitStatusFilter = request.StatusAllowsSeen.Filter(next.All, false, request.Status);

                if (request.UserUnitsManages != null && request.UserUnitsManages.Count > 0)
                {
                    var unitIds = request.UserUnitsManages.Select(x => x.OfficeId);
                    query = query.Where(x => unitIds.Contains(x.CashFollow.UnitId));
                }

                query = query.Where(x =>
                    x.CashFollow.Year == request.Year || request.Year == (int)GlobalEnums.StatusDefaultEnum.All);

                query = query.OrderByDescending(x => x.CashFollow.CreatedDate);
                var data = (await query.ToListAsync()).Select(c => new
                {
                    c.CashFollow,
                    c.OfficesCode,
                    OfficesSub = c.OfficesSub?.Equals("YT", StringComparison.CurrentCultureIgnoreCase) == true ? GlobalEnums.UnitIn : GlobalEnums.UnitOut
                }).ToList();


                var latest = Extensions.Extensions.CreateList(data.FirstOrDefault());
                latest.Clear();

                foreach (var g in data)
                {
                    if (g.CashFollow.IsSub && subStatusFilter.Count > 0)
                    {
                        var subFil = subStatusFilter.FirstOrDefault(c => c.UnitType.Equals(GlobalEnums.ObjectSub, StringComparison.CurrentCultureIgnoreCase));
                        if (subFil?.Values.Any(c => c == g.CashFollow.Status) == true)
                            latest.Add(g);
                    }
                    else
                    {
                        if (unitStatusFilter.Count > 0)
                        {
                            foreach (var activatorResult in unitStatusFilter)
                            {
                                if (g.OfficesSub?.Equals(activatorResult.UnitType) == true &&
                                    activatorResult.Values.Contains(g.CashFollow.Status))
                                {
                                    latest.Add(g);
                                    break;
                                }

                                if (g.OfficesCode.Equals(activatorResult.UnitType) && activatorResult.Values.Contains(g.CashFollow.Status))
                                {
                                    latest.Add(g);
                                    break;
                                }
                            }
                        }
                    }
                }

                var allSpecialUnitFollowConfigs = await _costStatusesRepository.GetSpecialUnitFollowConfigs();

                foreach (var cashFollow in latest)
                {
                    var fSpecial = allSpecialUnitFollowConfigs.FirstOrDefault(g =>
                        g.UnitCode.Equals(cashFollow.OfficesCode, StringComparison.CurrentCultureIgnoreCase));

                    int minLevel = next._minLevel(cashFollow.CashFollow.IsSub, cashFollow.OfficesSub, fSpecial?.UnitCode);

                    var item = new CashFollowResponseData
                    {
                        CreatedDate = cashFollow.CashFollow.CreatedDate,
                        Creator = cashFollow.CashFollow.CreatorName,
                        PathFile = $"{request.HostFileView}/{cashFollow.CashFollow.PathPdf}",
                        Status = cashFollow.CashFollow.StatusName,
                        UnitName = cashFollow.CashFollow.UnitName,
                        Record = cashFollow.CashFollow.Id.ToString().StringAesEncryption(request.PageRequest),
                        Year = cashFollow.CashFollow.Year,
                        Viewable = true,
                        Editable = next._visible(cashFollow.CashFollow.Status, NextAction.EDIT, request.PermissionEdit, cashFollow.CashFollow.IsSub,
                            cashFollow.CashFollow.CashFollowType, fSpecial?.UnitCode),
                        Approvalable = next._visible(cashFollow.CashFollow.Status, NextAction.APPROVED, request.PermissionApprove, cashFollow.CashFollow.IsSub, cashFollow.CashFollow.CashFollowType, fSpecial?.UnitCode),
                        Declineable = next._visible(cashFollow.CashFollow.Status, NextAction.DECLINE, request.PermissionApprove, cashFollow.CashFollow.IsSub, cashFollow.CashFollow.CashFollowType, fSpecial?.UnitCode),
                        Deleteable = next._visible(cashFollow.CashFollow.Status, NextAction.DELETE, request.PermissionDelete, cashFollow.CashFollow.IsSub,
                            cashFollow.CashFollow.CashFollowType, fSpecial?.UnitCode),
                        TotalCirculation = cashFollow.CashFollow.TotalCirculation,
                        TotalRevenue = cashFollow.CashFollow.TotalRevenue,
                        TotalSpending = cashFollow.CashFollow.TotalSpending
                    };


                    if (request.Status != (int)GlobalEnums.StatusDefaultEnum.All)
                    {
                        if (request.Status == (int)GlobalEnums.StatusDefaultEnum.Activator || request.Status == (int)GlobalEnums.StatusDefaultEnum.Active
                                                                                           || request.Status == (int)GlobalEnums.StatusDefaultEnum.Decline)
                        {
                            if (!item.Approvalable && !item.Declineable)
                                dataView.Add(item);
                        }

                        if (request.Status == (int)GlobalEnums.StatusDefaultEnum.InActive)
                            if (item.Approvalable || item.Declineable || minLevel == cashFollow.CashFollow.Status)
                            {
                                dataView.Add(item);
                            }
                    }
                    else
                        dataView.Add(item);
                }

                response.Data = dataView.Skip(start).Take(length).ToList();
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.RecordsFiltered = dataView.Count;
                response.RecordsTotal = dataView.Count;
                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return new CashFollowResponse
                {
                    Message = "Không tìm thấy dữ liệu yêu cầu!",
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Draw = request.Draw
                };
            }
        }
        /// <summary>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CashFollowApproveResponse> Approval(CashFollowApproveRequest request)
        {
            try
            {
                await using var transaction = _ctx.Database.BeginTransaction();
                var response = new CashFollowApproveResponse();
                try
                {

                    var query = await _ctx.CashFollow.Join(_ctx.Units, u => u.UnitId, v => v.Id, (x, y) => new
                    {
                        CashFollow = x,
                        y.OfficesSub,
                        y.OfficesCode
                    }).FirstOrDefaultAsync(x => x.CashFollow.Id == request.RawId);
                    if (query == null)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        response.Message = "Không tìm thấy dữ liệu yêu cầu!";
                    }
                    else
                    {
                        var allSpecialUnitFollowConfigs = await _costStatusesRepository.GetSpecialUnitFollowConfigs();
                        var statusData = request.StatusAllowsSeen.FirstOrDefault(x => x.Value == query.CashFollow.Status);
                        int intCurrentStats = statusData?.Value ?? -5000;

                        if (statusData == null || intCurrentStats == -5000)
                        {
                            // không tìm thấy trạng thái hiện tại -> lỗi
                            response.Code = (int)GlobalEnums.ResponseCodeEnum.UnAuthor;
                            response.Message = "Bạn không có quyền thực hiện thao tác, vui lòng liên hệ với quản trị viên!";

                            return response;
                        }

                        var next = new NextStatExtension(await _costStatusesRepository.GetAll(), request.StatusAllowsSeen, GlobalEnums.CashFollow, GlobalEnums.Year, "BC Kế hoạch dòng tiền");
                        var fSpecial = allSpecialUnitFollowConfigs.FirstOrDefault(g =>
                            g.UnitCode.Equals(query.OfficesCode, StringComparison.CurrentCultureIgnoreCase));

                        var oNext = next._getNext(query.CashFollow.Status, request.IsApproval, query.CashFollow.IsSub, request.UnitType, fSpecial?.UnitCode);
                        if (!oNext.NextValid)
                        {
                            response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                            response.Message = oNext.Message;
                            return response;
                        }

                        await transaction.CreateSavepointAsync("Before");
                        var oGroup = await _costStatusesRepository.GetUsedByGroup(oNext.Next.Id);
                        await _ctx.CashFollowLog.AddAsync(new CashFollowLog
                        {
                            Id = Guid.NewGuid(),
                            CreatorName = request.UserName,
                            CreatedDate = DateTime.Now,
                            Creator = request.UserId,
                            Reason = !request.IsApproval ? request.Reason : string.Empty,
                            FromStatus = query.CashFollow.Status,
                            ToStatusName = oNext.Next.Name,
                            ToStatus = oNext.Next.Value,
                            CashFollowId = query.CashFollow.Id,
                            PositionId = oGroup.Id,
                            PositionName = oGroup.Name
                        });
                        await _ctx.SaveChangesAsync();

                        query.CashFollow.Status = oNext.Next.Value;
                        query.CashFollow.StatusName = oNext.Next.Name;
                        _ctx.CashFollow.Update(query.CashFollow);
                        await _ctx.SaveChangesAsync();
                        await transaction.CommitAsync();

                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = request.IsApproval
                            ? "Phê duyệt kế hoạch dòng tiền thành công!"
                            : "Từ chối kế hoạch dòng tiền thành công";
                        var data = new CashFollowResponseData
                        {
                            Record = request.Record,
                            Status = query.CashFollow.StatusName,
                            UnitName = query.CashFollow.UnitName,
                            Creator = query.CashFollow.CreatorName,
                            Year = query.CashFollow.Year,
                            PathFile = $"{request.HostFileView}/{query.CashFollow.PathPdf}",
                            CreatedDate = query.CashFollow.CreatedDate,
                            Viewable = true,
                            Declineable = false,
                            Approvalable = false,
                            Editable = false
                        };
                        response.Data = data;
                    }
                    return response;
                }
                catch (Exception e)
                {
                    await transaction.RollbackToSavepointAsync("Before");
                    Log.Error(e, "Error");
                    return new CashFollowApproveResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                        Message = request.IsApproval
                        ? "Phê duyệt kế hoạch dòng tiền không thành công!"
                        : "Từ chối kế hoạch dòng tiền không thành công"
                    };
                }

            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return new CashFollowApproveResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = request.IsApproval
                        ? "Phê duyệt kế hoạch dòng tiền không thành công!"
                        : "Từ chối kế hoạch dòng tiền không thành công"
                };
            }
        }
        public async Task<CashFollowApproveResponse> Approval(CashFollowApproveRequest request, Database.Models.CashFollow record, SignatureCheckResponse sigCheck)
        {
            try
            {
                await using var transaction = _ctx.Database.BeginTransaction();
                var response = new CashFollowApproveResponse();
                try
                {

                    var query = record;
                    if (query == null)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        response.Message = "Không tìm thấy dữ liệu yêu cầu!";
                    }
                    else
                    {

                        query.Status = sigCheck.Granted.Value;
                        query.StatusName = sigCheck.Granted.Name;

                        await transaction.CreateSavepointAsync("Before");

                        _ctx.CashFollow.Update(query);
                        await _ctx.SaveChangesAsync();

                        await _ctx.CashFollowLog.AddAsync(new CashFollowLog
                        {
                            Id = Guid.NewGuid(),
                            CreatorName = request.UserName,
                            CreatedDate = DateTime.Now,
                            Creator = request.UserId,
                            Reason = !request.IsApproval ? request.Reason : string.Empty,
                            FromStatus = query.Status,
                            ToStatusName = sigCheck.Position.Name,
                            ToStatus = sigCheck.Granted.Value,
                            CashFollowId = query.Id,
                            PositionId = sigCheck.Position.Id,
                            PositionName = sigCheck.Position.Name
                        });
                        await _ctx.SaveChangesAsync();

                        await transaction.CommitAsync();

                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = request.IsApproval
                            ? "Phê duyệt kế hoạch dòng tiền thành công!"
                            : "Từ chối kế hoạch dòng tiền thành công";
                        var data = new CashFollowResponseData
                        {
                            Record = request.Record,
                            Status = query.StatusName,
                            UnitName = query.UnitName,
                            Creator = query.CreatorName,
                            Year = query.Year,
                            PathFile = $"{request.HostFileView}/{query.PathPdf}",
                            CreatedDate = query.CreatedDate,
                            Viewable = true,
                            Declineable = false,
                            Approvalable = false,
                            Deleteable = false,
                            Editable = false
                        };
                        response.Data = data;

                        if (!request.IsAuto)
                            await _dashboardRepository.SendOnApproval(new SendFormat
                            {
                                UnitId = query.UnitId,
                                UserId = request.UserId,
                                Year = query.Year,
                                ForSubject = record.IsSub ? "sub" : query.CashFollowType,
                                Level = sigCheck.Granted.Order,
                                PositionCode = sigCheck.Granted.PositionCode,
                                PlanType = GlobalEnums.CashFollow,
                                IsApproval = sigCheck.IsApproval,
                                RecordId = request.RawId,
                                Creator = query.Creator,
                                PositionName = sigCheck.Granted.PositionName
                            });
                    }

                    return response;
                }
                catch (Exception e)
                {
                    await transaction.RollbackToSavepointAsync("Before");
                    Log.Error(e, "Error");
                    return new CashFollowApproveResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                        Message = request.IsApproval
                        ? "Phê duyệt kế hoạch dòng tiền không thành công!"
                        : "Từ chối kế hoạch dòng tiền không thành công"
                    };
                }

            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return new CashFollowApproveResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = request.IsApproval
                        ? "Phê duyệt kế hoạch dòng tiền không thành công!"
                        : "Từ chối kế hoạch dòng tiền không thành công"
                };
            }
        }

        public async Task<SignatureCheckResponse> CheckPermApproval(CashFollowApproveRequest request, Database.Models.CashFollow record)
        {
            try
            {
                var response = new SignatureCheckResponse();
                try
                {
                    var query = record;

                    if (query == null)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        response.Message = "Không tìm thấy dữ liệu yêu cầu!";
                        return response;
                    }

                    var oUnit = await _unitRepository.GetByIdAsync(query.UnitId);
                    var statusData = request.StatusAllowsSeen.FirstOrDefault(x => x.Value == query.Status);
                    int intCurrentStats = statusData?.Value ?? -5000;
                    if (statusData == null || intCurrentStats == -5000)
                    {
                        // không tìm thấy trạng thái hiện tại -> lỗi
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.UnAuthor;
                        response.Message = "Bạn không có quyền thực hiện thao tác, vui lòng liên hệ với quản trị viên!";
                        return response;
                    }

                    var next = new NextStatExtension(await _costStatusesRepository.GetAll(), request.StatusAllowsSeen, GlobalEnums.CashFollow, GlobalEnums.Year, "BC Kế hoạch dòng tiền");
                    var allSpecialUnitFollowConfigs = await _costStatusesRepository.GetSpecialUnitFollowConfigs();
                    var fSpecial = allSpecialUnitFollowConfigs.FirstOrDefault(g =>
                        g.UnitCode.Equals(oUnit.OfficesCode, StringComparison.CurrentCultureIgnoreCase));

                    var oNext = next._getNext(query.Status, request.IsApproval, query.IsSub, query.CashFollowType, fSpecial?.UnitCode);
                    if (!oNext.NextValid)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                        response.Message = oNext.Message;
                        return response;

                    }
                    var oGroup = await _costStatusesRepository.GetUsedByGroup(oNext.Next.Id);

                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.IsApproval = oNext.Next.IsApprove == 1;
                    response.IsSignature = oNext.Next.Sign;
                    response.Granted = oNext.Next;
                    response.Position = oGroup;

                    return response;
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error");
                    return new SignatureCheckResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                        Message = request.IsApproval
                            ? "Phê duyệt kế hoạch dòng tiền không thành công!"
                            : "Từ chối kế hoạch dòng tiền không thành công"
                    };
                }

            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return new SignatureCheckResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = request.IsApproval
                        ? "Phê duyệt kế hoạch dòng tiền không thành công!"
                        : "Từ chối kế hoạch dòng tiền không thành công"
                };
            }
        }

        public async Task<CashFollowExcelResponse> GetRaw(Guid id)
        {
            try
            {
                var response = new CashFollowExcelResponse();
                var primary = await _ctx.CashFollow.FirstOrDefaultAsync(x => x.Id == id);
                response.CashFollow = primary;

                var details = await _ctx.CashFollowItem.Where(x => x.CashFollowId == id).ToListAsync();
                var excelDetails = new List<CashFollowItemExcel>();

                foreach (var cashFollowItem in details)
                {
                    var i = _mapper.Map<CashFollowItemExcel>(cashFollowItem);
                    excelDetails.Add(i);
                }

                response.CashFollowItemExcels = excelDetails;
                return response;

            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", id);
                return null;
            }
        }

        public async Task<IList<CostEstimateLogResponse>> ViewHistories(CashFollowApproveRequest request)
        {
            try
            {
                if (request.RawId != Guid.Empty)
                {
                    var query = _ctx.CashFollowLog.Where(x => x.CashFollowId == request.RawId).
                        OrderByDescending(x => x.CreatedDate).
                        Select(x => new CostEstimateLogResponse
                        {
                            Status = x.ToStatusName,
                            TimeChange = x.CreatedDate.ToString("dd/MM/yyyy HH:mm"),
                            UserName = x.CreatorName,
                            Reason = x.Reason,
                            PositionName = x.PositionName
                        });
                    return await query.ToListAsync();
                }
                return new List<CostEstimateLogResponse>();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return new List<CostEstimateLogResponse>(); ;
            }
        }
        /// <summary>
        /// So sánh giữa ngân sách và thực chi
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CompareCFAndActuallyResponse> ComparingAsync(CompareCFAndActuallyRequest request)
        {
            try
            {
                var response = new CompareCFAndActuallyResponse();
                var record = await GetRaw(request.RawId);
                if (record == null)
                {
                    return new CompareCFAndActuallyResponse
                    {
                        Message = "Không tìm thấy dữ liệu yêu cầu!",
                        Code = (int)GlobalEnums.ResponseCodeEnum.Error
                    };
                }

                var listDbWeeks = request.Year.DbWeekInYear();
                var weekFilters = listDbWeeks.Where(x =>
                   x.weekStart.Month >= request.FromMonth && x.weekFinish.Month <= request.ToMonth && x.weekFinish.Year == request.Year).ToList();
                int startWeek = weekFilters.Min(x => x.weekNum);
                int endWeek = weekFilters.Max(x => x.weekNum);



                // lấy tất cả dữ liệu thực chi trong khoảng thời gian
                var ranges = await _actuallySpentRepository.GetAllByRangeDate(startWeek, endWeek, request.Year, record.CashFollow.UnitId, request.MaxFollowValue);

                var listViews = new List<CompareCFAndActuallyResponseData>();
                // nếu k có dữ liệu 
                // trả mã 204
                if (!ranges.Any())
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.Message = "Không có dữ liệu thực chi trong khoảng thời gian đã chọn!";
                    return response;
                }

                // dữ liệu 111 - 112

                var sctData = await _actuallySpentRepository.GetAllSctDataByActuallyIds(ranges.Select(x => x.Id));
                if (!sctData.Any())
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.Message = "Không có dữ liệu thực chi trong khoảng thời gian đã chọn!";
                    return response;
                }

                // nếu có dữ liệu
                var allTypes = await _paymentRepository.AllTypes(request.IsSub ? "sub" : request.UnitType);
                var allFollowTypes = await GetListCastFollowTypes(request.IsSub ? "sub" : request.UnitType);
                var allActuallySpentItems =
                    await _actuallySpentRepository.GetAllSpentItemsByActuallyIds(ranges.Select(x => x.Id));

                if (!allActuallySpentItems.Any())
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.Message = "Không có dữ liệu thực chi trong khoảng thời gian đã chọn!";
                    return response;
                }

                // tổng các khoản phát sinh nợ
                var cashReceiveBus = sctData.Sum(x => x.IncurredDebt ?? 0);

                // Tổng (Tiền thu được từ hoạt động kinh doanh trong kỳ)

                var cumulativeBudget = record.CashFollowItemExcels.FirstOrDefault(x =>
                        (x.CashFollowGroupName ?? string.Empty).Equals(CashReceiveBusiness, StringComparison.OrdinalIgnoreCase));

                if (cumulativeBudget == null)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.Message = "Không có dữ liệu tiền thu từ hoạt động kinh doanh!";
                    return response;
                }

                var cashReceiveBusiness = new CompareCFAndActuallyResponseData
                {
                    Content = CashReceiveBusiness,
                    TypeName = string.Empty,
                    No = 1,
                    CumulativeActuallySpent = cashReceiveBus,
                    CumulativeBudget = SumDataMonth(request.FromMonth, request.ToMonth, cumulativeBudget),
                    IsBold = false
                };
                listViews.Add(cashReceiveBusiness);

                // các mục lớn
                foreach (var followType in allFollowTypes.Where(p => p.Name.Equals(CashSpentPeriod, StringComparison.OrdinalIgnoreCase)))
                {
                    int counter = 2;
                    var topLevel = new CompareCFAndActuallyResponseData { No = counter };
                    // lấy các nhóm cấp nhỏ
                    var subTypes = allFollowTypes.Where(x => x.ParentId == followType.Id).OrderBy(x => x.Position).ToList();
                    var listLevel = new List<CompareCFAndActuallyResponseData>();
                    if (subTypes.Count > 0)
                    {
                        foreach (var cashFollowType in subTypes)
                        {
                            counter++;
                            var cashFollowRoot = new CompareCFAndActuallyResponseData { No = counter };

                            // kiểm tra xem có được gán với nhóm chi phí không

                            if (!string.IsNullOrEmpty(cashFollowType.RefPaymentId))
                            {
                                var payments = cashFollowType.RefPaymentId.Split(';')
                                    .Select(x => int.TryParse(x, out var i) ? i : 0).ToList();
                                // các loại chi gán với nhóm
                                var listCostTypeByPayments =
                                    allTypes.Where(x => payments.Contains(x.PaymentType)).ToList();


                                var subItems = new List<CompareCFAndActuallyResponseData>();
                                // thống kê theo từng nhóm

                                foreach (var listCostTypeByPayment in listCostTypeByPayments)
                                {
                                    counter++;
                                    // dữ liệu từ BC dòng tiền 
                                    // từ tháng - tháng
                                    // theo nhóm

                                    //todo: xem chuyển về sử dụng cùng 1 bảng
                                    var cashFollowTypeMatch =
                                        allFollowTypes.FirstOrDefault(x => x.Name.Equals(listCostTypeByPayment.Name, StringComparison.OrdinalIgnoreCase));


                                    var oFromCashFollow = record.CashFollowItemExcels.FirstOrDefault(x =>
                                        x.CashFollowGroupId == cashFollowTypeMatch?.Id);

                                    double fromCashFollow = oFromCashFollow == null ? 0 : SumDataMonth(request.FromMonth, request.ToMonth, oFromCashFollow);

                                    var fromActually = allActuallySpentItems.Where(x =>
                                        x.CostEstimateItemTypeId == listCostTypeByPayment.Id).Sum(x => x.ActualSpent);
                                    var typeName = listCostTypeByPayment.ComparingType ?? string.Empty;

                                    // vượt chi
                                    // công thức: tổng thực chi - tổng dòng tiền
                                    double overBudget = fromActually - fromCashFollow < 0 ? 0 : fromActually - fromCashFollow;
                                    var addItem = new CompareCFAndActuallyResponseData
                                    {
                                        Content = listCostTypeByPayment.Name,
                                        IsBold = false,
                                        CumulativeBudget = fromCashFollow,
                                        CumulativeActuallySpent = fromActually,
                                        // thực chi - ngân sách < 0 thì set 0
                                        OverBudget = overBudget,

                                        BudgetRate = Math.Round(fromCashFollow / cashReceiveBusiness.CumulativeBudget * 100),

                                        ActuallyRate = Math.Round(fromActually / (double)cashReceiveBus * 100),

                                        OverBudgetRate = typeName.Equals("A1") && fromActually > fromCashFollow ?
                                            fromActually - fromCashFollow :
                                            (typeName.Equals("A2") || typeName.Equals("A3")) && fromCashFollow > 0 ? Math.Round(overBudget / fromCashFollow * 100) : 0,
                                        No = counter,
                                        TypeName = typeName
                                    };
                                    subItems.Add(addItem);
                                }

                                var sumCumulativeBudget = subItems.Sum(x => x.CumulativeBudget);
                                var sumCumulativeActuallySpent = subItems.Sum(x => x.CumulativeActuallySpent);
                                cashFollowRoot = new CompareCFAndActuallyResponseData
                                {
                                    Content = cashFollowType.Name,
                                    CumulativeBudget = sumCumulativeBudget,
                                    CumulativeActuallySpent = sumCumulativeActuallySpent,
                                    OverBudget = sumCumulativeActuallySpent - sumCumulativeBudget < 0 ? 0 : sumCumulativeActuallySpent - sumCumulativeBudget,
                                    No = cashFollowRoot.No,
                                    IsBold = true
                                };
                                listViews.Add(cashFollowRoot);
                                listViews.AddRange(subItems);
                            }
                            else
                            {
                                cashFollowRoot = new CompareCFAndActuallyResponseData
                                {
                                    Content = cashFollowType.Name,
                                    CumulativeBudget = 0,
                                    CumulativeActuallySpent = 0,
                                    OverBudget = 0,
                                    No = cashFollowRoot.No,
                                    IsBold = true
                                };
                                listViews.Add(cashFollowRoot);
                            }
                            listLevel.Add(cashFollowRoot);
                        }
                    }

                    var topCumulativeActuallySpent = listLevel.Sum(x => x.CumulativeActuallySpent);
                    var topCumulativeBudget = listLevel.Sum(x => x.CumulativeBudget);
                    topLevel = new CompareCFAndActuallyResponseData
                    {
                        No = topLevel.No,
                        Content = followType.Name,
                        IsBold = true,
                        CumulativeActuallySpent = topCumulativeActuallySpent,
                        CumulativeBudget = topCumulativeBudget,
                        OverBudget = topCumulativeActuallySpent - topCumulativeBudget < 0 ? 0 : topCumulativeActuallySpent - topCumulativeBudget
                    };
                    listViews.Add(topLevel);
                }

                response.Data = listViews.OrderBy(x => x.No).ToList();
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return new CompareCFAndActuallyResponse
                {
                    Message = "Không tìm thấy dữ liệu yêu cầu!",
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error
                };
            }
        }

        public async Task<CashFollowApproveResponse> Delete(CashFollowApproveRequest request)
        {
            try
            {
                var response = new CashFollowApproveResponse();
                var record = await GetRawById(request.RawId);

                if (record == null)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.Message = GlobalEnums.NoContentMessage;
                    return response;
                }

                if (record.Status != (int)GlobalEnums.StatusDefaultEnum.InActive)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NotOpenCode;
                    response.Message = "Không thể xóa kế hoạch này!";
                    return response;
                }

                record.Status = (int)GlobalEnums.StatusDefaultEnum.Deleted;
                _ctx.Update(record);
                await _actionLogsRepository.AddLogAsync(new ActionLogs
                {
                    Action = "Delete",
                    Content = JsonConvert.SerializeObject(request),
                    CreatedDate = DateTime.Now,
                    FunctionUnique = Functions.CashFollowView,
                    UserId = request.UserId,
                    UserName = request.UserName
                });
                _ctx.SaveChanges();

                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Message = "Xóa kế hoạch thành công!";
                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", request);
                return new CashFollowApproveResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = GlobalEnums.ErrorMessage
                };
            }
        }
        public async Task<SignatureCheckResponse> _decline(CashFollowApproveRequest request, string positionAct)
        {
            try
            {
                var response = new SignatureCheckResponse();
                try
                {
                    var listIds = request.StatusAllowsSeen.Select(c => c.Id);
                    var statusGroup = await _ctx.CostStatuses.Join(_ctx.CostStatusesGroups, c => c.Id, d => d.StatusesId,
                        (x, y) => new
                        {
                            Status = x,
                            Position = y.GroupCode,
                            y.Type
                        }).Where(c => listIds.Contains(c.Status.Id) && !string.IsNullOrEmpty(c.Type)).ToListAsync();

                    var decline = statusGroup.FirstOrDefault(c => c.Position.Equals(positionAct) && c.Status.IsApprove == 0 && c.Type.Equals("Used", StringComparison.CurrentCultureIgnoreCase));
                    if (decline == null)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                        response.Message = "Không có quyền thực hiện thao tác";
                        return response;
                    }

                    var oGroup = await _costStatusesRepository.GetUsedByGroup(decline.Status.Id);
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.IsApproval = false;
                    response.Granted = new StatusesGranted
                    {
                        Value = decline.Status.Value,
                        Name = decline.Status.Name
                    };
                    response.Position = oGroup;
                    return response;
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error");
                    return new SignatureCheckResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                        Message = "Từ chối kế hoạch dòng tiền không thành công"
                    };
                }

            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return new SignatureCheckResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = "Từ chối kế hoạch dòng tiền không thành công"
                };
            }
        }
        public async Task<bool> AutoDecline(int financeYear, CashFollowApproveRequest declineModel, string positionAct)
        {
            try
            {
                var listCashFollowFinanceYear = await _ctx.CashFollow.Where(c => c.Year == financeYear && c.UnitId == declineModel.UnitId).ToListAsync();

                if (listCashFollowFinanceYear.Count == 0)
                    return true;
                var listIds = listCashFollowFinanceYear.Select(c => c.Id);
                var listCashFollowLogs =
                    await _ctx.CashFollowLog.Where(c => listIds.Contains(c.CashFollowId) && !string.IsNullOrEmpty(c.Reason)).ToListAsync();

                var checkDecline = await _decline(declineModel, positionAct);
                if (checkDecline.Code != (int)GlobalEnums.ResponseCodeEnum.Success)
                    return false;
                declineModel.IsAuto = true;
                foreach (var cashFollow in listCashFollowFinanceYear)
                    if (listCashFollowLogs.All(c => c.CashFollowId != cashFollow.Id)) //chỉ từ chối các BM không ở trạng thái duyệt
                        await Approval(declineModel, cashFollow, checkDecline);

                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", financeYear);
                return false;
            }
        }

        public double SumDataMonth(int from, int to, CashFollowItemExcel cumulativeBudget)
        {
            var totalSumBudget = 0.0;
            for (int i = from; i <= to; i++)
            {
                switch (i)
                {
                    case 1:
                        totalSumBudget += cumulativeBudget.M1;
                        break;
                    case 2:
                        totalSumBudget += cumulativeBudget.M2;
                        break;
                    case 3:
                        totalSumBudget += cumulativeBudget.M3;
                        break;
                    case 4:
                        totalSumBudget += cumulativeBudget.M4;
                        break;
                    case 5:
                        totalSumBudget += cumulativeBudget.M5;
                        break;
                    case 6:
                        totalSumBudget += cumulativeBudget.M6;
                        break;
                    case 7:
                        totalSumBudget += cumulativeBudget.M7;
                        break;
                    case 8:
                        totalSumBudget += cumulativeBudget.M8;
                        break;
                    case 9:
                        totalSumBudget += cumulativeBudget.M9;
                        break;
                    case 10:
                        totalSumBudget += cumulativeBudget.M10;
                        break;
                    case 11:
                        totalSumBudget += cumulativeBudget.M11;
                        break;
                    case 12:
                        totalSumBudget += cumulativeBudget.M12;
                        break;
                }
            }

            return totalSumBudget;
        }
    }
}
