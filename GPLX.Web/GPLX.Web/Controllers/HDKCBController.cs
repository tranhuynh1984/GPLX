using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Aspose.Cells;
using GPLX.Core.Contants;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.HDKCB;
using GPLX.Core.DTO.Request;
using GPLX.Core.DTO.Request.HDKCB;
using GPLX.Core.DTO.Request.Users;
using GPLX.Core.DTO.Response.Groups;
using GPLX.Core.DTO.Response.HDKCB;
using GPLX.Core.Enum;
using GPLX.Core.Exceptions;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Infrastructure.Services;
using GPLX.Web.Filters;
using GPLX.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using HttpContentMediaTypes = GPLX.Core.Contants.HttpContentMediaTypes;

namespace GPLX.Web.Controllers
{
    public class HDKCBController : BaseController
    {
        private readonly IHDKCBRepository _HDKCBRepository;
        private readonly ILogger<HDKCBController> _logger;
        private readonly IMedAuthenticateConnect _authenticateConnect;
        private readonly IConfiguration _configuration;

        public HDKCBController(IHDKCBRepository hdkcbRepository, ILogger<HDKCBController> logger, IMedAuthenticateConnect authenticateConnect, IConfiguration configuration)
        {
            _HDKCBRepository = hdkcbRepository;
            _logger = logger;
            _authenticateConnect = authenticateConnect;
            _configuration = configuration;
        }
        
        public IActionResult List()
        {
            return View();
        }

        public async Task<IActionResult> Search(int length, int start, HDKCBSearchRequest @base)
        {
            HDKCBSearchResponse data;

            @base.Draw = Request.Query["draw"].ToString().ToInt32();
            data = await _HDKCBRepository.Search(start, length, @base);

            return Json(data);
        }
        public async Task<IActionResult> Detail(string id = default, string viewMode = default)
        {
            ViewBag.ViewMode = viewMode;
            HDKCBDetailResponse model = null;
            if (!string.IsNullOrEmpty(id))
            {
                model = await _HDKCBRepository.GetByIdAsync(id.ToInt32());
            }
            return PartialView("Detail", model);
        }
        
        public async Task<IActionResult> ServiceDetail(int id, int? draw, string keyword, int start = 0, int length = 25)
        {
            return Json(await _HDKCBRepository.GetByHDKCKService(start, length, keyword, draw, id));
        }
        public async Task<IActionResult> ExportExcel(HDKCBSearchRequest @base)
        {
            var data = await _HDKCBRepository.SearchAll(@base).ConfigureAwait(false);
            var mappingHeader = new Dictionary<string, string>
            {
                ["Stt"] = "STT",
                ["IDHD"] = "ID hợp đồng",
                ["MaHD"] = "Mã hợp đồng",
                ["TenHD"] ="Tên hợp đồng",
                ["MaLoai"] ="Mã loại HĐ",
                ["NDString"] = "Ngày bắt đầu",
                ["NSString"] = "Ngày kết thúc",
                ["IsActiveName"] = "Trạng thái",
                ["Updateby"] = "Người sửa",
                ["UpdatedateString"] = "Thời gian sửa"
            };
            var workbook = ExcelService.ExportExcel(mappingHeader, data.Data.Cast<dynamic>().ToList(), "Danh sách HDCKB");
            using (var memoryStream = new MemoryStream())
            {
                workbook.Save(memoryStream, new OoxmlSaveOptions(SaveFormat.Xlsx));
                memoryStream.Position = 0;
                byte[] sheetData = memoryStream.ToArray();
                return File(sheetData, HttpContentMediaTypes.XLSX, "Danh_sach_hdkcb.xlsx");
            }
        }
    }
}
