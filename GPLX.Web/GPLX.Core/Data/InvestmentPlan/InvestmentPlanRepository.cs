using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.Dashboard;
using GPLX.Core.Contracts.Investment;
using GPLX.Core.Contracts.Statuses;
using GPLX.Core.Contracts.Unit;
using GPLX.Core.DTO.Request.InvestmentPlan;
using GPLX.Core.DTO.Request.Notify;
using GPLX.Core.DTO.Response;
using GPLX.Core.DTO.Response.InvestmentPlan;
using GPLX.Core.Enum;
using GPLX.Core.Extensions;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using Functions = GPLX.Core.Contants.Functions;

namespace GPLX.Core.Data.InvestmentPlan
{
    public class InvestmentPlanRepository : IInvestmentPlanRepository
    {
        private readonly Context _ctx;
        private readonly ICostStatusesRepository _costStatusesRepository;
        private readonly IMapper _mapper;
        private readonly IActionLogsRepository _actionLogsRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IDashboardRepository _dashboardRepository;

        public InvestmentPlanRepository(Context ctx, ICostStatusesRepository costStatusesRepository, IMapper mapper, IActionLogsRepository actionLogsRepository, IUnitRepository unitRepository, IDashboardRepository dashboardRepository)
        {
            _ctx = ctx;
            _costStatusesRepository = costStatusesRepository;
            _mapper = mapper;
            _actionLogsRepository = actionLogsRepository;
            _unitRepository = unitRepository;
            _dashboardRepository = dashboardRepository;
        }

        /// <summary>
        /// Danh sách phân loại danh mục
        /// </summary>
        /// <param name="subjectType"></param>
        /// <returns></returns>
        public async Task<IList<InvestmentPlanContents>> GetAllInvestmentPlanContentsForSubject(string subjectType)
        {
            try
            {
                var q = await _ctx.InvestmentPlanContents
                    .Where(x => x.ForSubject.ToLower().Equals(subjectType.ToLower()))
                    .ToListAsync();
                return q;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Log.Error(e, "exception {0}", e.Message);
                return new List<InvestmentPlanContents>();
            }
        }

        public async Task<InvestmentPlanCreateResponse> Create(InvestmentPlanCreateRequest request)
        {
            var response = new InvestmentPlanCreateResponse();
            try
            {

                // rule: kiểm tra đã tạo kế hoạch lợi nhuận chưa 
                //todo:  bỏ điều kiện theo y.c @ThoaĐT - 09/02/2022

                //var findRevenueInYear = await _ctx.ProfitPlan.Where(x =>
                //    x.Year == request.InvestmentPlan.Year && x.UnitId == request.UnitId &&
                //    x.Status != (int)GlobalEnums.StatusDefaultEnum.Deleted).OrderByDescending(x => x.CreatedDate).FirstOrDefaultAsync();

                //if (findRevenueInYear == null)
                //{
                //    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                //    response.Message = $"Chưa có kế hoạch lợi nhuận năm tài chính {request.InvestmentPlan.Year}!";
                //    return response;
                //}

                var transaction = _ctx.Database.BeginTransaction();
                await transaction.CreateSavepointAsync("Before");
                try
                {
                    #region bảng chính
                    var primary = request.InvestmentPlan;
                    primary.Id = Guid.NewGuid();
                    primary.Status = (int)GlobalEnums.StatusDefaultEnum.InActive;
                    primary.StatusName = GlobalEnums.DefaultStatusNames[(int)GlobalEnums.StatusDefaultEnum.InActive];
                    primary.IsSub = request.IsSub;
                    primary.Creator = request.Creator;
                    primary.CreatorName = request.CreatorName;
                    primary.CreatedDate = DateTime.Now;
                    primary.UnitId = request.UnitId;
                    await _ctx.InvestmentPlan.AddAsync(primary);
                    await _ctx.SaveChangesAsync();
                    #endregion

                    #region bảng tổng hợp

                    foreach (var requestInvestmentPlanAggregate in request.InvestmentPlanAggregates)
                    {
                        requestInvestmentPlanAggregate.Id = Guid.NewGuid();
                        requestInvestmentPlanAggregate.InvestmentPlanId = primary.Id;

                    }

                    await _ctx.InvestmentPlanAggregate.AddRangeAsync(request.InvestmentPlanAggregates);
                    await _ctx.SaveChangesAsync();

                    #endregion

                    #region Bảng chi tiết

                    if (request.InvestmentPlanDetails != null)
                    {
                        foreach (var requestInvestmentPlanDetail in request.InvestmentPlanDetails)
                        {
                            requestInvestmentPlanDetail.Id = Guid.NewGuid();
                            requestInvestmentPlanDetail.InvestmentPlanId = primary.Id;
                        }
                        await _ctx.InvestmentPlanDetails.AddRangeAsync(request.InvestmentPlanDetails);
                        await _ctx.SaveChangesAsync();
                    }


                    #endregion

                    await transaction.CommitAsync();

                    await _dashboardRepository.SendCreateNotify(GlobalEnums.Investment, request.UnitId, request.IsSub ? "sub" : request.UnitType, primary.Year);

                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Tạo kế hoạch đầu tư thành công!";
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

        public async Task<SearchInvestmentPlanResponse> SearchAsync(SearchInvestmentPlanRequest request, int offset, int limit, string unitType)
        {
            try
            {
                var next = new NextStatExtension(await _costStatusesRepository.GetAll(), request.StatusAllowsSeen,
                    GlobalEnums.Investment, GlobalEnums.Year, "Kế hoạch đầu tư");

                var response = new SearchInvestmentPlanResponse();
                var searchQuery = _ctx.InvestmentPlan.
                    Join(_ctx.Units, c => c.UnitId, y => y.Id, (x, y) => new
                    {
                        InvestmentPlan = x,
                        y.OfficesSub,
                        y.OfficesCode
                    })
                    .AsQueryable();


                if (request.UserUnit != (int)GlobalEnums.StatusDefaultEnum.All)
                    searchQuery = searchQuery.Where(p => p.InvestmentPlan.UnitId == request.UserUnit);

                if (request.Year != (int)GlobalEnums.StatusDefaultEnum.All)
                    searchQuery = searchQuery.Where(p => p.InvestmentPlan.Year == request.Year);

                // trạng thái mà user được phép thấy

                //searchQuery = searchQuery.Where(p =>
                //    request.StatusAllowsSeen.ActivatorFilter(next.All, true, request.Status, unitType).Contains(p.InvestmentPlan.Status) && p.InvestmentPlan.IsSub ||
                //    request.StatusAllowsSeen.ActivatorFilter(next.All, false, request.Status, unitType).Contains(p.InvestmentPlan.Status) && !p.InvestmentPlan.IsSub
                //);

                var subStatusFilter = request.StatusAllowsSeen.Filter(next.All, true, request.Status);
                var unitStatusFilter = request.StatusAllowsSeen.Filter(next.All, false, request.Status);

                if (request.UserUnitsManages != null && request.UserUnitsManages.Count > 0)
                {
                    var unitIds = request.UserUnitsManages.Select(x => x.OfficeId);
                    searchQuery = searchQuery.Where(p => unitIds.Contains(p.InvestmentPlan.UnitId));
                }

                if (!string.IsNullOrEmpty(request.Keywords))
                    searchQuery = searchQuery.Where(p => p.InvestmentPlan.UnitName.Contains(request.Keywords));

                var dataSearch = (await searchQuery.
                    OrderByDescending(x => x.InvestmentPlan.CreatedDate).
                    ToListAsync()).Select(c => new
                    {
                        c.InvestmentPlan,
                        c.OfficesCode,
                        OfficesSub = c.OfficesSub?.Equals("YT", StringComparison.CurrentCultureIgnoreCase) == true ? GlobalEnums.UnitIn : GlobalEnums.UnitOut
                    }).ToList();

                var latest = Extensions.Extensions.CreateList(dataSearch.FirstOrDefault());
                latest.Clear();
                // filter theo trạng thái được cấu hình
                foreach (var g in dataSearch)
                {
                    if (g.InvestmentPlan.IsSub && subStatusFilter.Count > 0)
                    {
                        var subFil = subStatusFilter.FirstOrDefault(c => c.UnitType.Equals(GlobalEnums.ObjectSub, StringComparison.CurrentCultureIgnoreCase));
                        if (subFil?.Values.Any(c => c == g.InvestmentPlan.Status) == true)
                            latest.Add(g);
                    }
                    else
                    {
                        if (unitStatusFilter.Count > 0)
                        {
                            foreach (var activatorResult in unitStatusFilter)
                            {
                                if (g.OfficesSub?.Equals(activatorResult.UnitType) == true &&
                                    activatorResult.Values.Contains(g.InvestmentPlan.Status))
                                {
                                    latest.Add(g);
                                    break;
                                }
                                if (g.OfficesCode.Equals(activatorResult.UnitType) && activatorResult.Values.Contains(g.InvestmentPlan.Status))
                                {
                                    latest.Add(g);
                                    break;
                                }
                            }
                        }
                    }
                }

                var data = new List<SearchInvestmentPlanResponseData>();
                var allSpecialUnitFollowConfigs = await _costStatusesRepository.GetSpecialUnitFollowConfigs();

                foreach (var x in latest)
                {

                    var item = _mapper.Map<SearchInvestmentPlanResponseData>(x.InvestmentPlan);
                    item.Record = x.InvestmentPlan.Id.ToString().StringAesEncryption(request.PageRequest);
                    var fSpecial = allSpecialUnitFollowConfigs.FirstOrDefault(g =>
                        g.UnitCode.Equals(x.OfficesCode, StringComparison.CurrentCultureIgnoreCase));
                    int minLevel = next._minLevel(x.InvestmentPlan.IsSub, x.OfficesSub, fSpecial?.UnitCode);

                    item.Approvalable = next._visible(x.InvestmentPlan.Status, NextAction.APPROVED, request.PermissionApprove, x.InvestmentPlan.IsSub, x.OfficesSub, fSpecial?.UnitCode);
                    item.Declineable = next._visible(x.InvestmentPlan.Status, NextAction.DECLINE, request.PermissionApprove, x.InvestmentPlan.IsSub, x.OfficesSub, fSpecial?.UnitCode);
                    item.Editable = next._visible(x.InvestmentPlan.Status, NextAction.EDIT, request.PermissionEdit, x.InvestmentPlan.IsSub, x.OfficesSub, fSpecial?.UnitCode);
                    item.Deleteable = next._visible(x.InvestmentPlan.Status, NextAction.DELETE, request.PermissionDelete, x.InvestmentPlan.IsSub, x.OfficesSub, fSpecial?.UnitCode);
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
                            if (item.Approvalable || item.Declineable || minLevel == x.InvestmentPlan.Status)
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
                return new SearchInvestmentPlanResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                    Data = null,
                    Draw = request.Draw,
                    Message = "Không tìm thấy dữ liệu yêu cầu"
                };
            }
        }

        public async Task<InvestmentPlanApproveResponse> Approval(InvestmentPlanApproveRequest request, Database.Models.InvestmentPlan record, SignatureCheckResponse sigCheck)
        {
            try
            {
                var response = new InvestmentPlanApproveResponse();

                var investmentPlan = record;
                await using var transaction = _ctx.Database.BeginTransaction();
                try
                {
                    await transaction.CreateSavepointAsync("Before");
                    await _ctx.InvestmentPlanLogs.AddAsync(new InvestmentPlanLogs
                    {
                        Id = Guid.NewGuid(),
                        Creator = request.UserId,
                        CreatedDate = DateTime.Now,
                        FromStatus = investmentPlan.Status,
                        CreatorName = request.UserName,
                        Reason = request.Reason,
                        InvestmentPlanId = investmentPlan.Id,
                        PositionName = sigCheck.Position.Name,
                        ToStatusName = sigCheck.Granted.Name,
                        PositionId = sigCheck.Position.Id,
                        ToStatus = sigCheck.Granted.Value,
                    });
                    await _ctx.SaveChangesAsync();

                    investmentPlan.Status = sigCheck.Granted.Value;
                    investmentPlan.StatusName = sigCheck.Granted.Name;
                    investmentPlan.PathPdf = record.PathPdf;

                    _ctx.InvestmentPlan.Update(investmentPlan);
                    await _ctx.SaveChangesAsync();
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = request.IsApproval ? "Phê duyệt kế hoạch thành công!" : "Từ chối kế hoạch thành công!";

                    var item = _mapper.Map<SearchInvestmentPlanResponseData>(investmentPlan);

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
                        UnitId = investmentPlan.UnitId,
                        UserId = request.UserId,
                        Year = investmentPlan.Year,
                        ForSubject = record.IsSub ? "sub" : request.UnitType,
                        Level = sigCheck.Granted.Order,
                        PositionCode = sigCheck.Granted.PositionCode,
                        PlanType = GlobalEnums.Investment,
                        IsApproval = sigCheck.IsApproval,
                        RecordId = request.RawId,
                        Creator = investmentPlan.Creator,
                        PositionName = sigCheck.Granted.PositionName
                    });
                }
                catch (Exception e)
                {
                    Log.Error(e, e.Message);
                    transaction.RollbackToSavepoint("Before");
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    response.Message = request.IsApproval ? "Phê duyệt kế hoạch không thành công!" : "Từ chối kế hoạch không thành công!";
                }

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, "error");
                return new InvestmentPlanApproveResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = request.IsApproval ? "Phê duyệt kế hoạch không thành công!" : "Từ chối kế hoạch không thành công!"
                };
            }
        }

        public async Task<SignatureCheckResponse> CheckPermissionApprove(InvestmentPlanApproveRequest request, Database.Models.InvestmentPlan record)
        {
            try
            {
                var investmentPlan = record;
                if (investmentPlan == null)
                    return new SignatureCheckResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                        Message = "Không tìm thấy dữ liệu yêu cầu!"
                    };
                // lấy ra mã trạng thái hiện tại
                var statusData = request.StatusAllowsSeen.FirstOrDefault(x => x.Value == investmentPlan.Status);
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
                    request.StatusAllowsSeen, GlobalEnums.Investment, GlobalEnums.Year, "Kế hoạch đầu tư");
                var allSpecialUnitFollowConfigs = await _costStatusesRepository.GetSpecialUnitFollowConfigs();
                var fSpecial = allSpecialUnitFollowConfigs.FirstOrDefault(g =>
                    g.UnitCode.Equals(oUnit.OfficesCode, StringComparison.CurrentCultureIgnoreCase));
                var oNext = next._getNext(investmentPlan.Status, request.IsApproval, investmentPlan.IsSub, rcUnit, fSpecial?.UnitCode);
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
                    IsSignature = oNext.Next.Sign,
                    IsApproval = oNext.Next.IsApprove == 1,
                    Granted = oNext.Next,
                    Position = oGroup
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

        public async Task<Database.Models.InvestmentPlan> GetByIdAsync(Guid id)
        {
            try
            {
                var q = await _ctx.InvestmentPlan.FirstOrDefaultAsync(x => x.Id == id);
                return q;
            }
            catch (Exception e)
            {
                Log.Error(e, "exception {0}", e.Message);
                return null;
            }
        }

        public async Task<InvestmentPlanApproveResponse> DeleteAsync(InvestmentPlanApproveRequest request)
        {
            InvestmentPlanApproveResponse response = new InvestmentPlanApproveResponse();
            try
            {
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
                    response.Message = "Không thể xóa kế hoạch đầu tư này!";
                    return response;
                }
                record.Status = (int)GlobalEnums.StatusDefaultEnum.Deleted;
                _ctx.Update(record);
                await _actionLogsRepository.AddLogAsync(new ActionLogs
                {
                    Action = "Delete",
                    Content = JsonConvert.SerializeObject(request),
                    CreatedDate = DateTime.Now,
                    FunctionUnique = Functions.InvestmentPlanView,
                    UserId = request.UserId,
                    UserName = request.UserName
                });
                _ctx.SaveChanges();

                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Message = "Xóa kế hoạch đầu tư thành công!";
                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", request);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.ErrorMessage;
            }

            return response;
        }

        public async Task<IList<InvestmentPlanViewHistoryResponse>> ViewHistories(InvestmentPlanViewHistoryRequest request)
        {
            try
            {
                // loại bỏ trạng thái tạo mới / chỉnh sửa
                var query = await _ctx.InvestmentPlanLogs.Where(x => x.InvestmentPlanId == request.RawId && x.FromStatus != (int)GlobalEnums.StatusDefaultEnum.Temporary)
                    .OrderByDescending(x => x.CreatedDate).ToListAsync();
                var data = query.Select(x => new InvestmentPlanViewHistoryResponse
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
                Log.Error(e, e.Message);

                return new List<InvestmentPlanViewHistoryResponse>();
            }
        }

        public async Task<List<InvestmentPlanAggregate>> GetInvestmentPlanAggregateByYearAsync(int year, int unit)
        {
            try
            {
                var investmentPlan = await _ctx.InvestmentPlan.OrderByDescending(c => c.CreatedDate).FirstOrDefaultAsync(x => x.Year == year && x.UnitId == unit && x.Status != (int)GlobalEnums.StatusDefaultEnum.Deleted);
                if (investmentPlan == null)
                    return null;
                return await _ctx.InvestmentPlanAggregate.Where(x => x.InvestmentPlanId == investmentPlan.Id).ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }
    }
}
