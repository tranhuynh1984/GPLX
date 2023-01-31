using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GPLX.Core.Contants;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.DMHuyen;
using GPLX.Core.DTO.Request;
using GPLX.Core.DTO.Request.DMHuyen;
using GPLX.Core.DTO.Request.Users;
using GPLX.Core.DTO.Response.Groups;
using GPLX.Core.DTO.Response.DMHuyen;
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
using GPLX.Core.Contracts.DM;
using GPLX.Web.Models.Dashboard;
using GPLX.Core.DTO.Response.DM;
using GPLX.Core.DTO.Request.DM;

namespace GPLX.Web.Controllers
{
    public class DMHuyenController : BaseController
    {
        private readonly IDMRepository _DMRepository;
        private readonly IDMHuyenRepository _DMHuyenRepository;
        private readonly ILogger<DMHuyenController> _logger;
        private readonly IMedAuthenticateConnect _authenticateConnect;
        private readonly IConfiguration _configuration;

        public DMHuyenController(IDMRepository DMRepository, IDMHuyenRepository DMHuyenRepository, ILogger<DMHuyenController> logger, IMedAuthenticateConnect authenticateConnect, IConfiguration configuration)
        {
            _DMRepository = DMRepository;
            _DMHuyenRepository = DMHuyenRepository;
            _logger = logger;
            _authenticateConnect = authenticateConnect;
            _configuration = configuration;
        }
        public async Task<IActionResult> List()
        {
            var searchModel = new DMHuyenFilterModel();
            searchModel.DMTinh = new List<SelectListItem>();
            searchModel.DMTinh.Add(new SelectListItem("Tất cả", "-1"));

            DMSearchResponse data;
            DMSearchRequest request = new DMSearchRequest();
            request.IdTree = 59;
            request.Status = -1;
            data = await _DMRepository.SearchAll(request).ConfigureAwait(false);

            foreach (var d in data.Data)
            {
                var item = new SelectListItem();
                item.Text = d.MaDM + "-" + d.TenDM;
                item.Value = d.MaDM;

                searchModel.DMTinh.Add(item);
            }

            return View(searchModel);
        }

        public async Task<IActionResult> Search(int length, int start, DMHuyenSearchRequest @base)
        {
            @base.Draw = Request.Query["draw"].ToString().ToInt32();
            @base.RequestPage = DepartmentConst.PublicKey;
            DMHuyenSearchResponse data = await _DMHuyenRepository.Search(start, length, @base);
            return Json(data);
        }

        public async Task<IActionResult> ExportExcel(DMHuyenSearchRequest @base)
        {
            var data = await _DMHuyenRepository.SearchAll(@base).ConfigureAwait(false);
            var mappingHeader = new Dictionary<string, string>
            {
                ["Stt"] = "STT",
                ["MaHuyen"] = "Mã quận huyện",
                ["TenHuyen"] = "Tên quận huyện",
                ["MaTinh"] = "Mã tỉnh thành",
                ["TenTinh"] = "Tên tỉnh thành",
                ["IsActiveName"] = "Trạng thái",
            };
            var workbook = ExcelService.ExportExcel(mappingHeader, data.Data.Cast<dynamic>().ToList(), "Danh sách quận huyện");
            using (var memoryStream = new MemoryStream())
            {
                workbook.Save(memoryStream, new OoxmlSaveOptions(SaveFormat.Xlsx));
                memoryStream.Position = 0;
                byte[] sheetData = memoryStream.ToArray();
                return File(sheetData, HttpContentMediaTypes.XLSX, "Danh_sach_quan_huyen.xlsx");
            }
        }
    }
}
