using Aspose.Cells;
using GPLX.Core.Contants;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.DMBS_ChuyenKhoa;
using GPLX.Core.Contracts.DMCTV;
using GPLX.Core.DTO.Request.DMCP;
using GPLX.Core.DTO.Request.DMCVT;
using GPLX.Core.DTO.Response.DMCP;
using GPLX.Core.DTO.Response;
using GPLX.Core.DTO.Response.Groups;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Infrastructure.Services;
using GPLX.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GPLX.Core.DTO.Response.DMCTV;
using GPLX.Core.Contracts.DMDV;
using GPLX.Core.DTO.Response.DMDV;
using Microsoft.AspNetCore.Mvc.Rendering;
using GPLX.Core.DTO.Request.DMCTV;

namespace GPLX.Web.Controllers
{
    public class DMCTVController : BaseController
    {
        private readonly IDMCTVRepository _DMCTVRepository;
        private readonly IDMDVRepository _DMDVRepository;
        private readonly IDMBS_ChuyenKhoaRepository _ChuyenKhoaRepository;

        private readonly ILogger<DMCTVController> _logger;

        private readonly IMedAuthenticateConnect _authenticateConnect;
        private readonly IConfiguration _configuration;

        public DMCTVController(ILogger<DMCTVController> logger, IMedAuthenticateConnect authenticateConnect
            , IConfiguration configuration
            , IDMCTVRepository dMCTVRepository
            , IDMBS_ChuyenKhoaRepository chuyenKhoaRepository
            , IDMDVRepository dMDVRepository
            )

        {
            //_legalRepository = legalRepository;
            _logger = logger;
            _authenticateConnect = authenticateConnect;
            _configuration = configuration;
            _DMCTVRepository = dMCTVRepository;
            _ChuyenKhoaRepository = chuyenKhoaRepository;
            _DMDVRepository = dMDVRepository;
        }

        //[AuthorizeUser(Module = Functions.DMCPView, Permission = PermissionConstant.VIEW)]
        public IActionResult List()
        {
            return View();
        }

        public async Task<IActionResult> Search(int length, int start, DMCTVSearchRequest @base)
        {
            DMCTVSearchResponse data;
            try
            {
                @base.Draw = Request.Query["draw"].ToString().ToInt32();
                @base.RequestPage = DepartmentConst.PublicKey;
                data = await _DMCTVRepository.Search(start, length, @base).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                return Json(new GroupsSearchResponse
                {
                    Code = (int)HttpStatusCode.NoContent,
                    Draw = @base.Draw,
                    Message = "Không tìm thấy dữ liệu yêu cầu!"
                });
            }
            return Json(data);
        }

        public async Task<IActionResult> ExportExcel(DMCTVSearchRequest request)
        {
            var data = await _DMCTVRepository.SearchAll(request).ConfigureAwait(false);
            var mappingHeader = new Dictionary<string, string>
            {
                //["Stt"] = "STT",
                //["PhapNhanId"] = "Id pháp nhân",
                //["PhapNhanName"] = "Tên pháp nhân",
                //["IsActiveName"] = "Trạng thái",
                //["CompanyName"] = "Tên công ty",
                //["TaxNumber"] = "Mã số thuế",
                //["AddressCompany"] = "Địa chỉ"

                ["MaCP"] = "Mã dịch vụ",
                ["TenCP"] = "Tên dịch vụ",
                ["DG"] = "Đơn giá",
                ["MaNhCP"] = "Nhóm dịch vụ",
                ["IsActiveName"] = "Trạng thái"
            };
            var workbook = ExcelService.ExportExcel(mappingHeader, data.Data.Cast<dynamic>().ToList(), "Danh sách dịch vụ");
            using (var memoryStream = new MemoryStream())
            {
                workbook.Save(memoryStream, new OoxmlSaveOptions(SaveFormat.Xlsx));
                memoryStream.Position = 0;
                byte[] sheetData = memoryStream.ToArray();
                return File(sheetData, Core.Contants.HttpContentMediaTypes.XLSX, "Danh_sach_dich_vu.xlsx");
            }
        }

        public async Task<IActionResult> Create(string record = default, string viewMode = default, string From = "")
        {
            //Danh mục chuyên khoa
            Task<Core.DTO.Response.DMBS_ChuyenKhoa.DMBS_ChuyenKhoaSearchResponse> dmChuyenKhoaRespone = _ChuyenKhoaRepository.SearchAll(new Core.DTO.Request.DMBS_ChuyenKhoa.DMBS_ChuyenKhoaSearchRequest() { Ma = "", Ten = "", Status = -1 });
            var specialist = dmChuyenKhoaRespone.Result.Data;
            specialist.Add(new Core.DTO.Response.DMBS_ChuyenKhoa.DMBS_ChuyenKhoaSearchResponseData() { Ma = "-1", Ten = "--Chọn--" });
            var specicalistRespone = new SelectList(specialist, "Ma", "Ten", "-1");
            ViewBag.specicalistRespone = specicalistRespone;

            //Danh mục đơn vị quản lý
            List<DMDVSearchResponseData> units = new List<DMDVSearchResponseData>();
            units = await _DMDVRepository.GetCategory().ConfigureAwait(true);
            units.Insert(0, new DMDVSearchResponseData() { MaDV = "-1", TenDV = "--Chọn--" });
            var unitRespone = new SelectList(units, "MaDV", "TenDV", "-1");
            ViewBag.unitRespone = unitRespone;

            //Danh mục tỉnh
            List<ProvinceRespone> provinces = new List<ProvinceRespone>();
            provinces = await _DMCTVRepository.GetProvince().ConfigureAwait(false);
            provinces.Insert(0, new ProvinceRespone() { ProvinceCode = "-1", ProvinceName = "--Chọn--" });
            var provinceRespone = new SelectList(provinces, "ProvinceCode", "ProvinceName", "-1");
            ViewBag.provinceRespone = provinceRespone;

            //Danh mục huyện
            List<DistrictRespone> districts = new List<DistrictRespone>();
            districts = await _DMCTVRepository.GetDistrict().ConfigureAwait(false);
            districts.Insert(0, new DistrictRespone() { DistrictCode = "-1", DistrictName = "--Chọn--" });
            var districtRespone = new SelectList(districts, "DistrictCode", "DistrictName", "-1");
            ViewBag.districtRespone = districtRespone;

            //Danh mục tỉnh công ty
            List<ProvinceRespone> provinceCompanys = new List<ProvinceRespone>();
            provinceCompanys = await _DMCTVRepository.GetProvince().ConfigureAwait(false);
            provinceCompanys.Insert(0, new ProvinceRespone() { ProvinceCode = "-1", ProvinceName = "--Chọn--" });
            var provinceCompanyRespone = new SelectList(provinceCompanys, "ProvinceCode", "ProvinceName", "-1");
            ViewBag.provinceCompanyRespone = provinceCompanyRespone;

            //Danh mục huyện công ty
            List<DistrictRespone> districtCompanys = new List<DistrictRespone>();
            districtCompanys = await _DMCTVRepository.GetDistrict().ConfigureAwait(false);
            districtCompanys.Insert(0, new DistrictRespone() { DistrictCode = "-1", DistrictName = "--Chọn--" });
            var districtCompanyRespone = new SelectList(districtCompanys, "DistrictCode", "DistrictName", "-1");
            ViewBag.districtCompanyRespone = districtCompanyRespone;

            //Danh mục chức danh
            List<JobTitleRespone> jobTitles = new List<JobTitleRespone>();
            jobTitles = await _DMCTVRepository.GetJobTitle().ConfigureAwait(false);
            jobTitles.Insert(0, new JobTitleRespone() { JobTitleCode = "-1", JobTitleName = "--Chọn--" });
            var jobTitleRespone = new SelectList(jobTitles, "JobTitleCode", "JobTitleName", "-1");
            ViewBag.jobTitleRespone = jobTitleRespone;

            //Danh mục đối tượng
            List<PartnerObjectRespone> partnerObjects = new List<PartnerObjectRespone>();
            partnerObjects = await _DMCTVRepository.GetPartnerObject().ConfigureAwait(false);
            partnerObjects.Insert(0, new PartnerObjectRespone() { PartnerObjectCode = "-1", PartnerObjectName = "--Chọn--" });
            var partnerObjectRespone = new SelectList(partnerObjects, "PartnerObjectCode", "PartnerObjectName", "-1");
            ViewBag.partnerObjectRespone = partnerObjectRespone;

            //Danh mục chiết khấu
            List<DiscountRespone> discounts = new List<DiscountRespone>();
            discounts = await _DMCTVRepository.GetDiscount().ConfigureAwait(false);
            discounts.Insert(0, new DiscountRespone() { DiscountCode = "-1", DiscountName = "--Chọn--" });
            var discountRespone = new SelectList(discounts, "DiscountCode", "DiscountName", "-1");
            ViewBag.discountRespone = discountRespone;

            DMCTVSearchResponseData model = new DMCTVSearchResponseData();
            model.From = From;
            return View(model);
        }

        public async Task<IActionResult> OnCreateDMCTV(DMCTVCreateRequest request)
        {
            DMCTVCreateResponse response = new DMCTVCreateResponse();
            response = await _DMCTVRepository.Create(request);

            return Json(response);
        }
       
    }
}