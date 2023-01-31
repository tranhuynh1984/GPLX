using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.Dashboard;
using GPLX.Core.Contracts.Profit;
using GPLX.Core.Contracts.Statuses;
using GPLX.Core.Contracts.Unit;
using GPLX.Core.DTO.Request.Notify;
using GPLX.Core.DTO.Request.Profit;
using GPLX.Core.DTO.Response;
using GPLX.Core.DTO.Response.CostStatus;
using GPLX.Core.DTO.Response.ProfitPlan;
using GPLX.Core.Enum;
using GPLX.Core.Extensions;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using Functions = GPLX.Core.Contants.Functions;

namespace GPLX.Core.Data.Profit
{
    public class ProfitPlanRepository : IProfitPlanRepository
    {
        private readonly Context _ctx;
        private readonly IMapper _mapper;
        private readonly ICostStatusesRepository _costStatusesRepository;
        private readonly IActionLogsRepository _actionLogsRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IDashboardRepository _dashboardRepository;

        public ProfitPlanRepository(Context ctx, IMapper mapper, ICostStatusesRepository costStatusesRepository, IActionLogsRepository actionLogsRepository, IUnitRepository unitRepository, IDashboardRepository dashboardRepository)
        {
            _ctx = ctx;
            _mapper = mapper;
            _costStatusesRepository = costStatusesRepository;
            _actionLogsRepository = actionLogsRepository;
            _unitRepository = unitRepository;
            _dashboardRepository = dashboardRepository;
        }

        public async Task<IList<ProfitPlanGroups>> ListGroups(string subject)
        {
            try
            {
                var q = await _ctx.ProfitPlanGroups.Where(x => x.ForSubject.ToLower().Equals(subject.ToLower())).ToListAsync();
                return q;
            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", subject);
                return null;
            }
        }

        public async Task<ProfitPlanCreateResponse> Create(ProfitPlanCreateRequest request)
        {
            var response = new ProfitPlanCreateResponse();
            try
            {

                // rule: kiểm tra đã tạo doanh thu khách hàng chưa 

                var findRevenueInYear = await _ctx.RevenuePlan.Where(x =>
                    x.Year == request.ProfitPlan.Year && x.UnitId == request.UnitId &&
                    x.Status != (int)GlobalEnums.StatusDefaultEnum.Deleted).OrderByDescending(x => x.CreatedDate).FirstOrDefaultAsync();

                if (findRevenueInYear == null)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.Message = $"Chưa có kế hoạch doanh thu - khách hàng năm tài chính {request.ProfitPlan.Year}!";
                    return response;
                }

                // rule: Tổng doanh thu kế hoạch khách hàng = Tổng lợi nhuận 

                var dataRevenue = await _ctx.RevenuePlanCashDetails.Where(x => x.RevenuePlanId == findRevenueInYear.Id)
                    .ToListAsync();

                var recordTotalRevenue =
                    dataRevenue.FirstOrDefault(x => x.RevenuePlanContentName.Contains("Tổng doanh thu", StringComparison.CurrentCultureIgnoreCase));
                if (recordTotalRevenue == null)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.Message = "Không tìm thấy dữ liệu doanh thu - khách hàng!";
                    return response;
                }

                var totalExpectRevenue = recordTotalRevenue.Total;
                var totalRevenue = request.ProfitPlanAggregates.FirstOrDefault(x =>
                    x.ProfitPlanGroupName.StartsWith("Tổng doanh thu", StringComparison.CurrentCultureIgnoreCase));

                if (totalRevenue == null)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.Message = "Không tìm thấy dữ liệu tổng doanh thu!";
                    return response;
                }

                if (totalExpectRevenue != totalRevenue.TotalCosh)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.Message = "Tổng doanh thu không khớp so với kế hoạch doanh thu - khách hàng!";
                    return response;
                }

                var transaction = _ctx.Database.BeginTransaction();
                await transaction.CreateSavepointAsync("Before");
                try
                {
                    #region bảng chính
                    var primary = request.ProfitPlan;
                    primary.Id = Guid.NewGuid();
                    primary.Status = (int)GlobalEnums.StatusDefaultEnum.InActive;
                    primary.StatusName = GlobalEnums.DefaultStatusNames[(int)GlobalEnums.StatusDefaultEnum.InActive];
                    primary.IsSub = request.IsSub;
                    primary.Creator = request.Creator;
                    primary.CreatorName = request.CreatorName;
                    primary.CreatedDate = DateTime.Now;
                    primary.UnitId = request.UnitId;
                    primary.UnitName = request.UnitName;
                    primary.CreatedDate = DateTime.Now;
                    await _ctx.ProfitPlan.AddAsync(primary);
                    await _ctx.SaveChangesAsync();
                    #endregion

                    #region bảng tổng hợp
                    var aggregates = new List<ProfitPlanAggregates>();
                    foreach (var rqAggregate in request.ProfitPlanAggregates)
                    {
                        var agg = _mapper.Map<ProfitPlanAggregates>(rqAggregate);
                        agg.Id = Guid.NewGuid();
                        agg.ProfitPlanId = primary.Id;
                        aggregates.Add(agg);
                    }

                    await _ctx.ProfitPlanAggregates.AddRangeAsync(aggregates);
                    await _ctx.SaveChangesAsync();

                    #endregion

                    #region Bảng chi tiết
                    var details = new List<ProfitPlanDetails>();
                    foreach (var rqDetail in request.ProfitPlanDetails)
                    {
                        var d = _mapper.Map<ProfitPlanDetails>(rqDetail);
                        d.Id = Guid.NewGuid();
                        d.ProfitPlanId = primary.Id;
                        details.Add(d);
                    }

                    await _ctx.ProfitPlanDetails.AddRangeAsync(details);
                    await _ctx.SaveChangesAsync();
                    #endregion

                    await transaction.CommitAsync();
                    await _dashboardRepository.SendCreateNotify(GlobalEnums.Profit, request.UnitId, primary.IsSub ? "sub" : primary.ProfitPlanType, primary.Year);
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Tạo kế hoạch lợi nhuận thành công!";
                }
                catch (Exception e)
                {
                    await transaction.RollbackToSavepointAsync("Before");
                    Log.Error(e, "exception {0}", e.Message);
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    response.Message = GlobalEnums.ErrorMessage;
                }
            }
            catch (Exception e)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.ErrorMessage;
                Log.Error(e, "exception {0}", e.Message);
            }

            return response;
        }

        public async Task<SearchProfitPlanResponse> SearchAsync(SearchProfitPlanRequest request, int offset, int limit, string unitType)
        {
            try
            {
                var next = new NextStatExtension(await _costStatusesRepository.GetAll(), request.StatusAllowsSeen,
                    GlobalEnums.Profit, GlobalEnums.Year, "Kế hoạch lợi nhuận");

                var response = new SearchProfitPlanResponse();
                var searchQuery = _ctx.ProfitPlan.
                    Join(_ctx.Units, c => c.UnitId, w => w.Id, (x, y) => new
                    {
                        ProfitPlan = x,
                        y.OfficesCode,
                        y.OfficesSub
                    }).
                    AsQueryable();

                var subStatusFilter = request.StatusAllowsSeen.Filter(next.All, true, request.Status);
                var unitStatusFilter = request.StatusAllowsSeen.Filter(next.All, false, request.Status);

                if (request.UserUnit != (int)GlobalEnums.StatusDefaultEnum.All)
                    searchQuery = searchQuery.Where(p => p.ProfitPlan.UnitId == request.UserUnit);
                if (request.Year != (int)GlobalEnums.StatusDefaultEnum.All)
                    searchQuery = searchQuery.Where(p => p.ProfitPlan.Year == request.Year);

                // trạng thái mà user được phép thấy
                //searchQuery = searchQuery.Where(p =>
                //    request.StatusAllowsSeen.ActivatorFilter(next.All, true, request.Status, unitType).Contains(p.ProfitPlan.Status) && p.ProfitPlan.IsSub ||
                //    request.StatusAllowsSeen.ActivatorFilter(next.All, false, request.Status, unitType).Contains(p.ProfitPlan.Status) && !p.ProfitPlan.IsSub
                //);

                if (request.UserUnitsManages != null && request.UserUnitsManages.Count > 0)
                {
                    var unitIds = request.UserUnitsManages.Select(x => x.OfficeId);
                    searchQuery = searchQuery.Where(p => unitIds.Contains(p.ProfitPlan.UnitId));
                }

                if (!string.IsNullOrEmpty(request.Keywords))
                    searchQuery = searchQuery.Where(p => p.ProfitPlan.UnitName.Contains(request.Keywords));

                var dataSearch = (await searchQuery.
                    OrderByDescending(x => x.ProfitPlan.CreatedDate).
                    ToListAsync()).Select(c => new
                    {
                        c.ProfitPlan,
                        c.OfficesCode,
                        OfficesSub = c.OfficesSub?.Equals("YT", StringComparison.CurrentCultureIgnoreCase) == true ? GlobalEnums.UnitIn : GlobalEnums.UnitOut
                    }).ToList();


                var latest = Extensions.Extensions.CreateList(dataSearch.FirstOrDefault());
                latest.Clear();

                // filter theo trạng thái được cấu hình
                foreach (var g in dataSearch)
                {
                    if (g.ProfitPlan.IsSub && subStatusFilter.Count > 0)
                    {
                        var subFil = subStatusFilter.FirstOrDefault(c => c.UnitType.Equals(GlobalEnums.ObjectSub, StringComparison.CurrentCultureIgnoreCase));
                        if (subFil?.Values.Any(c => c == g.ProfitPlan.Status) == true)
                            latest.Add(g);
                    }
                    else
                    {
                        if (unitStatusFilter.Count > 0)
                        {
                            foreach (var activatorResult in unitStatusFilter)
                            {
                                if (g.OfficesSub?.Equals(activatorResult.UnitType) == true &&
                                    activatorResult.Values.Contains(g.ProfitPlan.Status))
                                {
                                    latest.Add(g);
                                    break;
                                }

                                if (g.OfficesCode.Equals(activatorResult.UnitType) && activatorResult.Values.Contains(g.ProfitPlan.Status))
                                {
                                    latest.Add(g);
                                    break;
                                }
                            }
                        }
                    }
                }


                var data = new List<SearchProfitPlanResponseData>();
                var allSpecialUnitFollowConfigs = await _costStatusesRepository.GetSpecialUnitFollowConfigs();

                foreach (var x in latest)
                {
                    var fSpecial = allSpecialUnitFollowConfigs.FirstOrDefault(g =>
                        g.UnitCode.Equals(x.OfficesCode, StringComparison.CurrentCultureIgnoreCase));

                    var item = _mapper.Map<SearchProfitPlanResponseData>(x.ProfitPlan);
                    item.Record = x.ProfitPlan.Id.ToString().StringAesEncryption(request.PageRequest);

                    int minLevel = next._minLevel(x.ProfitPlan.IsSub, x.OfficesSub, fSpecial?.UnitCode);

                    item.Approvalable = next._visible(x.ProfitPlan.Status, NextAction.APPROVED, request.PermissionApprove, x.ProfitPlan.IsSub, x.ProfitPlan.ProfitPlanType, fSpecial?.UnitCode);
                    item.Declineable = next._visible(x.ProfitPlan.Status, NextAction.DECLINE, request.PermissionApprove, x.ProfitPlan.IsSub, x.ProfitPlan.ProfitPlanType, fSpecial?.UnitCode);
                    item.Editable = next._visible(x.ProfitPlan.Status, NextAction.EDIT, request.PermissionEdit, x.ProfitPlan.IsSub, x.ProfitPlan.ProfitPlanType, fSpecial?.UnitCode);
                    item.Deleteable = next._visible(x.ProfitPlan.Status, NextAction.DELETE, request.PermissionDelete, x.ProfitPlan.IsSub, x.ProfitPlan.ProfitPlanType, fSpecial?.UnitCode);
                    item.Viewable = true;
                    item.PathPdf = $"{request.HostFileView}{item.PathPdf}";


                    if (request.Status != (int)GlobalEnums.StatusDefaultEnum.All)
                    {
                        if (request.Status == (int)GlobalEnums.StatusDefaultEnum.Activator || request.Status == (int)GlobalEnums.StatusDefaultEnum.Active
                                                                                           || request.Status == (int)GlobalEnums.StatusDefaultEnum.Decline)
                        {
                            if (!item.Approvalable && !item.Declineable)
                                data.Add(item);
                        }

                        if (request.Status == (int)GlobalEnums.StatusDefaultEnum.InActive)
                            if (item.Approvalable || item.Declineable || minLevel == x.ProfitPlan.Status)
                            {
                                data.Add(item);
                            }
                    }
                    else
                        data.Add(item);
                }


                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Draw = request.Draw;
                response.RecordsFiltered = data.Count;
                response.RecordsTotal = data.Count;
                response.Data = data.Skip(offset).Take(limit).ToList();
                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, "request {0}", request);
                return new SearchProfitPlanResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                    Data = null,
                    Draw = request.Draw,
                    Message = "Không tìm thấy dữ liệu yêu cầu"
                };
            }
        }

        public async Task<ProfitPlanApproveResponse> Approval(ProfitPlanApproveRequest request, ProfitPlan record, SignatureCheckResponse sigCheck)
        {
            try
            {
                var response = new ProfitPlanApproveResponse();

                var profitPlan = record;

                await using var transaction = _ctx.Database.BeginTransaction();
                try
                {
                    await transaction.CreateSavepointAsync("Before");
                    await _ctx.ProfitPlanLogs.AddAsync(new ProfitPlanLogs
                    {
                        Id = Guid.NewGuid(),
                        Creator = request.UserId,
                        CreatedDate = DateTime.Now,
                        FromStatus = profitPlan.Status,
                        CreatorName = request.UserName,
                        Reason = request.Reason,
                        ProfitPlanId = profitPlan.Id,
                        PositionName = sigCheck.Position.Name,
                        ToStatusName = sigCheck.Granted.Name,
                        PositionId = sigCheck.Position.Id,
                        ToStatus = sigCheck.Granted.Value,
                    });
                    await _ctx.SaveChangesAsync();

                    profitPlan.Status = sigCheck.Granted.Value;
                    profitPlan.StatusName = sigCheck.Granted.Name;
                    profitPlan.PathPdf = record.PathPdf;

                    _ctx.ProfitPlan.Update(profitPlan);
                    await _ctx.SaveChangesAsync();
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = request.IsApproval ? "Phê duyệt kế hoạch thành công!" : "Từ chối kế hoạch thành công!";

                    var item = _mapper.Map<SearchProfitPlanResponseData>(profitPlan);

                    item.Record = request.Record;

                    // sau khi phê duyệt hoặc từ chối
                    // chỉ còn view được --> tất cả các action khác đều bị vô hiệu

                    item.Viewable = true;
                    item.Editable = false;
                    item.Approvalable = false;
                    item.Declineable = false;
                    item.PathPdf = $"{request.HostFileView}{item.PathPdf}";
                    response.Data = item;


                    await transaction.CommitAsync();
                    if (!request.IsAuto)
                        await _dashboardRepository.SendOnApproval(new SendFormat
                        {
                            UnitId = profitPlan.UnitId,
                            UserId = request.UserId,
                            Year = profitPlan.Year,
                            ForSubject = record.IsSub ? "sub" : profitPlan.ProfitPlanType,
                            Level = sigCheck.Granted.Order,
                            PositionCode = sigCheck.Granted.PositionCode,
                            PlanType = GlobalEnums.Profit,
                            IsApproval = sigCheck.IsApproval,
                            RecordId = request.RawId,
                            Creator = profitPlan.Creator,
                            PositionName = sigCheck.Granted.PositionName
                        });
                }
                catch (Exception e)
                {
                    Log.Error(e, e.Message);
                    transaction.RollbackToSavepoint("Before");
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    response.Message = request.IsApproval
                        ? "Phê duyệt kế hoạch không thành công!"
                        : "Từ chối kế hoạch không thành công!";
                }

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                return new ProfitPlanApproveResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = request.IsApproval ? "Phê duyệt kế hoạch không thành công!" : "Từ chối kế hoạch không thành công!"
                };
            }
        }

        public async Task<SignatureCheckResponse> CheckPermissionApprove(ProfitPlanApproveRequest request, ProfitPlan record)
        {
            try
            {
                var profitPlan = record;
                if (profitPlan == null)
                    return new SignatureCheckResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                        Message = "Không tìm thấy dữ liệu yêu cầu!"
                    };
                // lấy ra mã trạng thái hiện tại
                var statusData = request.StatusAllowsSeen.FirstOrDefault(x => x.Value == profitPlan.Status);
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

                var next = new NextStatExtension(await _costStatusesRepository.GetAll(),
                    request.StatusAllowsSeen, GlobalEnums.Profit, GlobalEnums.Year, "Kế hoạch lợi nhuận");
                var oUnit = await _unitRepository.GetByIdAsync(profitPlan.UnitId);
                var allSpecialUnitFollowConfigs = await _costStatusesRepository.GetSpecialUnitFollowConfigs();
                var fSpecial = allSpecialUnitFollowConfigs.FirstOrDefault(g =>
                    g.UnitCode.Equals(oUnit.OfficesCode, StringComparison.CurrentCultureIgnoreCase));
                var oNext = next._getNext(profitPlan.Status, request.IsApproval, profitPlan.IsSub, profitPlan.ProfitPlanType, fSpecial?.UnitCode);
                if (!oNext.NextValid)
                    return new SignatureCheckResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.UnAuthor,
                        Message = "Bạn không có quyền thực hiện thao tác, vui lòng liên hệ với quản trị viên!"
                    };
                var oGroup = await _costStatusesRepository.GetUsedByGroup(oNext.Next.Id);

                return new SignatureCheckResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Success,
                    IsApproval = oNext.Next.IsApprove == 1,
                    IsSignature = oNext.Next.Sign,
                    Position = oGroup,
                    Granted = oNext.Next
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

        public async Task<ProfitPlan> GetByIdAsync(Guid id)
        {
            try
            {
                var q = await _ctx.ProfitPlan.FirstOrDefaultAsync(x => x.Id == id);
                return q;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                return null;
            }
        }

        public async Task<IList<ProfitPlanViewHistoryResponse>> ViewHistories(ProfitPlanViewHistoryRequest request)
        {
            try
            {
                // loại bỏ trạng thái tạo mới / chỉnh sửa
                var query = await _ctx.ProfitPlanLogs.Where(x => x.ProfitPlanId == request.RawId && x.FromStatus != (int)GlobalEnums.StatusDefaultEnum.Temporary)
                    .OrderByDescending(x => x.CreatedDate).ToListAsync();
                var data = query.Select(x => new ProfitPlanViewHistoryResponse
                {
                    UserName = x.CreatorName,
                    PositionName = x.PositionName,
                    Reason = x.Reason,
                    TimeChange = x.CreatedDate.ToString("dd/MM/yyyy HH:mm"),
                    Status = x.ToStatusName
                }).ToList();
                return data;

            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", request);
                return new List<ProfitPlanViewHistoryResponse>();
            }
        }

        public async Task<ProfitPlanApproveResponse> Delete(ProfitPlanApproveRequest request)
        {
            try
            {
                var response = new ProfitPlanApproveResponse();
                var record = await GetByIdAsync(request.RawId);

                if (record == null)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.Message = GlobalEnums.NoContentMessage;
                    return response;
                }

                if (record.Status != (int)GlobalEnums.StatusDefaultEnum.InActive)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NotOpenCode;
                    response.Message = "Không thể xóa kế hoạch lợi nhuận này!";
                    return response;
                }

                record.Status = (int)GlobalEnums.StatusDefaultEnum.Deleted;
                _ctx.Update(record);
                await _actionLogsRepository.AddLogAsync(new ActionLogs
                {
                    Action = "Delete",
                    Content = JsonConvert.SerializeObject(request),
                    CreatedDate = DateTime.Now,
                    FunctionUnique = Functions.ProfitPlanView,
                    UserId = request.UserId,
                    UserName = request.UserName
                });
                _ctx.SaveChanges();

                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Message = "Xóa kế hoạch doanh lợi nhuận thành công!";
                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", request);
                return new ProfitPlanApproveResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = GlobalEnums.ErrorMessage
                };
            }
        }


        public async Task<SignatureCheckResponse> _decline(ProfitPlanApproveRequest request, string positionAct)
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
        public async Task<bool> AutoDecline(int financeYear, ProfitPlanApproveRequest declineModel, string positionAct)
        {
            try
            {
                var listFinanceYear = await _ctx.ProfitPlan.Where(c => c.Year == financeYear && c.UnitId == declineModel.UnitId).ToListAsync();

                if (listFinanceYear.Count == 0)
                    return true;
                var listIds = listFinanceYear.Select(c => c.Id);
                var listRevenueLogs =
                    await _ctx.ProfitPlanLogs.Where(c => listIds.Contains(c.ProfitPlanId) && !string.IsNullOrEmpty(c.Reason)).ToListAsync();

                var checkDecline = await _decline(declineModel, positionAct);
                if (checkDecline.Code != (int)GlobalEnums.ResponseCodeEnum.Success)
                    return false;
                declineModel.IsAuto = true;
                foreach (var rv in listFinanceYear)
                    if (listRevenueLogs.All(c => c.ProfitPlanId != rv.Id)) //chỉ từ chối các BM không ở trạng thái duyệt
                        await Approval(declineModel, rv, checkDecline);

                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", financeYear);
                return false;
            }
        }
    }
}
