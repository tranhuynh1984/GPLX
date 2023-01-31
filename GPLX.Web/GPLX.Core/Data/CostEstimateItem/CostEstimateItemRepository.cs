using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.CostEstimateItem;
using GPLX.Core.Contracts.Payment;
using GPLX.Core.Contracts.Statuses;
using GPLX.Core.Contracts.Unit;
using GPLX.Core.DTO.Request.CostEstimateItem;
using GPLX.Core.DTO.Response.CostEstimateItem;
using GPLX.Core.DTO.Response.CostStatus;
using GPLX.Core.Enum;
using GPLX.Core.Extensions;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;

namespace GPLX.Core.Data.CostEstimateItem
{
    public class CostEstimateItemRepository : ICostEstimateItemRepository
    {
        private readonly Context _ctx;
        private readonly IActionLogsRepository _actionLogsRepository;
        public static int AllStat = -100;

        private readonly IMapper _mapper;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICostStatusesRepository _costStatusesRepository;
        private readonly IUnitRepository _unitRepository;

        public CostEstimateItemRepository(Context ctx,
            IActionLogsRepository actionLogsRepository,
            IMapper mapper, IPaymentRepository paymentRepository, ICostStatusesRepository costStatusesRepository, IUnitRepository unitRepository)
        {
            _ctx = ctx;
            _actionLogsRepository = actionLogsRepository;
            _mapper = mapper;
            _paymentRepository = paymentRepository;
            _costStatusesRepository = costStatusesRepository;
            _unitRepository = unitRepository;
        }


        public async Task<Database.Models.CostEstimateItem[]> GetByCostEstimateIdAsync(Guid id)
        {
            var ids = _ctx.CostEstimateMapItems.
                Where(a => a.CostEstimateId == id && a.IsDeleted == 0 && a.Status == (int)GlobalEnums.StatusDefaultEnum.Active && a.Status != (int)GlobalEnums.StatusDefaultEnum.Deleted).
                Select(a => a.CostEstimateItemId).ToList();

            return await Task.FromResult(_ctx.CostEstimateItems.Where(a => ids.Contains(a.Id)).ToArray());
        }

        /// <summary>
        /// Lấy ra các yêu cầu đã được phê duyệt mà chưa được chi
        /// </summary>
        /// <param name="week">Tuần - để lấy các dự trù nhỏ hơn tuần lập N-2 </param>
        /// <param name="unit">Đơn vị</param>
        /// <param name="isSub">sub hay đvtv</param>
        /// <param name="unitType"></param>
        /// <returns></returns>
        public async Task<IList<Database.Models.CostEstimateItem>> GetCostEstimateItemNotSpent(int week, int unit, bool isSub, string unitType)
        {
            try
            {
                var res = new List<Database.Models.CostEstimateItem>();
                var allStats = await _costStatusesRepository.GetAll();

                // dự trù được phê duyệt bởi cấp cao nhất
                var maxApproveFollow = allStats.Where(x =>
                    x.Type == GlobalEnums.CostStatusesElement &&
                    x.StatusForSubject.Equals(isSub ? GlobalEnums.ObjectSub : GlobalEnums.ObjectUnit) &&
                    x.StatusForCostEstimateType.Equals(GlobalEnums.Week) && x.IsApprove == 1).OrderByDescending(x => x.Order).FirstOrDefault();

                var getListCostEstimateBefore = await _ctx.CostEstimates.Where(x =>
                    x.Status == maxApproveFollow.Value
                    && x.CostEstimateType == (int)GlobalEnums.StatusDefaultType.Weekly && x.UnitId == unit
                    && x.ReportForWeek >= week - 2
                    && x.ReportForWeek < week).Select(x => x.Id).ToListAsync();

                //không tìm thấy dự trù nào từ 2 tuần trước trở lại
                if (getListCostEstimateBefore.Count == 0)
                    return res;

                var qJoin = from ci in _ctx.CostEstimateItems
                            join map in _ctx.CostEstimateMapItems on ci.Id equals map.CostEstimateItemId
                            join ce in _ctx.CostEstimates on map.CostEstimateId equals ce.Id
                            where getListCostEstimateBefore.Contains(ce.Id) && map.IsDeleted == 0 &&
                                  ce.ReportForWeek >= week - 2 && ce.ReportForWeek < week
                            select ci;
                var dataJoin = await qJoin.Distinct().ToListAsync();

                var listItemsByCostEstimateIds = dataJoin.Select(x => x.Id).ToList();

                //danh sách các yêu cầu đa được chi
                var listSpent = await _ctx.CostEstimateItems.Join(_ctx.CostEstimateItemMapActuallySpentItem, w => w.Id,
                    y => y.CostEstimateItemId, (x, y) => new
                    {
                        CostEstimateItems = x,
                        y.Status
                    }).Where(x => x.Status == (int)GlobalEnums.StatusActuallyEnum.Done || x.Status == (int)GlobalEnums.StatusActuallyEnum.NotDone
                    && listItemsByCostEstimateIds.Contains(x.CostEstimateItems.Id)).Select(x => x.CostEstimateItems.Id).ToListAsync();

                var listNotSpent = dataJoin.Where(x => listSpent.All(m => m != x.Id)).ToList();
                return listNotSpent;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);

                return null;
            }
        }

        public async Task<IList<Database.Models.CostEstimateItem>> GetByListRequestCode(IEnumerable<string> codes)
        {
            try
            {
                var query = _ctx.CostEstimateItems.Where(x => codes.Contains(x.RequestCode.ToUpper()) && x.Status != (int)GlobalEnums.StatusDefaultEnum.Deleted);
                var data = await query.ToListAsync();
                return data;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message, codes);

                return null;
            }
        }

        public async Task<CostEstimateItemApprovalResponse> Delete(CostEstimateItemApprovalRequest request)
        {
            try
            {
                var response = new CostEstimateItemApprovalResponse();
                var query = await _ctx.CostEstimateItems.FirstOrDefaultAsync(x => x.Id == request.RawId && x.UnitId == request.UnitId);
                if (query == null)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.Message = "Không tìm thấy dữ liệu yêu cầu!";
                }
                else
                {
                    if (query.IsLock == 1)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NotOpenCode;
                        response.Message = "Không thể xóa yêu cầu đã được phê duyệt!";
                    }
                    else
                    {
                        query.Status = (int)GlobalEnums.StatusDefaultEnum.Deleted;
                        _ctx.Update(query);
                        await _actionLogsRepository.AddLogAsync(new ActionLogs
                        {
                            Action = "Delete",
                            Content = JsonConvert.SerializeObject(request),
                            CreatedDate = DateTime.Now,
                            FunctionUnique = "Delete",
                            UserId = request.UserId,
                            UserName = request.UserName
                        });
                        _ctx.SaveChanges();

                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = "Xóa yêu cầu thành công!";
                    }
                }

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);

                return new CostEstimateItemApprovalResponse
                {
                    Message = "Lỗi hệ thống, vui lòng thử lại sau!",
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error
                };
            }
        }



        public async Task<Database.Models.CostEstimateItem> GetByIdAsync(Guid id)
        {
            return await _ctx.CostEstimateItems.FirstOrDefaultAsync(a => a.Id == id && a.Status != (int)GlobalEnums.StatusDefaultEnum.Deleted);
        }

        public async Task<Database.Models.CostEstimateItem> GetByCodeAsync(string requestCode, int unitId)
        {
            return await _ctx.CostEstimateItems.FirstOrDefaultAsync(a => a.RequestCode == requestCode && a.UnitId == unitId && a.Status != (int)GlobalEnums.StatusDefaultEnum.Deleted);
        }

        public async Task<bool> CheckBillCodeExists(string billCode)
        {
            try
            {
                if (string.IsNullOrEmpty(billCode))
                    return false;
                bool billCodeNotAvail = (await _ctx.CostEstimateItems.CountAsync(c => c.BillCode.Equals(billCode))) > 0;
                return billCodeNotAvail;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message, billCode);
                return true;
            }
        }

        public async Task<Database.Models.CostEstimateItem> CreateAsync(Database.Models.CostEstimateItem item)
        {
            try
            {
                item.Status = (int)GlobalEnums.StatusDefaultEnum.InActive;

                await _ctx.CostEstimateItems.AddAsync(item);
                await _actionLogsRepository.AddLogAsync(new ActionLogs
                {
                    Action = "Add",
                    Content = JsonConvert.SerializeObject(item),
                    CreatedDate = DateTime.Now,
                    FunctionUnique = "CreateAsync",
                    UserId = item.CreatorId,
                    UserName = item.CreatorName
                });
                _ctx.SaveChanges();
                return item;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message, item);

                return null;
            }
        }
        /// <summary>
        /// Lưu danh sách yêu cầu (Kế toán viên)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CostEstimateCreateResponse> CreateBulk(CostEstimateItemBulkCreateRequest request)
        {
            await using var transaction = _ctx.Database.BeginTransaction();
            try
            {
                var allPayments = await _paymentRepository.AllPayments();
                var allTypes = await _paymentRepository.AllTypes(request.IsSub ? "sub" : request.UnitType);
                await transaction.CreateSavepointAsync("Before");
                Regex numeric = new Regex("[0-9]+");
                var listItemCreates = new List<Database.Models.CostEstimateItem>();
                foreach (var record in request.Data)
                {
                    var toCreateItem = _mapper.Map<Database.Models.CostEstimateItem>(record);

                    record.PayWeek = int.TryParse(numeric.Match(record.PayWeekName).Value, out var n) ? n : -1;
                    toCreateItem.Id = Guid.NewGuid();
                    toCreateItem.Status = (int)GlobalEnums.StatusDefaultEnum.Active;
                    toCreateItem.UnitId = request.UnitId;
                    toCreateItem.UnitName = request.UnitName;
                    toCreateItem.RequestContentNonUnicode = record.RequestContent.StringUnSign();
                    toCreateItem.AccountImage = toCreateItem.AccountImage?.Replace(request.HostPath, string.Empty,
                        StringComparison.OrdinalIgnoreCase);
                    toCreateItem.RequestImage = toCreateItem.AccountImage;
                    toCreateItem.CreatedDate = DateTime.Now;

                    toCreateItem.CostEstimateItemTypeId = record.CostEstimateType;
                    // toCreateItem.RequestContent = $"$.{toCreateItem.RequestCode}$ {toCreateItem.RequestContent}";
                    var typeGet = allTypes.FirstOrDefault(x => x.Id == record.CostEstimateType);
                    if (typeGet != null)
                    {
                        var payment = allPayments.FirstOrDefault(x => x.Id == typeGet.PaymentType);
                        toCreateItem.CostEstimateGroupName = payment?.Name;
                        toCreateItem.CostEstimatePaymentType = payment?.Id ?? -1;
                    }

                    listItemCreates.Add(toCreateItem);
                }
                await _ctx.CostEstimateItems.AddRangeAsync(listItemCreates);
                await _ctx.SaveChangesAsync();
                await transaction.CommitAsync();
                return new CostEstimateCreateResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Success,
                    Message = "Lưu danh sách yêu cầu thành công!"
                };
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);

                await transaction.RollbackToSavepointAsync("Before");
                return new CostEstimateCreateResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = "Lưu danh sách yêu cầu không thành công!"
                };
            }
        }

        // kiểm tra trạng thái phiếu
        // nếu ở trạng thái duyệt thì k đc phép chỉnh sửa
        // nếu ở trạng thái từ chối --> cho phép chỉnh sửa
        // duyệt | từ chối map với bảng CostStatuses cột [IsApprove]
        public async Task<CostEstimateUpdateResponse> UpdateAsync(Guid id, Database.Models.CostEstimateItem request)
        {
            try
            {
                var item = await _ctx.CostEstimateItems.FirstOrDefaultAsync(a => a.Id == id && a.Status != (int)GlobalEnums.StatusDefaultEnum.Deleted);
                if (item == null)
                    return null;
                if (item.IsLock == (int)GlobalEnums.StatusDefaultEnum.Active)
                    return null;
                //var rgxMatchRequestCodeOnContent = new Regex("(^[$.]+)([a-zA-Z0-9-]+[$])", RegexOptions.IgnoreCase);
                // if (!rgxMatchRequestCodeOnContent.IsMatch(request.RequestContent))
                //request.RequestContent = $"$.{item.RequestCode}$ {request.RequestContent}";

                item.RequestContent = request.RequestContent;
                item.CostEstimateItemTypeId = request.CostEstimateItemTypeId;
                item.CostEstimateItemTypeName = request.CostEstimateItemTypeName;
                item.CostEstimatePaymentType = request.CostEstimatePaymentType;
                item.Cost = request.Cost;
                item.PayWeek = request.PayWeek;
                item.PayWeekName = request.PayWeekName;
                item.SupplierId = request.SupplierId;
                item.SupplierName = request.SupplierName;
                item.BillCode = request.BillCode;
                item.BillDate = request.BillDate;
                item.BillCost = request.BillCost;


                if (!string.IsNullOrEmpty(request.BillCode))
                {
                    var recordByBillCode = await _ctx.CostEstimateItems.FirstOrDefaultAsync(c => c.BillCode.Equals(request.BillCode));
                    if (recordByBillCode != null && recordByBillCode.Id != id)
                        return new CostEstimateUpdateResponse { Code = (int)GlobalEnums.ResponseCodeEnum.Error, Msg = "Số hóa đơn/phiếu thu đã tồn tại" };
                }

                string dbImage = item.RequestImage;
                item.RequestImage = !string.IsNullOrEmpty(request.RequestImage) ? request.RequestImage : dbImage;
                // Nếu được chỉnh sửa -> set trạng thái phiếu về chờ phê duyệt
                item.Status = (int)GlobalEnums.StatusDefaultEnum.InActive;

                await _actionLogsRepository.AddLogAsync(new ActionLogs
                {
                    Action = "Update",
                    Content = JsonConvert.SerializeObject(request),
                    CreatedDate = DateTime.Now,
                    FunctionUnique = "UpdateAsync",
                    UserId = request.CreatorId,
                    UserName = request.CreatorName
                });
                _ctx.Update(item);
                _ctx.SaveChanges();

                return new CostEstimateUpdateResponse { Code = (int)GlobalEnums.ResponseCodeEnum.Success };
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                return null;
            }
        }

        public async Task<CostEstimateItemSearchResponse> SearchAsync(CostEstimateItemSearchRequest search, int offset, int limit, string unitType)
        {
            try
            {
                var getAllStats = await _costStatusesRepository.GetAll();
                var statsExtension = new NextStatExtension(getAllStats, search.StatusAllowsSeen,
                    GlobalEnums.CostStatusesElementItem, GlobalEnums.Week, "Yêu cầu");

                var dictionariesCostStatusName = new Dictionary<Guid, string>();

                var query = _ctx.CostEstimateItems.
                    Join(_ctx.Units, comparer => comparer.UnitId, y => y.Id, (x, y) => new
                    {
                        CostEstimateItems = x,
                        y.OfficesSub,
                        y.OfficesCode
                    }).
                    Where(x => x.CostEstimateItems.Status != (int)GlobalEnums.StatusDefaultEnum.Deleted).AsQueryable();

                var subStatusFilter = search.StatusAllowsSeen.Filter(statsExtension.All, true, search.Status);
                var unitStatusFilter = search.StatusAllowsSeen.Filter(statsExtension.All, false, search.Status);

                //trường hợp user được giao quản lý một số đơn vị
                if (search.UserUnitsManages != null && search.UserUnitsManages.Count > 0)
                {
                    var unitIds = search.UserUnitsManages.Select(x => x.OfficeId).ToList();
                    query = query.Where(x => unitIds.Contains(x.CostEstimateItems.UnitId));
                }
                // Đơn vị
                query = query.Where(x => x.CostEstimateItems.UnitId == search.UserUnit);

                // Năm - tuần
                query = query.Where(x => x.CostEstimateItems.EstimateType == search.EstimateType);


                // Phòng ban
                if (search.UserDepartmentId != AllStat)
                    query = query.Where(x => x.CostEstimateItems.DepartmentId == search.UserDepartmentId);

                if (!string.IsNullOrEmpty(search.FromDate))
                    query = query.Where(x => x.CostEstimateItems.CreatedDate >= search.DateFrom);
                if (!string.IsNullOrEmpty(search.ToDate))
                    query = query.Where(x => x.CostEstimateItems.CreatedDate <= search.DateTo);

                if (search.FilterWeek > 0)
                    query = query.Where(c => c.CostEstimateItems.PayWeek == search.FilterWeek);

                // tìm kiếm theo người tạo
                if (search.EqualUser > 0)
                    query = query.Where(x => x.CostEstimateItems.CreatorId == search.EqualUser);

                if (!string.IsNullOrEmpty(search.Keywords))
                    query = query.Where(x => x.CostEstimateItems.RequestContent.Contains(search.Keywords)
                                             || x.CostEstimateItems.RequestContentNonUnicode.Contains(search.KeywordsNonUnicode) || x.CostEstimateItems.RequestCode.Contains(search.Keywords));

                // Tuần lập báo cáo
                // Chỉ lấy các yêu cầu đc lập lớn hơn hoặc bằng tuần lập báo cáo N-2
                if (search.ReportWeek > 0)
                {
                    query = query.Where(x => x.CostEstimateItems.PayWeek >= search.ReportWeek - 2);
                }
                query = query.OrderByDescending(x => x.CostEstimateItems.CreatedDate);
                var data = (await query.ToListAsync()).Select(c => new
                {
                    c.OfficesCode,
                    c.CostEstimateItems,
                    OfficesSub = c.OfficesSub?.Equals("YT", StringComparison.CurrentCultureIgnoreCase) == true ? GlobalEnums.UnitIn : GlobalEnums.UnitOut
                }).ToList();

                //todo: data.Count > 0
                // loại bỏ các y/c nằm trong dự trù chờ duyệt hoặc đã duyệt
                if (search.ReportWeek > 0 && data.Count > 0)
                {
                    // lấy các yêu cầu đã được chọn vào dự trù chờ duyệt hoặc đã duyệt tuần >= N-2
                    var notDecline = getAllStats
                        .Where(c => (c.IsApprove == 1 || c.Value == 0) &&
                                    c.StatusForSubject ==
                                    (search.IsSub ? GlobalEnums.ObjectSub : GlobalEnums.ObjectUnit) &&
                                    c.Type == GlobalEnums.CostStatusesElement).Select(c => c.Value).ToList();

                    var joining = from map in _ctx.CostEstimateMapItems
                                  join cost in _ctx.CostEstimates on map.CostEstimateId equals cost.Id
                                  where cost.UnitId == search.UserUnit && cost.ReportForWeek >= search.ReportWeek - 2 &&
                                        notDecline.Contains(cost.Status) && cost.Id != search.Current
                                  select new { map.CostEstimateItemId, map.CostEstimateId };

                    var dataJoin = await joining.ToListAsync();

                    if (dataJoin.Count > 0)
                        data = data.Where(cm => dataJoin.All(mm => cm.CostEstimateItems.Id != mm.CostEstimateItemId))
                            .ToList();
                }

                var latest = Extensions.Extensions.CreateList(data.FirstOrDefault());
                latest.Clear();
                // filter theo trạng thái được cấu hình
                foreach (var g in data)
                {
                    if (g.CostEstimateItems.IsSub && subStatusFilter.Count > 0)
                    {
                        var subFil = subStatusFilter.FirstOrDefault(c => c.UnitType.Equals(GlobalEnums.ObjectSub, StringComparison.CurrentCultureIgnoreCase));
                        if (subFil?.Values.Any(c => c == g.CostEstimateItems.Status) == true)
                        {
                            if (!dictionariesCostStatusName.ContainsKey(g.CostEstimateItems.Id))
                            {
                                dictionariesCostStatusName.Add(g.CostEstimateItems.Id,
                                    getAllStats.FirstOrDefault(c => c.StatusForSubject.Equals(subFil.UnitType) 
                                                                    && c.Value == g.CostEstimateItems.Status 
                                                                    && c.Type == GlobalEnums.CostStatusesElementItem)?.Name);
                            }
                            latest.Add(g);
                        }
                    }
                    else
                    {
                        if (unitStatusFilter.Count > 0)
                        {
                            foreach (var activatorResult in unitStatusFilter)
                            {
                                if (g.OfficesSub?.Equals(activatorResult.UnitType) == true &&
                                    activatorResult.Values.Contains(g.CostEstimateItems.Status))
                                {
                                    latest.Add(g);

                                    if (!dictionariesCostStatusName.ContainsKey(g.CostEstimateItems.Id))
                                    {
                                        dictionariesCostStatusName.Add(g.CostEstimateItems.Id,
                                            getAllStats.FirstOrDefault(c => c.StatusForSubject.Equals(activatorResult.UnitType) && c.Value == g.CostEstimateItems.Status && c.Type == GlobalEnums.CostStatusesElementItem)?.Name);
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }

                var listResponse = new List<CostEstimateItemSearchResponseData>();

                latest.ForEach(p =>
                {

                    var item = new CostEstimateItemSearchResponseData
                    {
                        RequestContent = p.CostEstimateItems.RequestContent,
                        Status = p.CostEstimateItems.Status,
                        StatusName = dictionariesCostStatusName.TryGetValue(p.CostEstimateItems.Id, out var n) ? n : string.Empty,
                        BillCode = p.CostEstimateItems.BillCode,
                        BillCost = p.CostEstimateItems.BillCost,
                        BillDate = p.CostEstimateItems.BillDate,
                        Cost = p.CostEstimateItems.Cost,
                        CostEstimateItemTypeName = p.CostEstimateItems.CostEstimateItemTypeName,
                        CostEstimatePaymentType = p.CostEstimateItems.CostEstimatePaymentType,
                        CreatorName = p.CostEstimateItems.CreatorName,
                        DepartmentName = p.CostEstimateItems.DepartmentName,
                        Explanation = p.CostEstimateItems.Explanation,
                        Id = p.CostEstimateItems.Id.ToString().StringAesEncryption(search.PageRequest),
                        PayWeekName = p.CostEstimateItems.PayWeekName,
                        SupplierName = p.CostEstimateItems.SupplierName,
                        UnitName = p.CostEstimateItems.UnitName,
                        CreatedDate = p.CostEstimateItems.CreatedDate.ToString("dd/MM/yyyy"),
                        RequestCode = p.CostEstimateItems.RequestCode,

                        Editable = statsExtension._visible(p.CostEstimateItems.Status, NextAction.EDIT,
                            search.PermissionEdit, p.CostEstimateItems.IsSub, p.OfficesSub, string.Empty) && p.CostEstimateItems.CreatorId == search.UserId,
                        Viewable = true,
                        Approvalable = statsExtension._visible(p.CostEstimateItems.Status, NextAction.APPROVED,
                            search.PermissionApprove, p.CostEstimateItems.IsSub, p.OfficesSub, string.Empty),
                        Declineable = statsExtension._visible(p.CostEstimateItems.Status, NextAction.DECLINE,
                            search.PermissionApprove, p.CostEstimateItems.IsSub, p.OfficesSub, string.Empty),
                        Deleteable = statsExtension._visible(p.CostEstimateItems.Status, NextAction.DELETE,
                            search.PermissionDelete, p.CostEstimateItems.IsSub, p.OfficesSub, string.Empty),

                        CostEstimateGroupName = p.CostEstimateItems.CostEstimateGroupName,
                        RequesterName = p.CostEstimateItems.RequesterName,
                        PayForm = p.CostEstimateItems.PayForm,
                        RequestImage = $"{search.FileHostView}/{p.CostEstimateItems.RequestImage}"
                    };
                    int minLevel = statsExtension._minLevel(p.CostEstimateItems.IsSub, p.OfficesSub, string.Empty);
                    if (search.Status != (int)GlobalEnums.StatusDefaultEnum.All)
                    {
                        if (search.Status == (int)GlobalEnums.StatusDefaultEnum.Activator || search.Status == (int)GlobalEnums.StatusDefaultEnum.Active
                                                                                           || search.Status == (int)GlobalEnums.StatusDefaultEnum.Decline)
                            if (!item.Approvalable && !item.Declineable)
                                listResponse.Add(item);

                        if (search.Status == (int)GlobalEnums.StatusDefaultEnum.InActive)
                            if (item.Approvalable || item.Declineable || minLevel == p.CostEstimateItems.Status)
                                listResponse.Add(item);
                    }
                    else
                        listResponse.Add(item);

                });

                return new CostEstimateItemSearchResponse
                {
                    Data = listResponse.Skip(offset).Take(limit).ToList(),
                    Draw = search.Draw,
                    Code = (int)GlobalEnums.ResponseCodeEnum.Success,
                    RecordsFiltered = listResponse.Count,
                    RecordsTotal = listResponse.Count
                };
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message, search);

                return new CostEstimateItemSearchResponse
                {
                    Draw = search.Draw,
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error
                };
            }
        }

        /// <summary>
        /// Duyệt - Từ chối yêu cầu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CostEstimateItemApprovalResponse> ApprovalOrDecline(CostEstimateItemApprovalRequest request)
        {
            if (request.Records != null && request.Records.Count > 0)
                return await _multipleTask(request);
            return await _singleTask(request);
        }

        async Task<CostEstimateItemApprovalResponse> _singleTask(CostEstimateItemApprovalRequest request)
        {
            try
            {
                var result = new CostEstimateItemApprovalResponse();
                var listGrantedViews = request.StatusAllowsSeen.Select(x => x.Value).ToList();
                var joinData = await _ctx.CostEstimateItems.Join(_ctx.Units, comparer => comparer.UnitId, y => y.Id,
                    (x, y) => new
                    {
                        CostEstimateItems = x,
                        y.OfficesSub,
                        y.OfficesCode
                    }).Where(x => listGrantedViews.Contains(x.CostEstimateItems.Status)
                                   && x.CostEstimateItems.UnitId == request.UnitId && x.CostEstimateItems.Status != (int)GlobalEnums.StatusDefaultEnum.Deleted).FirstOrDefaultAsync(x =>
                        x.CostEstimateItems.Id == request.RawId

                );


                if (joinData != null)
                {
                    // Kiểm tra bản ghi phải không lock

                    if (joinData.CostEstimateItems.IsLock == 1)
                    {
                        result.Code = (int)GlobalEnums.ResponseCodeEnum.NotOpenCode;
                        result.Message = "Không thể thay đổi trạng thái của yêu cầu đã được phê duyệt!";
                        return result;
                    }

                    string rcUnit = joinData.OfficesSub.Equals("YT", StringComparison.CurrentCultureIgnoreCase)
                        ? GlobalEnums.UnitIn
                        : GlobalEnums.UnitOut;

                    var next = new NextStatExtension(await _costStatusesRepository.GetAll(), request.StatusAllowsSeen,
                        GlobalEnums.CostStatusesElementItem, GlobalEnums.Week, "Yêu cầu");
                    var sttNext = next._getNext(joinData.CostEstimateItems.Status, request.IsApproval, joinData.CostEstimateItems.IsSub, rcUnit, string.Empty);
                    if (!sttNext.NextValid)
                    {
                        result.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                        result.Message = sttNext.Message;
                        return result;
                    }

                    if (joinData.CostEstimateItems.Status != sttNext.Next.Value)
                    {
                        await using var transaction = _ctx.Database.BeginTransaction();
                        try
                        {
                            var oGroup = await _costStatusesRepository.GetUsedByGroup(sttNext.Next.Id);

                            await _ctx.CostEstimateItemLogs.AddAsync(new CostEstimateItemLogs
                            {
                                Id = Guid.NewGuid(),
                                CostEstimateItemId = request.RawId,
                                CreatedDate = DateTime.Now,
                                FromStatus = joinData.CostEstimateItems.Status,
                                ToStatus = sttNext.Next.Value,
                                PositionId = oGroup.Id,
                                PositionName = oGroup.Name,
                                Reason = request.Reason,
                                UserId = request.UserId,
                                UserName = request.UserName,
                                LogType = CostEstimateItemLogEnums.LogTypeRequest,
                                ToStatusName = sttNext.Next.Name
                            });
                            await _ctx.SaveChangesAsync();
                            transaction.CreateSavepoint("Before");

                            joinData.CostEstimateItems.Status = sttNext.Next.Value;
                            // nếu được phê duyệt
                            // thì yêu cầu sẽ bị khóa
                            if (request.IsApproval)
                                joinData.CostEstimateItems.IsLock = 1;

                            _ctx.CostEstimateItems.Update(joinData.CostEstimateItems);

                            #region Log hành động

                            await _actionLogsRepository.AddLogAsync(new ActionLogs
                            {
                                Action = "Approval",
                                Content = JsonConvert.SerializeObject(request),
                                CreatedDate = DateTime.Now,
                                FunctionUnique = "Approval",
                                UserId = request.UserId,
                                UserName = request.UserName
                            });
                            _ctx.SaveChanges();

                            #endregion

                            await _ctx.SaveChangesAsync();
                            await transaction.CommitAsync();

                            result.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

                            result.Message = request.IsApproval
                                ? "Phê duyệt yêu cầu thành công"
                                : "Từ chối yêu cầu thành công!";
                            result.Data = new CostEstimateItemSearchResponseData
                            {
                                RequestContent = joinData.CostEstimateItems.RequestContent,
                                Status = request.StatusChange,
                                StatusName = GlobalEnums.DefaultStatusNames[request.StatusChange],
                                BillCode = joinData.CostEstimateItems.BillCode,
                                BillCost = joinData.CostEstimateItems.BillCost,
                                BillDate = joinData.CostEstimateItems.BillDate,
                                Cost = joinData.CostEstimateItems.Cost,
                                CostEstimateItemTypeName = joinData.CostEstimateItems.CostEstimateItemTypeName,
                                CreatorName = joinData.CostEstimateItems.CreatorName,
                                DepartmentName = joinData.CostEstimateItems.DepartmentName,
                                Explanation = joinData.CostEstimateItems.Explanation,
                                Id = joinData.CostEstimateItems.Id.ToString().StringAesEncryption(request.PageRequest),
                                PayWeekName = joinData.CostEstimateItems.PayWeekName,
                                SupplierName = joinData.CostEstimateItems.SupplierName,
                                UnitName = joinData.CostEstimateItems.UnitName,
                                CreatedDate = joinData.CostEstimateItems.CreatedDate.ToString("dd/MM/yyyy"),

                                Editable = next._visible(sttNext.Next.Value, NextAction.EDIT, request.PermissionEdit, joinData.CostEstimateItems.IsSub, rcUnit, string.Empty),
                                Viewable = true,
                                Approvalable = next._visible(sttNext.Next.Value, NextAction.APPROVED, true, joinData.CostEstimateItems.IsSub, rcUnit, string.Empty),
                                Declineable = next._visible(sttNext.Next.Value, NextAction.DECLINE, true, joinData.CostEstimateItems.IsSub, rcUnit, string.Empty),
                                Deleteable = false
                            };
                            return result;
                        }
                        catch (Exception e)
                        {
                            await transaction.RollbackToSavepointAsync("Before");
                            Log.Error(e, e.Message, request);

                            result.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                            result.Message = request.IsApproval
                                ? "Không tìm thấy yêu cầu cần phê duyệt"
                                : "Không tìm thấy yêu cầu cần từ chối!";
                            return result;
                        }
                    }

                    result.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    result.Message = (request.IsApproval)
                        ? "Phê duyệt yêu cầu không thành công"
                        : "Từ chối yêu cầu không thành công!";
                    return result;
                }

                // Không tìm thấy bản ghi
                // Hoặc bản ghi đã bị duyệt
                // Bản ghi đã được duyệt -> Đã bị lock
                result.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                result.Message = request.IsApproval
                    ? "Phê duyệt yêu cầu không thành công"
                    : "Từ chối yêu cầu không thành công!";
                return result;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message, request);

                return new CostEstimateItemApprovalResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = request.IsApproval
                        ? "Không tìm thấy yêu cầu cần phê duyệt"
                        : "Không tìm thấy yêu cầu cần từ chối!"

                };
            }
        }

        async Task<CostEstimateItemApprovalResponse> _multipleTask(CostEstimateItemApprovalRequest request)
        {
            try
            {
                CostEstimateItemApprovalResponse cr = new CostEstimateItemApprovalResponse();
                var parseRaw = new List<Guid>();

                foreach (var requestRecord in request.Records)
                {
                    var gFromString = Guid.TryParse(requestRecord.StringAesDecryption(request.PageRequest), out var g);
                    if (gFromString)
                        parseRaw.Add(g);
                }

                if (parseRaw.Count != request.Records.Count)
                {
                    cr.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    cr.Message = "Không tìm thấy dữ liệu yêu cầu";
                    return cr;
                }
                var listGrantedViews = request.StatusAllowsSeen.Select(x => x.Value).ToList();
                var qData = await _ctx.CostEstimateItems.Join(_ctx.Units, comparer => comparer.UnitId, y => y.Id, (x, y) => new
                {
                    CostEstimateItems = x,
                    y.OfficesSub,
                    y.OfficesCode
                }).Where(x => listGrantedViews.Contains(x.CostEstimateItems.Status)
                              && x.CostEstimateItems.UnitId == request.UnitId && x.CostEstimateItems.Status != (int)GlobalEnums.StatusDefaultEnum.Deleted && parseRaw.Contains(x.CostEstimateItems.Id)).
                    ToListAsync();

                if (qData.Count == 0)
                {
                    cr.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    cr.Message = "Thao tác không thành công, vui lòng làm mới dữ liệu!";
                    return cr;
                }

                await using var transaction = _ctx.Database.BeginTransaction();

                try
                {
                    foreach (var q in qData)
                    {
                        var c = await _checkPermission(q.CostEstimateItems, q.OfficesSub, request.IsApproval, request.StatusAllowsSeen);
                        if (c.NextValid)
                        {
                            if (q.CostEstimateItems.Status != c.Next.Value)
                            {
                                var oGroup = await _costStatusesRepository.GetUsedByGroup(c.Next.Id);

                                #region Log trạng thái duyệt

                                await _ctx.CostEstimateItemLogs.AddAsync(new CostEstimateItemLogs
                                {
                                    Id = Guid.NewGuid(),
                                    CostEstimateItemId = q.CostEstimateItems.Id,
                                    CreatedDate = DateTime.Now,
                                    FromStatus = q.CostEstimateItems.Status,
                                    ToStatus = c.Next.Value,
                                    PositionId = oGroup.Id,
                                    PositionName = oGroup.Name,
                                    Reason = request.Reason,
                                    UserId = request.UserId,
                                    UserName = request.UserName,
                                    LogType = CostEstimateItemLogEnums.LogTypeRequest,
                                    ToStatusName = c.Next.Name
                                });
                                await _ctx.SaveChangesAsync();

                                #endregion

                                transaction.CreateSavepoint("Before");

                                q.CostEstimateItems.Status = c.Next.Value;
                                // nếu được phê duyệt
                                // thì yêu cầu sẽ bị khóa
                                if (request.IsApproval)
                                    q.CostEstimateItems.IsLock = 1;

                                _ctx.CostEstimateItems.Update(q.CostEstimateItems);

                                #region Log hành động

                                await _actionLogsRepository.AddLogAsync(new ActionLogs
                                {
                                    Action = "Approval",
                                    Content = JsonConvert.SerializeObject(request),
                                    CreatedDate = DateTime.Now,
                                    FunctionUnique = "Approval",
                                    UserId = request.UserId,
                                    UserName = request.UserName
                                });
                                _ctx.SaveChanges();

                                #endregion

                                await _ctx.SaveChangesAsync();
                            }
                        }
                    }

                    await transaction.CommitAsync();
                    cr.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    cr.Message = request.IsApproval
                        ? "Phê duyệt yêu cầu thành công!"
                        : "Từ chối yêu cầu thành công!";

                    return cr;
                }
                catch (Exception e)
                {
                    await transaction.RollbackToSavepointAsync("Before");
                    Log.Error(e, e.Message, request);
                }
                cr.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                cr.Message = request.IsApproval
                    ? "Phê duyệt yêu cầu không thành công!"
                    : "Từ chối yêu cầu không thành công!";
                return cr;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message, request);
                return new CostEstimateItemApprovalResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = request.IsApproval
                        ? "Phê duyệt yêu cầu không thành công!"
                        : "Từ chối yêu cầu không thành công!"
                };
            }
        }

        async Task<NextResponse> _checkPermission(Database.Models.CostEstimateItem record, string officeType, bool isApproval, IEnumerable<StatusesGranted> statusAllowSeen)
        {
            var rt = new NextResponse();
            if (record.IsLock == 1)
            {
                rt.NextValid = false;
                rt.Message = "Không thể thay đổi trạng thái của yêu cầu đã được phê duyệt!";
                return rt;
            }

            string rcUnit = officeType.Equals("YT", StringComparison.CurrentCultureIgnoreCase) ? GlobalEnums.UnitIn : GlobalEnums.UnitOut;

            var next = new NextStatExtension(await _costStatusesRepository.GetAll(), statusAllowSeen,
                GlobalEnums.CostStatusesElementItem, GlobalEnums.Week, "Yêu cầu");
            var sttNext = next._getNext(record.Status, isApproval, record.IsSub, rcUnit, string.Empty);
            if (!sttNext.NextValid)
            {
                rt.NextValid = false;
                rt.Message = sttNext.Message;
                return rt;
            }

            return sttNext;
        }


        // Tạo mã dự trù khi yêu cầu được duyệt
        // Chỉ khi phiếu được duyệt bởi trưởng phòng
        // Thì phiếu mới được tạo mã
        // không tính các phiếu không được sử dụng
        // không nằm trong bảng link | có trong bảng link mà IsDelete = 1
        public async Task<int> CreateCostEstimateItemCodeForUnit(int unitId)
        {
            try
            {
                // Lấy bản ghi được tạo mới nhất
                var queryGetLast = await _ctx.CostEstimateItems.
                    Where(x => !string.IsNullOrEmpty(x.RequestCode)). // && x.Status != (int)GlobalEnums.StatusDefaultEnum.Deleted
                    OrderByDescending(x => x.CreatedDate).
                    FirstOrDefaultAsync(x => x.UnitId == unitId);
                if (queryGetLast == null)
                    return 0;
                var codeExtract = queryGetLast.RequestCode.Split('-');
                int lastCodeUnit = codeExtract.Length > 1 ? int.TryParse(codeExtract[1], out var i) ? i : -1 : -1;
                return lastCodeUnit;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message, unitId);

                Console.WriteLine(e);
                return -1;
            }
        }

        public Task<Database.Models.CostEstimateItem[]> GetByStatusAsync(int status)
        {
            throw new NotImplementedException();
        }
    }
}
