using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GPLX.Core.Contants;
using GPLX.Core.Contracts.Dashboard;
using GPLX.Core.Contracts.Unit;
using GPLX.Core.DTO.Request.Dashboard;
using GPLX.Core.DTO.Response.Dashboard;
using GPLX.Core.Enum;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Web.Filters;
using GPLX.Web.Models.Dashboard;
using GPLX.Web.Process;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace GPLX.Web.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly string _fileHostView;

        private readonly string _defaultRootFolder;
        private readonly string _physicalFolder;


        public DashboardController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IDashboardRepository dashboardRepository, IUnitRepository unitRepository)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _dashboardRepository = dashboardRepository;
            _unitRepository = unitRepository;
            _defaultRootFolder = configuration.GetValue<string>("DefaultRootFolder");
            _fileHostView = configuration.GetValue<string>("FileHosting");

            _physicalFolder = Path.Combine(_webHostEnvironment.WebRootPath, _defaultRootFolder);
        }
        [AuthorizeUser(Module = Functions.DashboardView, Permission = PermissionConstant.VIEW)]
        public async Task<IActionResult> List()
        {
            var searchModel = new SearchModel
            {
                Plans = new List<SelectListItem>
                {
                    new SelectListItem{Value = GlobalEnums.Revenue, Text = "Kế hoạch DT-KH"},
                    new SelectListItem{Value = GlobalEnums.Profit, Text = "Kế hoạch lợi nhuận"},
                    new SelectListItem{Value = GlobalEnums.Investment, Text = "Kế hoạch đầu tư"},
                    new SelectListItem{Value = GlobalEnums.CashFollow, Text = "Kế hoạch dòng tiền"}
                },
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
        [AuthorizeUser(Module = Functions.DashboardView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> Search(int length, int start, DashboardListRequest @base)
        {
            try
            {
                @base.Draw = (Request.Form["draw"].Count > 0 ? Request.Form["draw"][0] : "0").ToInt32();
                @base.HostFileView = _fileHostView;
                var data = await _dashboardRepository.SearchAsync(start, length, @base);
                data.Draw = @base.Draw;
                return Json(data);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return Json(new DashboardListResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau!",
                    Draw = @base.Draw
                });
            }
        }

        [AuthorizeUser(Module = Functions.DashboardView, Permission = PermissionConstant.VIEW)]
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
    }
}
