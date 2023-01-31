using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.Dashboard;
using GPLX.Core.Contracts.Revenue;
using GPLX.Core.Contracts.Statuses;
using GPLX.Core.Contracts.Unit;
using GPLX.Core.DTO.Request.Notify;
using GPLX.Core.DTO.Request.RevenuePlan;
using GPLX.Core.DTO.Response;
using GPLX.Core.DTO.Response.CostStatus;
using GPLX.Core.DTO.Response.RevenuePlan;
using GPLX.Core.Enum;
using GPLX.Core.Extensions;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using Functions = GPLX.Core.Contants.Functions;

namespace GPLX.Core.Data.Revenue
{
    public class RevenuePlanRepository : IRevenuePlanRepository
    {
        private readonly Context _ctx;
        private readonly ICostStatusesRepository _costStatusesRepository;
        private readonly IMapper _mapper;
        private readonly IActionLogsRepository _actionLogsRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IDashboardRepository _dashboardRepository;

        public RevenuePlanRepository(Context ctx, ICostStatusesRepository costStatusesRepository, IMapper mapper, IActionLogsRepository actionLogsRepository, IUnitRepository unitRepository, IDashboardRepository dashboardRepository)
        {
            _ctx = ctx;
            _costStatusesRepository = costStatusesRepository;
            _mapper = mapper;
            _actionLogsRepository = actionLogsRepository;
            _unitRepository = unitRepository;
            _dashboardRepository = dashboardRepository;
        }
        public async Task<IList<RevenuePlanContents>> ListRevenuePlanContents(string unitType)
        {
            try
            {
                var q = await _ctx.RevenuePlanContents.Where(c => c.ForSubject.ToLower().Equals(unitType.ToLower())).ToListAsync();
                return q;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                return null;
            }
        }

        public async Task<RevenuePlanCreateResponse> Create(RevenuePlanCreateRequest request)
        {
            var response = new RevenuePlanCreateResponse();
            try
            {
                var transaction = _ctx.Database.BeginTransaction();
                await transaction.CreateSavepointAsync("Before");
                try
                {
                    #region bảng chính
                    var primary = request.RevenuePlan;
                    primary.Id = Guid.NewGuid();
                    primary.Status = (int)GlobalEnums.StatusDefaultEnum.InActive;
                    primary.StatusName = GlobalEnums.DefaultStatusNames[(int)GlobalEnums.StatusDefaultEnum.InActive];
                    primary.IsSub = request.IsSub;
                    primary.Creator = request.Creator;
                    primary.CreatorName = request.CreatorName;
                    primary.CreatedDate = DateTime.Now;
                    primary.UnitId = request.UnitId;
                    await _ctx.RevenuePlan.AddAsync(primary);
                    await _ctx.SaveChangesAsync();
                    #endregion

                    #region bảng tổng hợp

                    //todo: check 

                    foreach (var rqAggregate in request.RevenuePlanAggregates)
                    {
                        rqAggregate.Id = Guid.NewGuid();
                        rqAggregate.RevenuePlanId = primary.Id;

                    }

                    await _ctx.RevenuePlanAggregate.AddRangeAsync(request.RevenuePlanAggregates);
                    await _ctx.SaveChangesAsync();

                    #endregion

                    #region Bảng khách hàng

                    foreach (var rqPlanCustomer in request.RevenuePlanCustomers)
                    {
                        rqPlanCustomer.Id = Guid.NewGuid();
                        rqPlanCustomer.RevenuePlanId = primary.Id;
                    }

                    await _ctx.RevenuePlanCustomerDetails.AddRangeAsync(request.RevenuePlanCustomers);
                    await _ctx.SaveChangesAsync();
                    #endregion

                    #region Bảng doanh thu

                    foreach (var rqPlanCustomer in request.RevenuePlanCash)
                    {
                        rqPlanCustomer.Id = Guid.NewGuid();
                        rqPlanCustomer.RevenuePlanId = primary.Id;
                    }

                    await _ctx.RevenuePlanCashDetails.AddRangeAsync(request.RevenuePlanCash);
                    await _ctx.SaveChangesAsync();
                    #endregion

                    await _dashboardRepository.SendCreateNotify(GlobalEnums.Revenue, request.UnitId, primary.IsSub ? "sub": primary.RevenuePlanType, primary.Year);

                    await transaction.CommitAsync();
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Tạo kế hoạch doanh thu khách hàng thành công!";
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

        public async Task<SearchRevenuePlanResponse> SearchAsync(SearchRevenuePlanRequest request, int offset, int limit, string unitType)
        {
            try
            {
                var next = new NextStatExtension(await _costStatusesRepository.GetAll(), request.StatusAllowsSeen,
                    GlobalEnums.Revenue, GlobalEnums.Year, "Kế hoạch doanh thu khách hàng");

                var response = new SearchRevenuePlanResponse();

                var joinQuery = _ctx.RevenuePlan.GroupJoin(_ctx.RevenuePlanAggregate, a => a.Id, b => b.RevenuePlanId,
                    (x, y) => new
                    {
                        RevenuePlan = x,
                        Aggregates = y
                    })
                    .SelectMany(x => x.Aggregates.DefaultIfEmpty(), (x, c) => new
                    {
                        x.RevenuePlan,
                        c.ExpectRevenue,
                        c.NumberCustomers,
                        c.ProportionCustomer,
                        c.RevenuePlanContentContent
                    });


                if (request.UserUnit != (int)GlobalEnums.StatusDefaultEnum.All)
                    joinQuery = joinQuery.Where(p => p.RevenuePlan.UnitId == request.UserUnit);
                if (request.Year != (int)GlobalEnums.StatusDefaultEnum.All)
                    joinQuery = joinQuery.Where(p => p.RevenuePlan.Year == request.Year);


                var subStatusFilter = request.StatusAllowsSeen.Filter(next.All, true, request.Status);
                var unitStatusFilter = request.StatusAllowsSeen.Filter(next.All, false, request.Status);



                // trạng thái mà user được phép thấy
                //joinQuery = joinQuery.Where(p =>
                //    request.StatusAllowsSeen.ActivatorFilter(next.All, true, request.Status, unitType).Contains(p.RevenuePlan.Status) && p.RevenuePlan.IsSub ||
                //    request.StatusAllowsSeen.ActivatorFilter(next.All, false, request.Status, unitType).Contains(p.RevenuePlan.Status) && !p.RevenuePlan.IsSub
                //);

                if (request.UserUnitsManages != null && request.UserUnitsManages.Count > 0)
                {
                    var unitIds = request.UserUnitsManages.Select(x => x.OfficeId);
                    joinQuery = joinQuery.Where(p => unitIds.Contains(p.RevenuePlan.UnitId));
                }

                if (!string.IsNullOrEmpty(request.Keywords))
                    joinQuery = joinQuery.Where(p => p.RevenuePlan.UnitName.Contains(request.Keywords));

                var dataSearch = await joinQuery.
                    OrderByDescending(x => x.RevenuePlan.CreatedDate).
                    ToListAsync();


                var unitIdList = dataSearch.Select(c => c.RevenuePlan.UnitId).ToList();
                var units = await _ctx.Units.Where(cc => unitIdList.Contains(cc.Id)).ToListAsync();

                var joinWithUnit = from da in dataSearch
                                   join u in units on da.RevenuePlan.UnitId equals u.Id
                                   select new
                                   {
                                       da.RevenuePlan,
                                       da.ExpectRevenue,
                                       da.RevenuePlanContentContent,
                                       da.NumberCustomers,
                                       da.ProportionCustomer,
                                       u.OfficesCode,
                                       u.OfficesSub
                                   };


                var groupData = joinWithUnit.GroupBy(x => new { x.RevenuePlan.Id }).Select(x => new
                {
                    x.First().RevenuePlan,
                    x.First().OfficesCode,
                    x.First().OfficesSub,

                }).ToList();


                var latest = new List<RevenuePlan>();

                // filter theo trạng thái được cấu hình
                foreach (var g in groupData)
                {
                    if (g.RevenuePlan.IsSub && subStatusFilter.Count > 0)
                    {
                        var subFil = subStatusFilter.FirstOrDefault(c => c.UnitType.Equals(GlobalEnums.ObjectSub, StringComparison.CurrentCultureIgnoreCase));
                        if (subFil?.Values.Any(c => c == g.RevenuePlan.Status) == true)
                            latest.Add(g.RevenuePlan);
                    }
                    else
                    {
                        if (unitStatusFilter.Count > 0)
                        {
                            foreach (var activatorResult in unitStatusFilter)
                            {
                                if (g.RevenuePlan.RevenuePlanType?.Equals(activatorResult.UnitType) == true &&
                                    activatorResult.Values.Contains(g.RevenuePlan.Status))
                                {
                                    latest.Add(g.RevenuePlan);
                                    break;
                                }

                                if (g.OfficesCode.Equals(activatorResult.UnitType) && activatorResult.Values.Contains(g.RevenuePlan.Status))
                                {
                                    latest.Add(g.RevenuePlan);
                                    break;
                                }
                            }
                        }
                    }
                }


                var data = new List<SearchRevenuePlanResponseData>();
                var allSpecialUnitFollowConfigs = await _costStatusesRepository.GetSpecialUnitFollowConfigs();
                var allUnits = await _unitRepository.GetAllAsync(string.Empty, 0, 1000);

                foreach (var x in latest)
                {
                    var oUnit = allUnits.FirstOrDefault(c => c.Id == x.UnitId);
                    var fSpecial = allSpecialUnitFollowConfigs.FirstOrDefault(g =>
                        g.UnitCode.Equals(oUnit?.OfficesCode, StringComparison.CurrentCultureIgnoreCase));


                    int minLevel = next._minLevel(x.IsSub, x.RevenuePlanType, fSpecial?.UnitCode);

                    var item = _mapper.Map<SearchRevenuePlanResponseData>(x);
                    item.Record = x.Id.ToString().StringAesEncryption(request.PageRequest);

                    item.Approvalable = next._visible(x.Status, NextAction.APPROVED, request.PermissionApprove, x.IsSub, x.RevenuePlanType, fSpecial?.UnitCode);
                    item.Declineable = next._visible(x.Status, NextAction.DECLINE, request.PermissionApprove, x.IsSub, x.RevenuePlanType, fSpecial?.UnitCode);
                    item.Editable = next._visible(x.Status, NextAction.EDIT, request.PermissionEdit, x.IsSub, x.RevenuePlanType,
                        fSpecial?.UnitCode);
                    item.Deleteable = next._visible(x.Status, NextAction.DELETE, request.PermissionDelete, x.IsSub, x.RevenuePlanType,
                        fSpecial?.UnitCode);
                    item.Viewable = true;
                    item.PathPdf = $"{request.HostFileView}{item.PathPdf}";

                    // đã duyệt - từ chối
                    // nếu phiếu vẫn có thể phê duyệt - từ chối -> hiển thị bên chờ duyệt


                    // ngoài y tế

                    if (x.RevenuePlanType.Equals(GlobalEnums.UnitOut,
                    StringComparison.CurrentCultureIgnoreCase))
                    {
                        var fTotal = dataSearch.FirstOrDefault(dx => x.Id == dx.RevenuePlan.Id
                                                                     && dx.RevenuePlanContentContent.StartsWith(
                                                                         "Tổng cộng",
                                                                         StringComparison.CurrentCultureIgnoreCase));

                        item.TotalNumberCustomer = fTotal?.NumberCustomers ?? 0;
                        item.TotalExpectRevenue = fTotal?.ExpectRevenue ?? 0;
                    }

                    else
                    {
                        var fTotal = dataSearch.FirstOrDefault(dx =>
                            x.Id == dx.RevenuePlan.Id &&
                            dx.RevenuePlanContentContent.StartsWith("Doanh thu và Khách hàng",
                                StringComparison.CurrentCultureIgnoreCase));
                        //Doanh thu và Khách hàng
                        item.TotalNumberCustomer = fTotal?.NumberCustomers ?? 0;
                        item.TotalExpectRevenue = fTotal?.ExpectRevenue ?? 0;
                    }


                    if (request.Status != (int)GlobalEnums.StatusDefaultEnum.All)
                    {
                        if (request.Status == (int)GlobalEnums.StatusDefaultEnum.Activator || request.Status == (int)GlobalEnums.StatusDefaultEnum.Active
                                                                                           || request.Status == (int)GlobalEnums.StatusDefaultEnum.Decline)
                        {
                            if (!item.Approvalable && !item.Declineable)
                                data.Add(item);
                        }

                        if (request.Status == (int)GlobalEnums.StatusDefaultEnum.InActive)
                            if (item.Approvalable || item.Declineable || minLevel == x.Status)
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
                return new SearchRevenuePlanResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                    Data = null,
                    Draw = request.Draw,
                    Message = "Không tìm thấy dữ liệu yêu cầu"
                };
            }
        }

        public async Task<RevenuePlanApproveResponse> Approval(RevenuePlanApproveRequest request, RevenuePlan record, SignatureCheckResponse sigCheck)
        {
            try
            {
                var response = new RevenuePlanApproveResponse();

                var revenuePlan = record;

                await using var transaction = _ctx.Database.BeginTransaction();
                try
                {
                    await transaction.CreateSavepointAsync("Before");
                    await _ctx.RevenuePlanLogs.AddAsync(new RevenuePlanLogs
                    {
                        Id = Guid.NewGuid(),
                        Creator = request.UserId,
                        CreatedDate = DateTime.Now,
                        FromStatus = revenuePlan.Status,
                        CreatorName = request.UserName,
                        Reason = request.Reason,
                        RevenuePlanId = revenuePlan.Id,
                        PositionName = sigCheck.Position.Name,
                        ToStatusName = sigCheck.Granted.Name,
                        PositionId = sigCheck.Position.Id,
                        ToStatus = sigCheck.Granted.Value,
                    });
                    await _ctx.SaveChangesAsync();

                    revenuePlan.Status = sigCheck.Granted.Value;
                    revenuePlan.StatusName = sigCheck.Granted.Name;
                    revenuePlan.PathPdf = record.PathPdf;

                    _ctx.RevenuePlan.Update(revenuePlan);
                    await _ctx.SaveChangesAsync();
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = request.IsApproval ? "Phê duyệt kế hoạch thành công!" : "Từ chối kế hoạch thành công!";

                    var item = _mapper.Map<SearchRevenuePlanResponseData>(revenuePlan);

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
                    await _dashboardRepository.SendOnApproval(new SendFormat
                    {
                        UnitId = revenuePlan.UnitId,
                        UserId = request.UserId,
                        Year = revenuePlan.Year,
                        ForSubject = record.IsSub ? "sub" : revenuePlan.RevenuePlanType,
                        Level = sigCheck.Granted.Order,
                        PositionCode = sigCheck.Granted.PositionCode,
                        PlanType = GlobalEnums.Revenue,
                        IsApproval = sigCheck.IsApproval,
                        RecordId = request.RawId,
                        Creator = revenuePlan.Creator,
                        PositionName = sigCheck.Granted.PositionName
                    });
                }
                catch (Exception e)
                {
                    Log.Error(e, e.Message);
                    transaction.RollbackToSavepoint("Before");
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    response.Message = "Phê duyệt kế hoạch không thành công!";
                }

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                return new RevenuePlanApproveResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = "Phê duyệt kế hoạch không thành công!"
                };
            }
        }

        public async Task<SignatureCheckResponse> CheckPermissionApprove(RevenuePlanApproveRequest request, RevenuePlan record)
        {
            try
            {
                var revenuePlan = record;
                if (revenuePlan == null)
                    return new SignatureCheckResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                        Message = "Không tìm thấy dữ liệu yêu cầu!"
                    };
                // lấy ra mã trạng thái hiện tại
                var statusData = request.StatusAllowsSeen.FirstOrDefault(x => x.Value == revenuePlan.Status);
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
                    request.StatusAllowsSeen, GlobalEnums.Revenue, GlobalEnums.Year, "Kế hoạch doanh thu khách hàng");

                var oUnit = await _unitRepository.GetByIdAsync(record.UnitId);
                var allSpecialUnitFollowConfigs = await _costStatusesRepository.GetSpecialUnitFollowConfigs();
                var fSpecial = allSpecialUnitFollowConfigs.FirstOrDefault(g =>
                    g.UnitCode.Equals(oUnit.OfficesCode, StringComparison.CurrentCultureIgnoreCase));
                var oNext = next._getNext(revenuePlan.Status, request.IsApproval, revenuePlan.IsSub, revenuePlan.RevenuePlanType, fSpecial?.UnitCode);
                if (!oNext.NextValid)
                    return new SignatureCheckResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.UnAuthor,
                        Message = "Bạn không có quyền thực hiện thao tác, vui lòng liên hệ với quản trị viên!"
                    };
                return new SignatureCheckResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Success,
                    IsSignature = oNext.Next.Sign,
                    IsApproval = oNext.Next.IsApprove == 1,
                    Position = await _costStatusesRepository.GetUsedByGroup(oNext.Next.Id),
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

        public async Task<RevenuePlan> GetByIdAsync(Guid id)
        {
            try
            {
                var q = await _ctx.RevenuePlan.FirstOrDefaultAsync(x => x.Id == id);
                return q;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                return null;
            }
        }

        public async Task<IList<RevenuePlanViewHistoryResponse>> ViewHistories(RevenuePlanViewHistoryRequest request)
        {
            try
            {
                // loại bỏ trạng thái tạo mới / chỉnh sửa
                var query = await _ctx.RevenuePlanLogs.Where(x => x.RevenuePlanId == request.RawId && x.FromStatus != (int)GlobalEnums.StatusDefaultEnum.Temporary)
                    .OrderByDescending(x => x.CreatedDate).ToListAsync();
                var data = query.Select(x => new RevenuePlanViewHistoryResponse()
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
                return new List<RevenuePlanViewHistoryResponse>();
            }
        }

        public async Task<RevenuePlanApproveResponse> Delete(RevenuePlanApproveRequest request)
        {
            try
            {
                var response = new RevenuePlanApproveResponse();
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
                    response.Message = "Không thể xóa kế hoạch doanh thu này!";
                    return response;
                }

                record.Status = (int)GlobalEnums.StatusDefaultEnum.Deleted;
                _ctx.Update(record);
                await _actionLogsRepository.AddLogAsync(new ActionLogs
                {
                    Action = "Delete",
                    Content = JsonConvert.SerializeObject(request),
                    CreatedDate = DateTime.Now,
                    FunctionUnique = Functions.RevenuePlanView,
                    UserId = request.UserId,
                    UserName = request.UserName
                });
                _ctx.SaveChanges();

                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Message = "Xóa kế hoạch doanh thu khách hàng thành công!";
                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", request);
                return new RevenuePlanApproveResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = GlobalEnums.ErrorMessage
                };
            }
        }

        public async Task<IList<RevenuePlanCashDetails>> GetLatestUnitRevenuePlanCashDetails(int year, int unitId)
        {
            try
            {
                var revenuePlan = await _ctx.RevenuePlan.OrderByDescending(c => c.CreatedDate).FirstOrDefaultAsync(c =>
                    c.UnitId == unitId && c.Year == year && c.Status != (int)GlobalEnums.StatusDefaultEnum.Deleted);
                if (revenuePlan == null)
                    return null;
                var revenuePlanCashDetails = await _ctx.RevenuePlanCashDetails.Where(x => x.RevenuePlanId == revenuePlan.Id)
                    .ToListAsync();
                return revenuePlanCashDetails;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }


    }
}
