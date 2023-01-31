using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Aspose.Cells;
using GPLX.Core.Contants;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.DMPN;
using GPLX.Core.DTO.Request;
using GPLX.Core.DTO.Request.DMPN;
using GPLX.Core.DTO.Request.Users;
using GPLX.Core.DTO.Response.Groups;
using GPLX.Core.DTO.Response.DMPN;
using GPLX.Core.DTO.Response.UpdateTime;
using GPLX.Core.Enum;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Infrastructure.Services;
using GPLX.Web.Filters;
using GPLX.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using ContentDispositionHeaderValue = System.Net.Http.Headers.ContentDispositionHeaderValue;
using HttpContentMediaTypes = GPLX.Core.Contants.HttpContentMediaTypes;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace GPLX.Web.Controllers
{
    public class DMPNController : BaseController
    {
        private readonly IDMPNRepository _DMPNRepository;
        private readonly ILogger<DMPNController> _logger;
        private readonly IMedAuthenticateConnect _authenticateConnect;
        private readonly IConfiguration _configuration;

        public DMPNController(IDMPNRepository DMPNRepository, ILogger<DMPNController> logger, IMedAuthenticateConnect authenticateConnect, IConfiguration configuration)
        {
            _DMPNRepository = DMPNRepository;
            _logger = logger;
            _authenticateConnect = authenticateConnect;
            _configuration = configuration;
        }
        //[AuthorizeUser(Module = Functions.DMDVView, Permission = PermissionConstant.VIEW)]
        public IActionResult List()
        {
            return View();
        }

        public async Task<IActionResult> Search(int length, int start, DMPNSearchRequest @base)
        {
            DMPNSearchResponse data;

            @base.Draw = Request.Query["draw"].ToString().ToInt32();
            @base.RequestPage = DepartmentConst.PublicKey;
            data = await _DMPNRepository.Search(start, length, @base);

            return Json(data);
        }
        
        public async Task<IActionResult> ExportExcel(DMPNSearchRequest @base)
        {
            var data = await _DMPNRepository.SearchAll(@base).ConfigureAwait(false);
            var mappingHeader = new Dictionary<string, string>
            {
                ["Stt"] = "STT",
                ["PhapNhanId"] = "Id pháp nhân",
                ["PhapNhanName"] = "Tên pháp nhân",
                ["IsActiveName"] = "Trạng thái",
                ["CompanyName"] ="Tên công ty",
                ["TaxNumber"] = "Mã số thuế",
                ["AddressCompany"] = "Địa chỉ"
            };
            var workbook = ExcelService.ExportExcel(mappingHeader, data.Data.Cast<dynamic>().ToList(), "Danh sách pháp nhân");
            using (var memoryStream = new MemoryStream())
            {
                workbook.Save(memoryStream, new OoxmlSaveOptions(SaveFormat.Xlsx));
                memoryStream.Position = 0;
                byte[] sheetData = memoryStream.ToArray();
                return File(sheetData, HttpContentMediaTypes.XLSX, "Danh_sach_phap_nhan.xlsx");
            }
        }
    }
}
