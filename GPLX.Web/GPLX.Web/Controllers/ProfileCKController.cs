using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GPLX.Core.Contants;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.ProfileCK;
using GPLX.Core.DTO.Request;
using GPLX.Core.DTO.Request.ProfileCK;
using GPLX.Core.DTO.Request.Users;
using GPLX.Core.DTO.Response.Groups;
using GPLX.Core.DTO.Response.ProfileCK;
using GPLX.Core.Enum;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Web.Filters;
using GPLX.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using GPLX.Web.Models.Dashboard;
using GPLX.Core.Contracts.DMBS_ChuyenKhoa;
using GPLX.Core.DTO.Response.DMBS_ChuyenKhoa;
using GPLX.Core.DTO.Request.DMBS_ChuyenKhoa;
using GPLX.Infrastructure.Services;
using System.IO;
using ContentDispositionHeaderValue = System.Net.Http.Headers.ContentDispositionHeaderValue;
using HttpContentMediaTypes = GPLX.Core.Contants.HttpContentMediaTypes;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;
using System.Linq;
using Aspose.Cells;
using GPLX.Core.Contracts.DMCP;
using GPLX.Core.DTO.Request.DMCP;
using GPLX.Core.Contracts.ProfileCKCP;
using GPLX.Core.DTO.Response.ProfileCKCP;
using GPLX.Core.DTO.Request.ProfileCKCP;
using GPLX.Core.DTO.Response.DMCP;

namespace GPLX.Web.Controllers
{
    public class ProfileCKController : BaseController
    {
        private readonly IDMBS_ChuyenKhoaRepository _DMBS_ChuyenKhoaRepository;
        private readonly IProfileCKRepository _ProfileCKRepository;
        private readonly IProfileCKCPRepository _ProfileCKCPRepository;
        private readonly ILogger<ProfileCKController> _logger;
        private readonly IMedAuthenticateConnect _authenticateConnect;
        private readonly IConfiguration _configuration;
        private readonly IDMCPRepository _DMCPRepository;

        public ProfileCKController(IDMCPRepository dMCPRepository, IDMBS_ChuyenKhoaRepository DMBS_ChuyenKhoaRepository, IProfileCKRepository profileCKRepository, IProfileCKCPRepository profileCKCPRepository, ILogger<ProfileCKController> logger, IMedAuthenticateConnect authenticateConnect, IConfiguration configuration)
        {
            _DMBS_ChuyenKhoaRepository = DMBS_ChuyenKhoaRepository;
            _ProfileCKRepository = profileCKRepository;
            _ProfileCKCPRepository = profileCKCPRepository;
            _logger = logger;
            _authenticateConnect = authenticateConnect;
            _configuration = configuration;
            _DMCPRepository = dMCPRepository;
        }

        public async Task<IActionResult> List()
        {
            var searchModel = new DMProfileCKFilterModel();

            DMBS_ChuyenKhoaSearchResponse data;
            DMBS_ChuyenKhoaSearchRequest request = new DMBS_ChuyenKhoaSearchRequest();
            request.Status = 1;
            data = await _DMBS_ChuyenKhoaRepository.SearchAll(request).ConfigureAwait(false);
            searchModel.DMChuyenKhoa = data.Data;

            return View(searchModel);
        }
        public async Task<IActionResult> Search(int length, int start, ProfileCKSearchRequest @base)
        {
            @base.Draw = Request.Query["draw"].ToString().ToInt32();
            @base.RequestPage = DepartmentConst.PublicKey;
            ProfileCKSearchResponse data = await _ProfileCKRepository.Search(start, length, @base);
            return Json(data);
        }

        public async Task<IActionResult> ExportExcel(ProfileCKSearchRequest @base)
        {
            var data = await _ProfileCKRepository.SearchAll(@base).ConfigureAwait(false);
            var mappingHeader = new Dictionary<string, string>
            {
                ["Index"] = "STT",
                ["ProfileCKMa"] = "Mã profile",
                ["ProfileCKTen"] = "Tên profile",
                ["IsActiveName"] = "Trạng thái",
                ["Createby"] = "Người tạo",
                ["CreatedateString"] = "Thời gian tạo",
                ["Updateby"] = "Người sửa",
                ["UpdatedateString"] = "Thời gian sửa"
            };
            
            var workbook = ExcelService.ExportExcel(mappingHeader, data.Data.Cast<dynamic>().ToList(), "Danh sách profile ck");
            using (var memoryStream = new MemoryStream())
            {
                workbook.Save(memoryStream, new OoxmlSaveOptions(SaveFormat.Xlsx));
                memoryStream.Position = 0;
                byte[] sheetData = memoryStream.ToArray();
                return File(sheetData, HttpContentMediaTypes.XLSX, "Danh_sach_profile_ck.xlsx");
            }
        }

        public async Task<IActionResult> Create(string ProfileCKMa = "")
        {
            ProfileCKFilterModel modelFilter = new ProfileCKFilterModel();
            ProfileCKDetailSearchResponse model = new ProfileCKDetailSearchResponse();
            model.ProfileCKMa = ProfileCKMa;
            model.ChuyenKhoaMa = "";
            model.Data = new ProfileCKSearchResponseData();
            model.Data.IsActive = 1;
            if (ProfileCKMa != "")
                model.Data = await _ProfileCKRepository.GetById(ProfileCKMa);
            DMBS_ChuyenKhoaSearchRequest request = new DMBS_ChuyenKhoaSearchRequest();
            request.Status = 1;
            model.ListChuyenKhoa = await _DMBS_ChuyenKhoaRepository.SearchAll(request).ConfigureAwait(false);

            DMCPSearchRequest requestdmcp = new DMCPSearchRequest();
            requestdmcp.Status = 1;
            var ListDMCP = await _DMCPRepository.SearchAll(requestdmcp);
            modelFilter.DMCPSearchResponse = ListDMCP;

            //DMCPSearchResponseData firstDMCP = new DMCPSearchResponseData();
            //firstDMCP.MaCP = "-1";
            //firstDMCP.TenCP = "Hãy chọn dịch vụ";

            //ListDMCP.Data.Insert(0, firstDMCP);
            var lstDMCP = new SelectList(ListDMCP.Data, "MaCP", "TenCP", "-1");

            modelFilter.ProfileCKDetailSearchResponse = model;
            modelFilter.ListDMCP = lstDMCP;

            return View(modelFilter);
        }

        public async Task<IActionResult> OnCreate(ProfileCKCreateRequest request)
        {
            request.Creator = GetUserId();
            request.CreatorName = GetUserSyncId();
            var response = await _ProfileCKRepository.Create(request);

            return Json(response);
        }
        public async Task<IActionResult> OnRemove(ProfileCKCreateRequest request)
        {
            var response = await _ProfileCKRepository.Remove(request);

            return Json(response);
        }

        public async Task<IActionResult> SearchCreate(int length, int start, ProfileCKSearchRequest @base)
        {
            ProfileCKCPSearchResponse data = await _ProfileCKCPRepository.Search(@base.ProfileCKMa, @base.Keywords);
            data.Draw = Request.Query["draw"].ToString().ToInt32();
            return Json(data);
        }

        public async Task<IActionResult> OnCreateCKCP(ProfileCKCPCreateRequest @base)
        {
            ProfileCKCPCreateResponse data = new ProfileCKCPCreateResponse();
            @base.Creator = GetUserId();
            @base.CreatorName = GetUserSyncId();
            data = await _ProfileCKCPRepository.Create(@base);
            return Json(data);
        }
        public async Task<IActionResult> OnRemoveCKCP(ProfileCKCPCreateRequest @base)
        {
            ProfileCKCPCreateResponse data = new ProfileCKCPCreateResponse();

            data = await _ProfileCKCPRepository.Remove(@base);
            return Json(data);
        }
    }
}
