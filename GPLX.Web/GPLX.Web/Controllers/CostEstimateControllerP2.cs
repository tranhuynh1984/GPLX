using GPLX.Core.Extensions;
using GPLX.Infrastructure.Constants;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GPLX.Core.Contants;
using GPLX.Core.DTO.Request.CostEstimate;
using GPLX.Core.DTO.Request.Dashboard;
using GPLX.Core.DTO.Response.CostEstimate;
using GPLX.Core.DTO.Response.Dashboard;
using GPLX.Infrastructure.Extensions;
using GPLX.Core.Enum;
using GPLX.Database.Models;
using GPLX.Web.Filters;
using GPLX.Web.Models;
using GPLX.Web.Models.Dashboard;
using GPLX.Web.Process;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;
using Functions = GPLX.Core.Contants.Functions;

namespace GPLX.Web.Controllers
{
    public partial class CostEstimateController : BaseController
    {
        [AuthorizeUser(Module = Functions.CostEstimateCreate, Permission = PermissionConstant.VIEW)]
        public async Task<IActionResult> Overview(string record, string type = default, bool partial = default)
        {
            ViewBag.Partial = partial;
            Database.Models.CostEstimate estimate = null;
            var wInYear = DateTime.Now.Year.WeekInYear();
            int cWeek = wInYear.Where(c => c.weekStart >= DateTime.Now).Min(c => c.weekNum);
            ViewBag.CWeek = cWeek;
            try
            {
                var model = new CostEstimateCreateModel
                {
                    Record = record,
                    EnableApprove = true,
                    EnableCreate = true,
                    ViewMode = string.Empty
                };

                var sessionUser = GetUsersSessionModel();
                if (!string.IsNullOrEmpty(record))
                {
                    if (Guid.TryParse(record.StringAesDecryption(CostElementConst.PublicKey), out _))
                    {
                        var dataView = await _costEstimateRepository.LoadCostEstimateItemsData(new CostEstimateViewRequest
                        {
                            Status = (int)GlobalEnums.StatusDefaultEnum.All,
                            PageRequest = CostElementConst.PublicKey,
                            Record = record,
                            UserName = sessionUser.UserName,
                            UnitName = sessionUser.Unit.UnitName,
                            DepartmentName = sessionUser.Department.DepartmentName,
                            UnitId = GetUserUnit().UnitId,
                            UserId = GetUserId(),
                            DepartmentId = sessionUser.Department.DepartmentId,
                            Positions = sessionUser.Positions,
                        });
                        if (dataView.Code == (int)GlobalEnums.ResponseCodeEnum.Success)
                        {
                            model.DataView = dataView;
                            model.RequestType = dataView.RequestType;
                            model.ReportForWeek = dataView.CostEstimate.ReportForWeek;
                            estimate = dataView.CostEstimate;

                            ViewBag.Older = dataView;

                            var permEdit = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                                Functions.CostEstimateCreate, PermissionConstant.EDIT);
                            ViewBag.PermissionEdit =
                                dataView.CostEstimate.Status == (int)GlobalEnums.StatusDefaultEnum.InActive && permEdit;
                        }
                    }
                }
                else
                {
                    var permCreate = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                        Functions.CostEstimateCreate, PermissionConstant.ADD);
                    ViewBag.PermissionEdit = permCreate;
                    var canCreate = await _costEstimateRepository.CanCreate(sessionUser.Unit.UnitId, IsSubUnit(sessionUser), cWeek);
                    ViewBag.CanCreate = canCreate;
                }

                ViewBag.RequestMode = type;
                ViewBag.PageModel = model;
                if (type?.Equals("approve") == true)
                {
                    var permApprove = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                        Functions.CostEstimateCreate, PermissionConstant.APPROVE);
                    ViewBag.PermissionApprove = permApprove;
                }
                else
                    ViewBag.PermissionApprove = false;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
            }

            return View(estimate);
        }
        [AuthorizeUser(Module = Functions.CostEstimateView, Permission = PermissionConstant.VIEW)]
        public async Task<IActionResult> Manage()
        {
            var searchModel = new SearchModel
            {
                Stats = GlobalEnums.DefaultStatusSearchList.Select(cc => new SelectListItem { Text = cc.Name, Value = $"{cc.Value}" }).ToList(),
                DefaultStats = Globs.DefaultSearchAllStats,
                DefaultStatsName = Globs.DefaultSearchAllStatsName
            };
            try
            {
                var allUnits = (await _unitRepository.GetAllAsync(string.Empty, 0, 1000));
                var searchUnit = allUnits
                    .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"[{x.OfficesCode}] - {x.OfficesShortName ?? x.OfficesName}" }).ToList();
                var unitSearch = new List<SelectListItem>();
                unitSearch.AddRange(searchUnit);
                searchModel.Units = unitSearch;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
            }

            return View(searchModel);
        }
        [AuthorizeUser(Module = Functions.CostEstimateView, Permission = PermissionConstant.VIEW)]
        public async Task<IActionResult> SearchManage(int length, int start, SearchManageRequest @base)
        {
            try
            {
                @base.Draw = (Request.Form["draw"].Count > 0 ? Request.Form["draw"][0] : "0").ToInt32();
                @base.HostFileView = _fileHostView;
                var data = await _costEstimateRepository.SearchManage(@base,start, length);
                data.Draw = @base.Draw;
                return Json(data);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return Json(new SearchCostEstimateResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau!",
                    Draw = @base.Draw
                });
            }
        }

        [AuthorizeUser(Module = Functions.CostEstimateView, Permission = PermissionConstant.VIEW)]
        public async Task<IActionResult> ExportFile(List<DashboardExportRequest> data, string exportType)
        {
            try
            {
                var excelPaths = new List<FileNPlanType>();
                var pdfPaths = new List<FileNPlanType>();
                var listUnits = new List<int>();
                var export = await _dashboardRepository.Export(data, excelPaths, pdfPaths, listUnits, _physicalFolder);
                if (export.Code == (int)GlobalEnums.ResponseCodeEnum.Success)
                {
                    var exporter = new Exporter();
                    var rtExport = exporter.Export(excelPaths, pdfPaths, listUnits, exportType, _physicalFolder, _fileHostView);
                    return Json(rtExport);

                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
            }
            return Json(new DashboardListResponse
            {
                Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                Message = "Lỗi hệ thống, vui lòng thử lại sau!"
            });
        }

        public async Task<string> _createPdfName(int year, UnitModel unit, int week)
        {
            try
            {
                int fileCounter =
                    await _pdfLogsRepository.CounterDay(unit.UnitId, CashFollowConst.PublicKey);
                string templateName =
                    $"{DateTime.Now:yyyy-MM-dd}_{year}_{unit.UnitName.StringToNonUnicode()}_Du tru tuan {week}_{fileCounter}.pdf";
                return templateName;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return string.Empty;
            }
        }
    }
}
