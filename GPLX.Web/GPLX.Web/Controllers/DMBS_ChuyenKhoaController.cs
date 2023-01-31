using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GPLX.Core.Contants;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.DMBS_ChuyenKhoa;
using GPLX.Core.DTO.Request;
using GPLX.Core.DTO.Request.DMBS_ChuyenKhoa;
using GPLX.Core.DTO.Request.Users;
using GPLX.Core.DTO.Response.Groups;
using GPLX.Core.DTO.Response.DMBS_ChuyenKhoa;
using GPLX.Core.Enum;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Web.Filters;
using GPLX.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    public class DMBS_ChuyenKhoaController : BaseController
    {
        private readonly IDMBS_ChuyenKhoaRepository _DMBS_ChuyenKhoaRepository;
        private readonly ILogger<DMBS_ChuyenKhoaController> _logger;
        private readonly IMedAuthenticateConnect _authenticateConnect;
        private readonly IConfiguration _configuration;

        public DMBS_ChuyenKhoaController(IDMBS_ChuyenKhoaRepository DMBS_ChuyenKhoaRepository, ILogger<DMBS_ChuyenKhoaController> logger, IMedAuthenticateConnect authenticateConnect, IConfiguration configuration)
        {
            _DMBS_ChuyenKhoaRepository = DMBS_ChuyenKhoaRepository;
            _logger = logger;
            _authenticateConnect = authenticateConnect;
            _configuration = configuration;
        }

        public IActionResult List()
        {
            return View();
        }
        public async Task<IActionResult> Search(int length, int start, DMBS_ChuyenKhoaSearchRequest @base)
        {
            DMBS_ChuyenKhoaSearchResponse data;

            @base.Draw = Request.Query["draw"].ToString().ToInt32();
            @base.RequestPage = DepartmentConst.PublicKey;
            data = await _DMBS_ChuyenKhoaRepository.Search(start, length, @base);

            return Json(data);
        }

        public async Task<IActionResult> ExportExcel(DMBS_ChuyenKhoaSearchRequest @base)
        {
            var data = await _DMBS_ChuyenKhoaRepository.SearchAll(@base).ConfigureAwait(false);
            var mappingHeader = new Dictionary<string, string>
            {
                ["Index"] = "STT",
                ["Ma"] = "Mã chuyên khoa",
                ["Ten"] = "Tên chuyên khoa",
                ["IsActiveName"] = "Trạng thái",
                ["Stt"] = "Thứ tự",
                ["Createby"] = "Người tạo",
                ["CreatedateString"] = "Thời gian tạo",
                ["Updateby"] = "Người sửa",
                ["UpdatedateString"] = "Thời gian sửa"
            };
            var workbook = ExcelService.ExportExcel(mappingHeader, data.Data.Cast<dynamic>().ToList(), "Danh sách bác sĩ chuyên khoa");
            var memoryStream = new MemoryStream();
            workbook.Save(memoryStream, new OoxmlSaveOptions(SaveFormat.Xlsx));
            memoryStream.Position = 0;
            byte[] sheetData = memoryStream.ToArray();
            return File(sheetData, HttpContentMediaTypes.XLSX, "Danh_sach_chuyen_khoa.xlsx");
        }

        public async Task<IActionResult> Create(string record = default, string viewMode = default)
        {
            ViewBag.ViewMode = viewMode;
            DMBS_ChuyenKhoaSearchResponseData model = new DMBS_ChuyenKhoaSearchResponseData();
            model.IsActive = 1;
            if (!string.IsNullOrEmpty(record))
            {
                model = await _DMBS_ChuyenKhoaRepository.GetById(record);
            }
            else
            {
                model.MaxStt = await _DMBS_ChuyenKhoaRepository.GetMaxStt();
            }
            return PartialView(model);
        }

        public async Task<IActionResult> OnCreate(DMBS_ChuyenKhoaCreateRequest request)
        {
            request.Creator = GetUserId();
            request.CreatorName = GetUserSyncId();
            var response = await _DMBS_ChuyenKhoaRepository.Create(request);

            return Json(response);
        }
        public async Task<IActionResult> OnRemove(DMBS_ChuyenKhoaCreateRequest request)
        {
            var response = await _DMBS_ChuyenKhoaRepository.Remove(request);

            return Json(response);
        }
    }
}
