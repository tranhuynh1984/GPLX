using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GPLX.Core.Contants;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Relationship;
using GPLX.Core.DTO.Request;
using GPLX.Core.DTO.Request.Relationship;
using GPLX.Core.DTO.Request.Users;
using GPLX.Core.DTO.Response.Groups;
using GPLX.Core.DTO.Response.Relationship;
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
    public class RelationshipController : BaseController
    {
        private readonly IRelationshipRepository _RelationshipRepository;
        private readonly ILogger<RelationshipController> _logger;
        private readonly IMedAuthenticateConnect _authenticateConnect;
        private readonly IConfiguration _configuration;

        public RelationshipController(IRelationshipRepository RelationshipRepository, ILogger<RelationshipController> logger, IMedAuthenticateConnect authenticateConnect, IConfiguration configuration)
        {
            _RelationshipRepository = RelationshipRepository;
            _logger = logger;
            _authenticateConnect = authenticateConnect;
            _configuration = configuration;
        }

        public IActionResult List()
        {
            return View();
        }

        public async Task<IActionResult> Search(int length, int start, RelationshipSearchRequest @base)
        {
            RelationshipSearchResponse data = new RelationshipSearchResponse();

            @base.Draw = Request.Query["draw"].ToString().ToInt32();
            @base.RequestPage = DepartmentConst.PublicKey;
            data = await _RelationshipRepository.Search(start, length, @base);

            return Json(data);
        }

        public async Task<IActionResult> ExportExcel(RelationshipSearchRequest @base)
        {
            var data = await _RelationshipRepository.SearchAll(@base).ConfigureAwait(false);
            var mappingHeader = new Dictionary<string, string>
            {
                ["Index"] = "STT",
                ["RelationshipCode"] = "Mã quan hệ",
                ["RelationshipName"] = "Tên quan hệ",
                ["IsActiveName"] = "Trạng thái",
                ["Stt"] = "Thứ tự",
                ["Createby"] = "Người tạo",
                ["CreatedateString"] = "Thời gian tạo",
                ["Updateby"] = "Người sửa",
                ["UpdatedateString"] = "Thời gian sửa"
            };
            var workbook = ExcelService.ExportExcel(mappingHeader, data.Data.Cast<dynamic>().ToList(), "Danh sách quan hệ");
            var memoryStream = new MemoryStream();
            workbook.Save(memoryStream, new OoxmlSaveOptions(SaveFormat.Xlsx));
            memoryStream.Position = 0;
            byte[] sheetData = memoryStream.ToArray();
            return File(sheetData, HttpContentMediaTypes.XLSX, "Danh_sach_quan_he.xlsx");
        }

        public async Task<IActionResult> Create(string record = default, string viewMode = default)
        {
            ViewBag.ViewMode = viewMode;
            RelationshipSearchResponseData model = null;
            if (!string.IsNullOrEmpty(record))
            {
                model = await _RelationshipRepository.GetById(record);
            }
            return PartialView(model);
        }

        public async Task<IActionResult> OnCreate(RelationshipCreateRequest request)
        {
            request.Creator = GetUserId();
            request.CreatorName = GetUserSyncId();
            var response = await _RelationshipRepository.Create(request);

            return Json(response);
        }
        public async Task<IActionResult> OnRemove(RelationshipCreateRequest request)
        {
            var response = await _RelationshipRepository.Remove(request);

            return Json(response);
        }
    }
}
