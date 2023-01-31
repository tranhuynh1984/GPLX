using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GPLX.Core.Contracts.Dashboard;
using GPLX.Core.Contracts.Notify;
using GPLX.Core.Contracts.Statuses;
using GPLX.Core.DTO.Request.Dashboard;
using GPLX.Core.DTO.Request.Notify;
using GPLX.Core.DTO.Response.Dashboard;
using GPLX.Core.Enum;
using GPLX.Core.Extensions;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GPLX.Core.Data.Dashboard
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly Context _ctx;
        private readonly ICostStatusesRepository _costStatusesRepository;
        private readonly INotifyRepository _notifyRepository;

        public DashboardRepository(Context context, ICostStatusesRepository costStatusesRepository, INotifyRepository notifyRepository)
        {
            _ctx = context;
            _costStatusesRepository = costStatusesRepository;
            _notifyRepository = notifyRepository;
        }
        public async Task<DashboardListResponse> SearchAsync(int offset, int limit, DashboardListRequest request)
        {
            try
            {
                var allStats = await _costStatusesRepository.GetAll();
                var rt = new DashboardListResponse();
                var data = new List<DashboardListResponseData>();
                var allUnits = await _ctx.Units.ToListAsync();

                #region Kế hoạch doanh thu KH

                if ((request.Type?.Count ?? 0) == 0 || request.Type?.Contains(GlobalEnums.Revenue) == true)
                {
                    var cp = Extensions.Extensions.CreateList(allStats.ToArray());

                    var subStatusFilter = Filter(allStats, cp, true, request.Status, GlobalEnums.Revenue);
                    var unitStatusFilter = Filter(allStats, cp, false, request.Status, GlobalEnums.Revenue);

                    var cQuery = _ctx.RevenuePlan.Where(cc =>
                       (request.Year == (int)GlobalEnums.StatusDefaultEnum.All || request.Year == cc.Year)
                    ).OrderByDescending(c => c.CreatedDate).AsQueryable();

                    if (request.UnitId?.Count > 0)
                        cQuery = cQuery.Where(c => request.UnitId.Contains(c.UnitId));
                    var revenueQuery = await cQuery.ToArrayAsync();
                    var unitIdList = revenueQuery.Select(c => c.UnitId).ToList();
                    var units = allUnits.Where(cc => unitIdList.Contains(cc.Id)).ToList();

                    var joinWithUnit = from da in revenueQuery
                                       join u in units on da.UnitId equals u.Id
                                       select new
                                       {
                                           RevenuePlan = da,
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



                    latest.ForEach(x =>
                    {
                        data.Add(new DashboardListResponseData
                        {
                            CreatedDate = x.CreatedDate,
                            CreatorName = x.CreatorName,
                            PlanName = "Kế hoạch DT-KH",
                            Record = x.Id.ToString().StringAesEncryption("revenue-plan"),
                            StatusName = x.StatusName,
                            UnitName = x.UnitName,
                            Year = x.Year,
                            PathPdf = $"{request.HostFileView}{x.PathPdf}",
                            PlanType = GlobalEnums.Revenue
                        });
                    });
                }

                #endregion

                #region Kế hoạch lợi nhuận

                if ((request.Type?.Count ?? 0) == 0 || request.Type?.Contains(GlobalEnums.Profit) == true)
                {
                    var cp = Extensions.Extensions.CreateList(allStats.ToArray());

                    var subStatusFilter = Filter(allStats, cp, true, request.Status, GlobalEnums.Profit);
                    var unitStatusFilter = Filter(allStats, cp, false, request.Status, GlobalEnums.Profit);


                    var cQuery = _ctx.ProfitPlan.Where(cc =>
                        (request.Year == (int)GlobalEnums.StatusDefaultEnum.All || request.Year == cc.Year)
                    ).OrderByDescending(c => c.CreatedDate).AsQueryable();

                    if (request.UnitId?.Count > 0)
                        cQuery = cQuery.Where(cc => request.UnitId.Contains(cc.UnitId));

                    var profitQuery = await cQuery.ToListAsync();

                    var unitIdList = profitQuery.Select(c => c.UnitId).ToList();
                    var units = allUnits.Where(cc => unitIdList.Contains(cc.Id)).ToList();

                    var joinWithUnit = from da in profitQuery
                                       join u in units on da.UnitId equals u.Id
                                       select new
                                       {
                                           ProfitPlan = da,
                                           u.OfficesCode,
                                           u.OfficesSub
                                       };


                    var groupData = joinWithUnit.GroupBy(x => new { x.ProfitPlan.Id }).Select(x => new
                    {
                        x.First().ProfitPlan,
                        x.First().OfficesCode,
                        x.First().OfficesSub,
                    }).ToList();


                    var latest = new List<ProfitPlan>();

                    // filter theo trạng thái được cấu hình
                    foreach (var g in groupData)
                    {
                        if (g.ProfitPlan.IsSub && subStatusFilter.Count > 0)
                        {
                            var subFil = subStatusFilter.FirstOrDefault(c => c.UnitType.Equals(GlobalEnums.ObjectSub, StringComparison.CurrentCultureIgnoreCase));
                            if (subFil?.Values.Any(c => c == g.ProfitPlan.Status) == true)
                                latest.Add(g.ProfitPlan);
                        }
                        else
                        {
                            if (unitStatusFilter.Count > 0)
                            {
                                foreach (var activatorResult in unitStatusFilter)
                                {
                                    if (g.ProfitPlan.ProfitPlanType?.Equals(activatorResult.UnitType) == true &&
                                        activatorResult.Values.Contains(g.ProfitPlan.Status))
                                    {
                                        latest.Add(g.ProfitPlan);
                                        break;
                                    }

                                    if (g.OfficesCode.Equals(activatorResult.UnitType) && activatorResult.Values.Contains(g.ProfitPlan.Status))
                                    {
                                        latest.Add(g.ProfitPlan);
                                        break;
                                    }
                                }
                            }
                        }
                    }


                    latest.ForEach(x =>
                    {
                        data.Add(new DashboardListResponseData
                        {
                            CreatedDate = x.CreatedDate,
                            CreatorName = x.CreatorName,
                            PlanName = "Kế hoạch lợi nhuận",
                            Record = x.Id.ToString().StringAesEncryption("profit-plan"),
                            StatusName = x.StatusName,
                            UnitName = x.UnitName,
                            Year = x.Year,
                            PathPdf = $"{request.HostFileView}{x.PathPdf}",
                            PlanType = GlobalEnums.Profit
                        });
                    });
                }

                #endregion

                #region Kế hoạch đầu tư
                if ((request.Type?.Count ?? 0) == 0 || request.Type?.Contains(GlobalEnums.Investment) == true)
                {
                    var cp = Extensions.Extensions.CreateList(allStats.ToArray());

                    var subStatusFilter = Filter(allStats, cp, true, request.Status, GlobalEnums.Investment);
                    var unitStatusFilter = Filter(allStats, cp, false, request.Status, GlobalEnums.Investment);

                    var cQuery = _ctx.InvestmentPlan.Where(cc =>
                       (request.Year == (int)GlobalEnums.StatusDefaultEnum.All || request.Year == cc.Year)
                    ).OrderByDescending(c => c.CreatedDate).AsQueryable();

                    if (request.UnitId?.Count > 0)
                        cQuery = cQuery.Where(cc => request.UnitId.Contains(cc.UnitId));

                    var investmentQuery = await cQuery.ToListAsync();

                    var unitIdList = investmentQuery.Select(c => c.UnitId).ToList();
                    var units = allUnits.Where(cc => unitIdList.Contains(cc.Id)).ToList();

                    var joinWithUnit = from da in investmentQuery
                                       join u in units on da.UnitId equals u.Id
                                       select new
                                       {
                                           InvestmentPlan = da,
                                           u.OfficesCode,
                                           u.OfficesSub
                                       };


                    var groupData = joinWithUnit.GroupBy(x => new { x.InvestmentPlan.Id }).Select(x => new
                    {
                        x.First().InvestmentPlan,
                        x.First().OfficesCode,
                        OfficesSub = x.First().OfficesSub?.Equals("YT", StringComparison.CurrentCultureIgnoreCase) == true ? GlobalEnums.UnitIn : GlobalEnums.UnitOut,
                    }).ToList();


                    var latest = new List<Database.Models.InvestmentPlan>();

                    // filter theo trạng thái được cấu hình
                    foreach (var g in groupData)
                    {
                        if (g.InvestmentPlan.IsSub && subStatusFilter.Count > 0)
                        {
                            var subFil = subStatusFilter.FirstOrDefault(c => c.UnitType.Equals(GlobalEnums.ObjectSub, StringComparison.CurrentCultureIgnoreCase));
                            if (subFil?.Values.Any(c => c == g.InvestmentPlan.Status) == true)
                                latest.Add(g.InvestmentPlan);
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
                                        latest.Add(g.InvestmentPlan);
                                        break;
                                    }

                                    if (g.OfficesCode.Equals(activatorResult.UnitType) && activatorResult.Values.Contains(g.InvestmentPlan.Status))
                                    {
                                        latest.Add(g.InvestmentPlan);
                                        break;
                                    }
                                }
                            }
                        }
                    }


                    latest.ForEach(x =>
                    {
                        data.Add(new DashboardListResponseData
                        {
                            CreatedDate = x.CreatedDate,
                            CreatorName = x.CreatorName,
                            PlanName = "Kế hoạch đầu tư",
                            Record = x.Id.ToString().StringAesEncryption("investment-plan"),
                            StatusName = x.StatusName,
                            UnitName = x.UnitName,
                            Year = x.Year,
                            PathPdf = $"{request.HostFileView}{x.PathPdf}",
                            PlanType = GlobalEnums.Investment
                        });
                    });
                }
                #endregion

                #region Kế hoạch dòng tiền
                if ((request.Type?.Count ?? 0) == 0 || request.Type?.Contains(GlobalEnums.CashFollow) == true)
                {
                    var cp = Extensions.Extensions.CreateList(allStats.ToArray());

                    var subStatusFilter = Filter(allStats, cp, true, request.Status, GlobalEnums.CashFollow);
                    var unitStatusFilter = Filter(allStats, cp, false, request.Status, GlobalEnums.CashFollow);
                    var cQuery = _ctx.CashFollow.Where(cc =>
                        (request.Year == (int)GlobalEnums.StatusDefaultEnum.All || request.Year == cc.Year)
                    ).OrderByDescending(c => c.CreatedDate).AsQueryable();
                    if (request.UnitId?.Count > 0)
                        cQuery = cQuery.Where(cc => request.UnitId.Contains(cc.UnitId));

                    var cashFollowQuery = await cQuery.ToListAsync();
                    var unitIdList = cashFollowQuery.Select(c => c.UnitId).ToList();
                    var units = allUnits.Where(cc => unitIdList.Contains(cc.Id)).ToList();

                    var joinWithUnit = from da in cashFollowQuery
                                       join u in units on da.UnitId equals u.Id
                                       select new
                                       {
                                           CashFollow = da,
                                           u.OfficesCode,
                                           u.OfficesSub
                                       };


                    var groupData = joinWithUnit.GroupBy(x => new { x.CashFollow.Id })
                        .Select(x => new
                        {
                            x.First().CashFollow,
                            x.First().OfficesCode,
                            OfficesSub = x.First().OfficesSub?.Equals("YT", StringComparison.CurrentCultureIgnoreCase) == true ? GlobalEnums.UnitIn : GlobalEnums.UnitOut,
                        }).ToList();


                    var latest = new List<Database.Models.CashFollow>();

                    // filter theo trạng thái được cấu hình
                    foreach (var g in groupData)
                    {
                        if (g.CashFollow.IsSub && subStatusFilter.Count > 0)
                        {
                            var subFil = subStatusFilter.FirstOrDefault(c => c.UnitType.Equals(GlobalEnums.ObjectSub, StringComparison.CurrentCultureIgnoreCase));
                            if (subFil?.Values.Any(c => c == g.CashFollow.Status) == true)
                                latest.Add(g.CashFollow);
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
                                        latest.Add(g.CashFollow);
                                        break;
                                    }

                                    if (g.OfficesCode.Equals(activatorResult.UnitType) && activatorResult.Values.Contains(g.CashFollow.Status))
                                    {
                                        latest.Add(g.CashFollow);
                                        break;
                                    }
                                }
                            }
                        }
                    }


                    latest.ForEach(x =>
                    {
                        data.Add(new DashboardListResponseData
                        {
                            CreatedDate = x.CreatedDate,
                            CreatorName = x.CreatorName,
                            PlanName = "Kế hoạch dòng tiền",
                            Record = x.Id.ToString().StringAesEncryption("cash-follow"),
                            StatusName = x.StatusName,
                            UnitName = x.UnitName,
                            Year = x.Year,
                            PathPdf = $"{request.HostFileView}{x.PathPdf}",
                            PlanType = GlobalEnums.CashFollow
                        });
                    });
                }
                #endregion

                rt.Data = data.Skip(offset).Take(limit).ToList();
                rt.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                rt.RecordsFiltered = data.Count;
                rt.RecordsTotal = data.Count;
                return rt;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return new DashboardListResponse
                {
                    Message = "Không tìm thấy dữ liệu yêu cầu!",
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Draw = request.Draw
                };
            }
        }

        public static List<ActivatorResult> Filter(IEnumerable<CostStatuses> value, IList<CostStatuses> allStatus, bool sub, int? statusFilter, string type)
        {
            var valueList = value?.Where(cc => cc.Type.Equals(type, StringComparison.CurrentCultureIgnoreCase)).ToList();
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
        public async Task<DashboardExportResponse> Export(List<DashboardExportRequest> rq, List<FileNPlanType> excelPaths, List<FileNPlanType> pdfPaths, List<int> listUnitIds, string localPath)
        {
            var rt = new DashboardExportResponse();
            try
            {
                foreach (var dashboardExportRequest in rq)
                {
                    Guid g = Guid.Empty;
                    switch (dashboardExportRequest.Type)
                    {
                        case "revenue":
                            if (Guid.TryParse(dashboardExportRequest.Record.StringAesDecryption("revenue-plan"), out g))
                            {
                                var revenuePlan = await _ctx.RevenuePlan.FirstOrDefaultAsync(cc => cc.Id == g);
                                if (revenuePlan != null)
                                {
                                    excelPaths.Add(new FileNPlanType { Type = GlobalEnums.Revenue, FilePath = $"{localPath}/{revenuePlan.PathExcel}" });
                                    pdfPaths.Add(new FileNPlanType { Type = GlobalEnums.Revenue, FilePath = $"{localPath}/{revenuePlan.PathPdf}" });
                                    listUnitIds.Add(revenuePlan.UnitId);
                                }
                            }
                           
                            break;
                        case "profit":
                            if (Guid.TryParse(dashboardExportRequest.Record.StringAesDecryption("profit-plan"), out g))
                            {
                                var profitPlan =
                                    await _ctx.ProfitPlan.FirstOrDefaultAsync(cc => cc.Id == g);
                                if (profitPlan != null)
                                {
                                    excelPaths.Add(new FileNPlanType
                                        {Type = GlobalEnums.Profit, FilePath = $"{localPath}/{profitPlan.PathExcel}"});
                                    pdfPaths.Add(new FileNPlanType
                                        {Type = GlobalEnums.Profit, FilePath = $"{localPath}/{profitPlan.PathPdf}"});

                                    listUnitIds.Add(profitPlan.UnitId);
                                }
                            }
                            break;
                        case "investment":
                            if (Guid.TryParse(dashboardExportRequest.Record.StringAesDecryption("investment-plan"), out g))
                            {
                                var investmentPlan =
                                    await _ctx.InvestmentPlan.FirstOrDefaultAsync(cc => cc.Id == g);
                                if (investmentPlan != null)
                                {
                                    excelPaths.Add(new FileNPlanType
                                    {
                                        Type = GlobalEnums.Investment,
                                        FilePath = $"{localPath}/{investmentPlan.PathExcel}"
                                    });
                                    pdfPaths.Add(new FileNPlanType
                                    {
                                        Type = GlobalEnums.Investment,
                                        FilePath = $"{localPath}/{investmentPlan.PathPdf}"
                                    });

                                    listUnitIds.Add(investmentPlan.UnitId);
                                }
                            }

                            break;
                        case "cashfollow":
                            if (Guid.TryParse(dashboardExportRequest.Record.StringAesDecryption("cash-follow"), out g))
                            {
                                var cashFollow =
                                    await _ctx.CashFollow.FirstOrDefaultAsync(cc => cc.Id == g);
                                if (cashFollow != null)
                                {
                                    excelPaths.Add(new FileNPlanType
                                    {
                                        Type = GlobalEnums.CashFollow, FilePath = $"{localPath}/{cashFollow.PathExcel}"
                                    });
                                    pdfPaths.Add(new FileNPlanType
                                    {
                                        Type = GlobalEnums.CashFollow, FilePath = $"{localPath}/{cashFollow.PathPdf}"
                                    });

                                    listUnitIds.Add(cashFollow.UnitId);
                                }
                            }
                            break;
                        case "estimate":
                            if (Guid.TryParse(dashboardExportRequest.Record.StringAesDecryption("cost-element"), out g))
                            {
                                var costEstimate =
                                    await _ctx.CostEstimates.FirstOrDefaultAsync(cc => cc.Id == g);
                                if (costEstimate != null)
                                {
                                    excelPaths.Add(new FileNPlanType
                                    {
                                        Type = GlobalEnums.CashFollow,
                                        FilePath = $"{localPath}/{costEstimate.PathExcel}"
                                    });
                                    pdfPaths.Add(new FileNPlanType
                                    {
                                        Type = GlobalEnums.CashFollow,
                                        FilePath = $"{localPath}/{costEstimate.PathPdf}"
                                    });

                                    listUnitIds.Add(costEstimate.UnitId);
                                }
                            }
                            break;
                    }
                }

                if (excelPaths.Count != rq.Count)
                {
                    rt.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    rt.Message = "Không tìm thấy dữ liệu yêu cầu!";
                }

                rt.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                return rt;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                rt.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                rt.Message = GlobalEnums.ErrorMessage;
            }

            return rt;
        }

        /// <summary>
        /// Chèn nội dung vào bảng bắn telegram
        /// </summary>
        /// <param name="planType">Loại kế hoạch</param>
        /// <param name="unitId"></param>
        /// <param name="forSubject"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<bool> SendCreateNotify(string planType, int unitId, string forSubject, int year)
        {
            try
            {
                string template =
                    "==== Tin nhắn từ Hệ thống MIS  ===\nXin chào {0}\nBạn có Kế hoạch {1} năm {2} cần phê duyệt.\nĐề nghị bạn click vào Link sau để phê duyệt kịp thời:\n{3}\nTrân trọng!";
                string planName = string.Empty;
                switch (planType)
                {
                    case "revenue":
                        planName = "DT & KH";
                        break;
                    case "profit":
                        planName = "Lợi nhuận";
                        break;
                    case "investment":
                        planName = "Đầu tư";
                        break;
                    case "cashfollow":
                        planName = "Dòng tiền";
                        break;
                }
                var nextUser = await _getNextPos(planType, forSubject, unitId);
                if (nextUser != null)
                {
                    await _notifyRepository.AddAsync(new Database.Models.Notify
                    {
                        Content = string.Format(template, nextUser.UserName, planName, year, "https://mis.medcom.vn/"),
                        CreatedDate = DateTime.Now,
                        Receiver = nextUser.UserCode,
                        Status = 0
                    });
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return false;
            }
        }

        public async Task<bool> SendOnApproval(SendFormat sendOnApproval)
        {
            try
            {
                if (sendOnApproval.IsApproval)
                {
                    string planName = string.Empty;
                    switch (sendOnApproval.PlanType)
                    {
                        case "revenue":
                            planName = "DT & KH";
                            break;
                        case "profit":
                            planName = "Lợi nhuận";
                            break;
                        case "investment":
                            planName = "Đầu tư";
                            break;
                        case "cashfollow":
                            planName = "Dòng tiền";
                            break;
                    }

                    // giám đốc đơn vị
                    // cả 4 KH phải cùng đc duyệt -> thì mới send
                    if (sendOnApproval.PositionCode.Equals("UnitManager", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var checkAllPlanApproval = await _checkApprovalAll(sendOnApproval.Year, sendOnApproval.UnitId,
                            sendOnApproval.UserId, sendOnApproval.ForSubject);
                        if (checkAllPlanApproval)
                        {
                            string template =
                                "==== Tin nhắn từ Hệ thống MIS  ===\nXin chào {0}\nBạn có Kế hoạch tài chính năm {1} của đơn vị {2} cần phê duyệt.\nĐề nghị bạn click vào Link sau để phê duyệt kịp thời:\n{3}\nTrân trọng!";
                            var nextUser = await _getNextPos(sendOnApproval.PlanType, sendOnApproval.ForSubject, sendOnApproval.UnitId,
                                sendOnApproval.Level);

                            if (nextUser != null)
                            {
                                var oUnit = await _ctx.Units.FirstOrDefaultAsync(cc => cc.Id == sendOnApproval.UnitId);
                                await _notifyRepository.AddAsync(new Database.Models.Notify
                                {
                                    Content = string.Format(template, nextUser.UserName, sendOnApproval.Year, oUnit?.OfficesName, "https://mis.medcom.vn/"),
                                    CreatedDate = DateTime.Now,
                                    Receiver = nextUser.UserCode,
                                    Status = 0
                                });
                            }
                        }
                    }
                    else
                    {

                        string template =
                            "==== Tin nhắn từ Hệ thống MIS  ===\nXin chào{0}\nBạn có Kế hoạch {1} năm {2} của đơn vị {3} cần phê duyệt. Đề nghị bạn click vào Link sau để phê duyệt kịp thời:\n{4}\nTrân trọng!";
                        var nextUser = await _getNextPos(sendOnApproval.PlanType, sendOnApproval.ForSubject, sendOnApproval.UnitId,
                            sendOnApproval.Level);
                        if (nextUser != null)
                        {
                            var oUnit = await _ctx.Units.FirstOrDefaultAsync(cc => cc.Id == sendOnApproval.UnitId);
                            await _notifyRepository.AddAsync(new Database.Models.Notify
                            {
                                Content = string.Format(template, nextUser.UserName, planName, sendOnApproval.Year, oUnit.OfficesName, "https://mis.medcom.vn/"),
                                CreatedDate = DateTime.Now,
                                Receiver = nextUser.UserCode,
                                Status = 0
                            });
                        }
                    }

                    // gửi thông báo duyệt cho user khác

                    await SendAct(sendOnApproval);
                }
                else
                    await SendAct(sendOnApproval);

                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return false;
            }
        }

        public async Task<bool> SendAct(SendFormat sendOn)
        {
            try
            {
                string planName = string.Empty;
                var listReceiverUserIds = new List<int>();
                switch (sendOn.PlanType)
                {
                    case "revenue":
                        planName = "DT & KH";
                        var logRevenues = await _ctx.RevenuePlanLogs.Where(c => c.RevenuePlanId
                            == sendOn.RecordId && c.Creator != sendOn.UserId).ToListAsync();
                        listReceiverUserIds = logRevenues.Select(c => c.Creator).ToList();
                        break;
                    case "profit":
                        planName = "Lợi nhuận";
                        var logProfits = await _ctx.ProfitPlanLogs.Where(c => c.ProfitPlanId
                            == sendOn.RecordId && c.Creator != sendOn.UserId).ToListAsync();
                        listReceiverUserIds = logProfits.Select(c => c.Creator).ToList();
                        break;
                    case "investment":
                        planName = "Đầu tư";
                        var logInvestments = await _ctx.InvestmentPlanLogs.Where(c => c.InvestmentPlanId
                            == sendOn.RecordId && c.Creator != sendOn.UserId).ToListAsync();
                        listReceiverUserIds = logInvestments.Select(c => c.Creator).ToList();
                        break;
                    case "cashfollow":
                        planName = "Dòng tiền";
                        var logCashFollows = await _ctx.CashFollowLog.Where(c => c.CashFollowId
                            == sendOn.RecordId && c.Creator != sendOn.UserId).ToListAsync();
                        listReceiverUserIds = logCashFollows.Select(c => c.Creator).ToList();
                        break;
                }

                var template = "==== Tin nhắn từ Hệ thống MIS  ===\nXin chào {0}\nKế hoạch {1} năm {2} đã bị từ chối bởi {3}\nĐề nghị bạn click vào Link sau để xem lại thông tin kế hoạch\nhttps://mis.medcom.vn/\nTrân trọng!";

                if (sendOn.IsApproval)
                {
                    template = "==== Tin nhắn từ Hệ thống MIS  ===\nXin chào {0}\nKế hoạch {1} năm {2} đã được phê duyệt bởi {3}\nĐề nghị bạn click vào Link sau để xem lại thông tin kế hoạch\nhttps://mis.medcom.vn/\nTrân trọng!";
                }


                // add thêm người tạo
                listReceiverUserIds.Add(sendOn.Creator);

                listReceiverUserIds = listReceiverUserIds.Distinct().ToList();
                var listUser = await _ctx.Users.Where(cc => listReceiverUserIds.Contains(cc.Id)).ToListAsync();

                foreach (var us in listUser)
                {
                    await _notifyRepository.AddAsync(new Database.Models.Notify
                    {
                        Content = string.Format(template, us.UserName, planName, sendOn.Year, sendOn.PositionName, "https://mis.medcom.vn/"),
                        CreatedDate = DateTime.Now,
                        Receiver = us.UserCode,
                        Status = 0
                    });
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return false;
            }
        }

        async Task<Users> _getNextPos(string planType, string forSubject, int unit)
        {
            try
            {
                forSubject = await _getSpecial(forSubject, unit);
                var allStats = await _costStatusesRepository.GetAll();
                var statusForPlanAndSubject = allStats.Where(c => c.Type.Equals(planType, StringComparison.CurrentCultureIgnoreCase)
                                                                  && c.StatusForSubject.Equals(forSubject, StringComparison.CurrentCultureIgnoreCase)).ToList();

                var minLevel = statusForPlanAndSubject.Min(cc => cc.Order);
                var nextLevel =
                    statusForPlanAndSubject.FirstOrDefault(cc => cc.Order == minLevel + 1 && cc.IsApprove == 1);
                if (nextLevel != null)
                {
                    var fetchGroup = await _ctx.CostStatusesGroups.FirstOrDefaultAsync(cc => cc.StatusesId == nextLevel.Id && !string.IsNullOrEmpty(cc.Type));
                    if (fetchGroup != null)
                        return await _getUserWithPositionOfUnit(fetchGroup.GroupCode, unit);
                }

                return null;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }

        async Task<Users> _getNextPos(string planType, string forSubject, int unit, int atLevel)
        {
            try
            {
                forSubject = await _getSpecial(forSubject, unit);
                var allStats = await _costStatusesRepository.GetAll();
                var statusForPlanAndSubject = allStats.Where(c => c.Type.Equals(planType, StringComparison.CurrentCultureIgnoreCase)
                                                                  && c.StatusForSubject.Equals(forSubject, StringComparison.CurrentCultureIgnoreCase)).ToList();

                var nextLevel =
                    statusForPlanAndSubject.FirstOrDefault(cc => cc.Order == atLevel + 1 && cc.IsApprove == 1);
                if (nextLevel != null)
                {
                    var fetchGroup = await _ctx.CostStatusesGroups.FirstOrDefaultAsync(cc => cc.StatusesId == nextLevel.Id && !string.IsNullOrEmpty(cc.Type));
                    if (fetchGroup != null)
                        return await _getUserWithPositionOfUnit(fetchGroup.GroupCode, unit);
                }

                return null;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }

        async Task<Users> _getUserWithPositionOfUnit(string positionCode, int unit)
        {
            try
            {
                var cQuery = from u in _ctx.Users
                             join mg in _ctx.UserGroups on u.Id equals mg.UserId
                             join g in _ctx.Groups on mg.GroupId equals g.Id
                             where g.GroupCode.Equals(positionCode) && (u.UnitId == unit || g.Order > 5)
                             select new
                             {
                                 Users = u,
                                 Group = g
                             };

                //&& u.UnitId == unit
                var data = await cQuery.ToListAsync();

                // trường hợp đơn vị k có user tại ví trị
                // tìm trong bảng kiêm nhiệm
                if (data == null || data.Count == 0)
                {
                    var concurrentlyUser = await _ctx.Users
                        .Join(_ctx.UserConcurrently, cc => cc.Id, mc => mc.UserId,
                            (v, c) => new { Users = v, Concurrently = c }).Where(cc =>
                              cc.Concurrently.GroupCode.Equals(positionCode) && cc.Concurrently.UnitId == unit)
                        .ToListAsync();
                    if (concurrentlyUser != null && concurrentlyUser.Count > 0)
                        return concurrentlyUser.First().Users;
                    return null;
                }

                var positionLevel = data.First().Group.Order;

                //todo: hard-code
                // = 5 là thứ tự của Giám đốc đơn vị
                // -> chuyển sang lấy vị trí theo OfficeCode
                if (positionLevel > 5)
                {
                    var userManagesOfOffice = await _ctx.UserUnitsManages.Where(c => c.OfficeId == unit).ToListAsync();
                    if (userManagesOfOffice?.Count > 0)
                    {
                        var rtUser = data.FirstOrDefault(c => userManagesOfOffice.Any(mm => mm.UserId == c.Users.Id));
                        return rtUser?.Users;
                    }
                    return data.First().Users;
                }
                return data.First().Users;

            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }

        async Task<string> _getSpecial(string forSubject, int unit)
        {
            try
            {
                var unitSpecial = await _ctx.SpecialUnitFollowConfigs.FirstOrDefaultAsync(cc => cc.UnitId == unit);
                if (unitSpecial != null)
                    return unitSpecial.UnitCode;
                return forSubject;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return forSubject;
            }
        }

        async Task<bool> _checkApprovalAll(int year, int unit, int userId, string forSubject)
        {
            try
            {
                // tất cả trạng thái phê duyệt của giám đốc đơn vị
                var allStatusApprovedOfUnitManager = await _ctx.CostStatuses.Join(_ctx.CostStatusesGroups, cc => cc.Id,
                    mm => mm.StatusesId, (x, y) => new
                    {
                        Statuses = x,
                        Group = y.GroupCode,
                        y.Type
                    }).Where(c => c.Group.Equals("UnitManager")
                                  && c.Statuses.StatusForSubject.Equals(forSubject)
                                  && !string.IsNullOrEmpty(c.Type) && c.Statuses.IsApprove == 1).
                    Select(c => c.Statuses).ToListAsync();
                var found = 4;

                #region KH doanh thu

                var latestRevenuePlan = await _ctx.RevenuePlan.OrderByDescending(c => c.CreatedDate).FirstOrDefaultAsync(c => c.Year == year && c.UnitId == unit);
                if (latestRevenuePlan != null)
                {
                    var allValues = allStatusApprovedOfUnitManager.
                        Where(c => c.Type.Equals(GlobalEnums.Revenue, StringComparison.CurrentCultureIgnoreCase)).
                        Select(c => c.Value).ToList();

                    var latestRevenuePlanLogs = await _ctx.RevenuePlanLogs.FirstOrDefaultAsync(cc =>
                        cc.RevenuePlanId == latestRevenuePlan.Id && cc.Creator == userId && allValues.Contains(cc.ToStatus));
                    if (latestRevenuePlanLogs != null)
                        found--;
                }

                #endregion

                #region KH Lợi nhuận

                var latestProfitPlan = await _ctx.ProfitPlan.OrderByDescending(c => c.CreatedDate).FirstOrDefaultAsync(c => c.Year == year && c.UnitId == unit);
                if (latestProfitPlan != null)
                {
                    var allValues = allStatusApprovedOfUnitManager.
                        Where(c => c.Type.Equals(GlobalEnums.Profit, StringComparison.CurrentCultureIgnoreCase)).
                        Select(c => c.Value).ToList();

                    var latestProfitPlanLogs = await _ctx.ProfitPlanLogs.FirstOrDefaultAsync(cc =>
                        cc.ProfitPlanId == latestProfitPlan.Id && cc.Creator == userId && allValues.Contains(cc.ToStatus));
                    if (latestProfitPlanLogs != null)
                        found--;
                }

                #endregion

                #region KH Đầu tư
                var latestInvestmentPlan = await _ctx.InvestmentPlan.OrderByDescending(c => c.CreatedDate)
                    .FirstOrDefaultAsync(c => c.Year == year && c.UnitId == unit);
                if (latestInvestmentPlan != null)
                {
                    var allValues = allStatusApprovedOfUnitManager.
                        Where(c => c.Type.Equals(GlobalEnums.Investment, StringComparison.CurrentCultureIgnoreCase)).
                        Select(c => c.Value).ToList();

                    var latestRevenuePlanLogs = await _ctx.InvestmentPlanLogs.FirstOrDefaultAsync(cc =>
                        cc.InvestmentPlanId == latestInvestmentPlan.Id && cc.Creator == userId && allValues.Contains(cc.ToStatus));
                    if (latestRevenuePlanLogs != null)
                        found--;
                }
                #endregion

                #region KH Dòng tiền
                var latestCashFollowPlan = await _ctx.CashFollow.OrderByDescending(c => c.CreatedDate)
                    .FirstOrDefaultAsync(c => c.Year == year && c.UnitId == unit);
                if (latestCashFollowPlan != null)
                {
                    var allValues = allStatusApprovedOfUnitManager.
                        Where(c => c.Type.Equals(GlobalEnums.CashFollow, StringComparison.CurrentCultureIgnoreCase)).
                        Select(c => c.Value).ToList();

                    var latestRevenuePlanLogs = await _ctx.CashFollowLog.FirstOrDefaultAsync(cc =>
                        cc.CashFollowId == latestCashFollowPlan.Id && cc.Creator == userId && allValues.Contains(cc.ToStatus));
                    if (latestRevenuePlanLogs != null)
                        found--;
                }
                #endregion

                return found == 0;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return false;
            }
        }
    }
}
