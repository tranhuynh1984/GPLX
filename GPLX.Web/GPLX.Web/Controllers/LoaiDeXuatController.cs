using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GPLX.Core.Contants;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.LoaiDeXuat;
using GPLX.Core.DTO.Request;
using GPLX.Core.DTO.Request.LoaiDeXuat;
using GPLX.Core.DTO.Request.Users;
using GPLX.Core.DTO.Response.Groups;
using GPLX.Core.DTO.Response.LoaiDeXuat;
using GPLX.Core.Enum;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Web.Filters;
using GPLX.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using GPLX.Core.DTO.Request.DM;
using GPLX.Core.DTO.Response.DM;

using GPLX.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using ContentDispositionHeaderValue = System.Net.Http.Headers.ContentDispositionHeaderValue;
using HttpContentMediaTypes = GPLX.Core.Contants.HttpContentMediaTypes;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;
using System.IO;
using System.Linq;
using Aspose.Cells;

namespace GPLX.Web.Controllers
{
    public class LoaiDeXuatController : BaseController
    {
        private readonly ILoaiDeXuatRepository _loaiDeXuatRepository;
        private readonly ILogger<LoaiDeXuatController> _logger;
        private readonly IMedAuthenticateConnect _authenticateConnect;
        private readonly IConfiguration _configuration;

        public LoaiDeXuatController(ILoaiDeXuatRepository loaiDeXuatRepository, ILogger<LoaiDeXuatController> logger, IMedAuthenticateConnect authenticateConnect, IConfiguration configuration)
        {
            _loaiDeXuatRepository = loaiDeXuatRepository;
            _logger = logger;
            _authenticateConnect = authenticateConnect;
            _configuration = configuration;
        }

        public IActionResult List()
        {
            return View();
        }

        public async Task<IActionResult> Search(int length, int start, LoaiDeXuatSearchRequest @base)
        {
            LoaiDeXuatSearchResponse data = new LoaiDeXuatSearchResponse();

            @base.Draw = Request.Query["draw"].ToString().ToInt32();
            @base.RequestPage = DepartmentConst.PublicKey;
            data = await _loaiDeXuatRepository.Search(start, length, @base);

            return Json(data);
        }

        public async Task<IActionResult> ExportExcel(LoaiDeXuatSearchRequest @base)
        {
            var data = await _loaiDeXuatRepository.SearchAll(@base).ConfigureAwait(false);
            var mappingHeader = new Dictionary<string, string>
            {
                ["Index"] = "STT",
                ["LoaiDeXuatCode"] = "Mã loại đề xuất",
                ["LoaiDeXuatName"] = "Tên loại đề xuất",
                ["IsActiveName"] = "Trạng thái",
                ["Stt"] = "Thứ tự",
                ["Createby"] = "Người tạo",
                ["CreatedateString"] = "Thời gian tạo",
                ["Updateby"] = "Người sửa",
                ["UpdatedateString"] = "Thời gian sửa"
            };
            var workbook = ExcelService.ExportExcel(mappingHeader, data.Data.Cast<dynamic>().ToList(), "Danh sách loại đề xuất");
            var memoryStream = new MemoryStream();
            workbook.Save(memoryStream, new OoxmlSaveOptions(SaveFormat.Xlsx));
            memoryStream.Position = 0;
            byte[] sheetData = memoryStream.ToArray();
            return File(sheetData, HttpContentMediaTypes.XLSX, "Danh_sach_loai_de_xuat.xlsx");
        }

        public async Task<IActionResult> Create(string record = default, string viewMode = default)
        {
            ViewBag.ViewMode = viewMode;
            LoaiDeXuatSearchResponseData model = null;
            if (!string.IsNullOrEmpty(record))
            {
                model = await _loaiDeXuatRepository.GetById(record);
            }
            return PartialView(model);
        }

        public async Task<IActionResult> OnCreate(LoaiDeXuatCreateRequest request)
        {
            request.Creator = GetUserId();
            request.CreatorName = GetUserSyncId();
            var response = await _loaiDeXuatRepository.Create(request);

            return Json(response);
        }
        public async Task<IActionResult> OnRemove(LoaiDeXuatCreateRequest request)
        {
            var response = await _loaiDeXuatRepository.Remove(request);

            return Json(response);
        }
    }
}
