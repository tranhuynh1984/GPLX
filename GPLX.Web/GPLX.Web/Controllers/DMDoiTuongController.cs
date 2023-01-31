using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GPLX.Core.Contants;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.DM;
using GPLX.Core.DTO.Request;
using GPLX.Core.DTO.Request.DM;
using GPLX.Core.DTO.Request.Users;
using GPLX.Core.DTO.Response.Groups;
using GPLX.Core.DTO.Response.DM;
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
    public class DMDoiTuongController : BaseController
    {
        private readonly IDMRepository _DMRepository;
        private readonly ILogger<DMTinhController> _logger;
        private readonly IMedAuthenticateConnect _authenticateConnect;
        private readonly IConfiguration _configuration;

        public DMDoiTuongController(IDMRepository DMRepository, ILogger<DMTinhController> logger, IMedAuthenticateConnect authenticateConnect, IConfiguration configuration)
        {
            _DMRepository = DMRepository;
            _logger = logger;
            _authenticateConnect = authenticateConnect;
            _configuration = configuration;
        }

        public IActionResult List()
        {
            return View();
        }

        public async Task<IActionResult> Search(int length, int start, DMSearchRequest @base)
        {
            DMSearchResponse data;

            @base.Draw = Request.Query["draw"].ToString().ToInt32();
            @base.RequestPage = DepartmentConst.PublicKey;
            //Danh mục ck doi tuong	83
            @base.IdTree = 83;
            data = await _DMRepository.Search(start, length, @base);

            return Json(data);
        }

        public async Task<IActionResult> ExportExcel(DMSearchRequest @base)
        {
            //Danh mục ck doi tuong	83
            @base.IdTree = 83;
            var data = await _DMRepository.SearchAll(@base).ConfigureAwait(false);
            var mappingHeader = new Dictionary<string, string>
            {
                ["Index"] = "STT",
                ["MaDM"] = "Mã đối tượng",
                ["TenDM"] = "Tên đối tượng",
                ["IsActiveName"] = "Trạng thái",
                ["Stt"] = "Thứ tự",
                ["Createby"] = "Người tạo",
                ["CreatedateString"] = "Thời gian tạo",
                ["Updateby"] = "Người sửa",
                ["UpdatedateString"] = "Thời gian sửa"
            };
            var workbook = ExcelService.ExportExcel(mappingHeader, data.Data.Cast<dynamic>().ToList(), "Danh sách đối tượng");
            var memoryStream = new MemoryStream();
            workbook.Save(memoryStream, new OoxmlSaveOptions(SaveFormat.Xlsx));
            memoryStream.Position = 0;
            byte[] sheetData = memoryStream.ToArray();
            return File(sheetData, HttpContentMediaTypes.XLSX, "Danh_sach_doi_tuong.xlsx");
        }

        public async Task<IActionResult> Create(string record = default, string viewMode = default)
        {
            ViewBag.ViewMode = viewMode;
            DMSearchResponseData model = new DMSearchResponseData();
            if (!string.IsNullOrEmpty(record))
            {
                model = await _DMRepository.GetById(83, record);
            }
            else
            {
                model.MaxStt = await _DMRepository.GetMaxStt(83);
                model.IsActive = 1;
            }
            return PartialView(model);
        }

        public async Task<IActionResult> OnCreate(DMCreateRequest request)
        {
            request.RequestPage = DepartmentConst.PublicKey;
            request.Creator = GetUserId();
            request.CreatorName = GetUserSyncId();
            request.IdTree = 83;
            var response = await _DMRepository.Create(request);

            return Json(response);
        }
        public async Task<IActionResult> OnRemove(DMCreateRequest request)
        {
            request.RequestPage = DepartmentConst.PublicKey;
            request.IdTree = 83;
            var response = await _DMRepository.Remove(request);

            return Json(response);
        }
    }
}
