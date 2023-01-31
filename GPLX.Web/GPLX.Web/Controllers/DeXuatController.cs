using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.LoaiDeXuat;
using GPLX.Core.DTO.Request.LoaiDeXuat;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using GPLX.Web.Models.Dashboard;
using GPLX.Core.Contracts.DeXuat;
using GPLX.Core.DTO.Request.DeXuat;
using GPLX.Core.DTO.Response.DeXuat;
using GPLX.Core.Contracts.DeXuatChiTiet;
using GPLX.Core.DTO.Request.DeXuatChiTiet;
using GPLX.Core.Contracts.DMCTV;
using GPLX.Core.DTO.Request.DMCVT;
using GPLX.Core.DTO.Request.DMCP;
using GPLX.Core.Contracts.DMCP;
using GPLX.Core.DTO.Request.DMDV;
using GPLX.Core.Contracts.DMDV;
using System.Collections.Generic;
using GPLX.Infrastructure.Services;
using System.IO;
using Aspose.Cells;
using GPLX.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using ContentDispositionHeaderValue = System.Net.Http.Headers.ContentDispositionHeaderValue;
using HttpContentMediaTypes = GPLX.Core.Contants.HttpContentMediaTypes;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;
using System.IO;
using System.Linq;
using Aspose.Cells;
using Fluid;
using static GPLX.Infrastructure.Constants.AppConstant.TemplatePath;
using GPLX.Core.Contracts.User;
using GPLX.Core.DTO.Request.Users;
using GPLX.Core.DTO.Request.DeXuatKhoaMaCTV;
using GPLX.Core.Contracts.DeXuatKhoaMaCTV;
using GPLX.Core.DTO.Request.DeXuatLuanChuyenMa;
using GPLX.Core.Contracts.DeXuatLuanChuyenMa;
using GPLX.Core.DTO.Response.DeXuatLuanChuyenMa;
using GPLX.Core.DTO.Response.DeXuatKhoaMaCTV;
using GPLX.Core.DTO.Response.Users;
using GPLX.Core.DTO.Response.LoaiDeXuat;
using GPLX.Core.Contracts.ProcessStep;
using GPLX.Core.Contracts.DeXuatGhiChu;
using GPLX.Core.DTO.Request.DeXuatGhiChu;
using Microsoft.Extensions.Hosting;

namespace GPLX.Web.Controllers
{
    public class DeXuatController : BaseController
    {
        private readonly IDMCTVRepository _dmctvRepository;
        private readonly IDMDVRepository _dmdvRepository;
        private readonly IDeXuatRepository _deXuatRepository;
        private readonly IDeXuatChiTietRepository _deXuatChiTietRepository;
        private readonly IDeXuatKhoaMaCTVRepository _deXuatKhoaMaCTVRepository;
        private readonly IDeXuatLuanChuyenMaRepository _deXuatLuanChuyenMaRepository;
        private readonly IDMCPRepository _dmcpRepository;
        private readonly ILoaiDeXuatRepository _loaiDeXuatRepository;
        private readonly ILogger<DeXuatController> _logger;
        private readonly IMedAuthenticateConnect _authenticateConnect;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IProcessStepRepository _processStepRepository;
        private readonly IDeXuatGhiChuRepository _dexuatGhichuRepository;
        private readonly IHostingEnvironment _hostingEnvironment;

        public DeXuatController(
            IHostingEnvironment hostingEnvironment,
            IDMCTVRepository dMCTVRepository,
            IUserRepository userRepository,
            IDMDVRepository dmdvRepository,
            IDeXuatRepository deXuatRepository,
            IProcessStepRepository processStepRepository,
            IDeXuatChiTietRepository deXuatChiTietRepository,
            IDeXuatKhoaMaCTVRepository deXuatKhoaMaCTVRepository,
            IDeXuatLuanChuyenMaRepository deXuatLuanChuyenMaRepository,
            IDMCPRepository dmcpRepository,
            ILoaiDeXuatRepository loaiDeXuatRepository,
            ILogger<DeXuatController> logger,
            IMedAuthenticateConnect authenticateConnect,
            IDeXuatGhiChuRepository dexuatGhichuRepository,
            IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _userRepository = userRepository;
            _dmdvRepository = dmdvRepository;
            _dmctvRepository = dMCTVRepository;
            _deXuatRepository = deXuatRepository;
            _deXuatChiTietRepository = deXuatChiTietRepository;
            _deXuatKhoaMaCTVRepository = deXuatKhoaMaCTVRepository;
            _deXuatLuanChuyenMaRepository = deXuatLuanChuyenMaRepository;
            _dmcpRepository = dmcpRepository;
            _loaiDeXuatRepository = loaiDeXuatRepository;
            _logger = logger;
            _authenticateConnect = authenticateConnect;
            _configuration = configuration;
            _processStepRepository = processStepRepository;
            _dexuatGhichuRepository = dexuatGhichuRepository;
        }

        public async Task<IActionResult> List()
        {
            var searchModel = new DeXuatFilterModel();

            var idRole = GetProcessRoleSessionModel();
            searchModel.ChoDuyet = await _deXuatRepository.ChoDuyet(idRole);
            searchModel.DaDuyet = await _deXuatRepository.DaDuyet(idRole);
            searchModel.HoanThanh = await _deXuatRepository.HoanThanh(idRole);
            searchModel.QuaHan = await _deXuatRepository.QuaHan(idRole);

            LoaiDeXuatSearchRequest requestloaidexuat = new LoaiDeXuatSearchRequest();
            requestloaidexuat.Status = 1;
            var ListLoaiDeXuat = await _loaiDeXuatRepository.SearchAll(requestloaidexuat);
            LoaiDeXuatSearchResponseData itemLDX = new LoaiDeXuatSearchResponseData();
            itemLDX.LoaiDeXuatName = "Tất cả";
            itemLDX.LoaiDeXuatCode = "-1";
            ListLoaiDeXuat.Data.Insert(0,itemLDX);

            var lstLoaiDeXuat = new SelectList(ListLoaiDeXuat.Data, "LoaiDeXuatCode", "LoaiDeXuatName", "-1");
            searchModel.LstLoaiDeXuat = lstLoaiDeXuat;

            UsersSearchRequest requestUser = new UsersSearchRequest();
            //requestUser.Status = 1;
            var ListUsers = await _userRepository.SearchAll(requestUser);

            UsersSearchResponseData itemUserDefault = new UsersSearchResponseData();
            itemUserDefault.UserName = "Tất cả";
            itemUserDefault.UserId = "Tất cả";
            ListUsers.Data.Insert(0, itemUserDefault);

            var lstUsers = new SelectList(ListUsers.Data, "UserId", "UserId", "-1");
            searchModel.LstUsers = lstUsers;

            return View(searchModel);
        }

        public async Task<IActionResult> Search(int length, int start, DeXuatSearchRequest @base)
        {
            DeXuatSearchResponse data = new DeXuatSearchResponse();

            @base.Draw = Request.Query["draw"].ToString().ToInt32();
            @base.RequestPage = DepartmentConst.PublicKey;
            @base.MaDonViDeXuat = GetUserUnit().UnitCode;
            @base.IDRole = GetProcessRoleSessionModel();
            data = await _deXuatRepository.Search(start, length, @base);

            return Json(data);
        }

        public async Task<IActionResult> CreateDeXuatCTV(string DeXuat = "", int Type = 0)
        {
            DeXuatTaoMaFilterModel modelFilter = new DeXuatTaoMaFilterModel();
            DeXuatSearchResponseData dexuat = await _deXuatRepository.GetByCode(DeXuat);
            modelFilter.DeXuatSearchResponseData = dexuat;

            if (string.IsNullOrEmpty(DeXuat))
            {
                DeXuatSearchResponseData deXuatSearchResponseData = new DeXuatSearchResponseData();
                deXuatSearchResponseData.ThoiGianKhoa = System.DateTime.Now;
                deXuatSearchResponseData.MaBacsi = "-1";

                modelFilter.DeXuatSearchResponseData = deXuatSearchResponseData;
            }

            DMCTVSearchRequest request = new DMCTVSearchRequest();
            request.Status = -1;
            var dmctv = await _dmctvRepository.SearchAll(request);
            modelFilter.DMCTVSearchResponse = dmctv;

            var ctv = new SelectList(dmctv.Data, "MaBS", "TenBS", "-1");
            modelFilter.ListCTV = ctv;
            modelFilter.IDRole = GetProcessRoleSessionModel();
            modelFilter.MaDeXuat = DeXuat;
            modelFilter.ProcessId = 4;

            return View(modelFilter);
        }

        public async Task<IActionResult> CreateDeXuatKhoaMa(string DeXuat = "")
        {
            DeXuatTaoMaFilterModel modelFilter = new DeXuatTaoMaFilterModel();

            DeXuatSearchResponseData dexuat = await _deXuatRepository.GetByCode(DeXuat);
            modelFilter.DeXuatSearchResponseData = dexuat;

            DMCTVSearchRequest request = new DMCTVSearchRequest();
            request.Status = -1;
            var dmctv = await _dmctvRepository.SearchAll(request);
            modelFilter.DMCTVSearchResponse = dmctv;

            var ctv = new SelectList(dmctv.Data, "MaBS", "TenBS", "-1");
            modelFilter.ListCTV = ctv;

            modelFilter.IDRole = GetProcessRoleSessionModel();
            modelFilter.MaDeXuat = DeXuat;
            modelFilter.ProcessId = 6;

            DMCPSearchRequest requestdmcp = new DMCPSearchRequest();
            requestdmcp.Status = 1;
            var ListDMCP = await _dmcpRepository.SearchAll(requestdmcp);

            var lstDMCP = new SelectList(ListDMCP.Data, "MaCP", "TenCP", "-1");
            modelFilter.ListDMCP = lstDMCP;

            DMDVSearchRequest requestdmdv = new DMDVSearchRequest();
            var ListDMDV = await _dmdvRepository.SearchAll(requestdmdv);

            var lstDMDV = new SelectList(ListDMDV.Data, "MaDV", "TenDV", "-1");
            modelFilter.ListDMDV = lstDMDV;

            if(!string.IsNullOrEmpty(DeXuat))
            {
                DeXuatKhoaMaCTVSearchRequest requestDeXuatKhoaMaCTV = new DeXuatKhoaMaCTVSearchRequest();
                requestDeXuatKhoaMaCTV.DeXuatCode = DeXuat;
                modelFilter.DeXuatKhoaMaCTVSearchResponse = await _deXuatKhoaMaCTVRepository.Search(requestDeXuatKhoaMaCTV);
            }
            else
            {
                modelFilter.DeXuatKhoaMaCTVSearchResponse = new DeXuatKhoaMaCTVSearchResponse();
            }

            modelFilter.Permission = GetPermission(modelFilter, dexuat);

            return View(modelFilter);
        }
        public async Task<IActionResult> CreateDeXuatMoMaCTV(string DeXuat = "")
        {
            DeXuatTaoMaFilterModel modelFilter = new DeXuatTaoMaFilterModel();
            DeXuatSearchResponseData dexuat = await _deXuatRepository.GetByCode(DeXuat);
            modelFilter.DeXuatSearchResponseData = dexuat;

            if (string.IsNullOrEmpty(DeXuat))
            {
                DeXuatSearchResponseData deXuatSearchResponseData = new DeXuatSearchResponseData();
                deXuatSearchResponseData.ThoiGianKhoa = System.DateTime.Now;
                deXuatSearchResponseData.MaBacsi = "-1";

                modelFilter.DeXuatSearchResponseData = deXuatSearchResponseData;
            }   

            DMCTVSearchRequest request = new DMCTVSearchRequest();
            request.Status = -1;
            var dmctv = await _dmctvRepository.SearchAll(request);
            modelFilter.DMCTVSearchResponse = dmctv;

            var ctv = new SelectList(dmctv.Data, "MaBS", "TenBS", "-1");
            modelFilter.ListCTV = ctv;

            modelFilter.IDRole = GetProcessRoleSessionModel();
            modelFilter.MaDeXuat = DeXuat;
            modelFilter.ProcessId = 11;

            DMCPSearchRequest requestdmcp = new DMCPSearchRequest();
            requestdmcp.Status = 1;
            var ListDMCP = await _dmcpRepository.SearchAll(requestdmcp);

            var lstDMCP = new SelectList(ListDMCP.Data, "MaCP", "TenCP", "-1");
            modelFilter.ListDMCP = lstDMCP;

            DMDVSearchRequest requestdmdv = new DMDVSearchRequest();
            var ListDMDV = await _dmdvRepository.SearchAll(requestdmdv);

            var lstDMDV = new SelectList(ListDMDV.Data, "MaDV", "TenDV", "-1");
            modelFilter.ListDMDV = lstDMDV;

            modelFilter.Permission = GetPermission(modelFilter, dexuat);

            return View(modelFilter);
        }

        public async Task<IActionResult> CreateDeXuatLuanChuyenMa(string DeXuat = "")
        {
            DeXuatTaoMaFilterModel modelFilter = new DeXuatTaoMaFilterModel();

            DeXuatSearchResponseData dexuat = await _deXuatRepository.GetByCode(DeXuat);
            modelFilter.DeXuatSearchResponseData = dexuat;

            DMCTVSearchRequest request = new DMCTVSearchRequest();
            request.Status = -1;
            var dmctv = await _dmctvRepository.SearchAll(request);
            modelFilter.DMCTVSearchResponse = dmctv;
            
            var ctv = new SelectList(dmctv.Data, "MaBS", "TenBS", "-1");
            modelFilter.ListCTV = ctv;

            modelFilter.IDRole = GetProcessRoleSessionModel();
            modelFilter.MaDeXuat = DeXuat;
            modelFilter.ProcessId = 12;

            DMCPSearchRequest requestdmcp = new DMCPSearchRequest();
            requestdmcp.Status = 1;
            var ListDMCP = await _dmcpRepository.SearchAll(requestdmcp);

            var lstDMCP = new SelectList(ListDMCP.Data, "MaCP", "TenCP", "-1");
            modelFilter.ListDMCP = lstDMCP;

            DMDVSearchRequest requestdmdv = new DMDVSearchRequest();
            requestdmdv.Status = 1;
            var ListDMDV = await _dmdvRepository.SearchAll(requestdmdv);

            var lstDMDV = new SelectList(ListDMDV.Data, "MaDV", "TenDV", "-1");
            modelFilter.ListDMDV = lstDMDV;

            if (!string.IsNullOrEmpty(DeXuat))
            {
                DeXuatLuanChuyenMaSearchRequest requestDeXuatLuanChuyenMa = new DeXuatLuanChuyenMaSearchRequest();
                requestDeXuatLuanChuyenMa.DeXuatCode = DeXuat;
                modelFilter.DeXuatLuanChuyenMaSearchResponse = await _deXuatLuanChuyenMaRepository.Search(requestDeXuatLuanChuyenMa);
            }
            else
            {
                modelFilter.DeXuatLuanChuyenMaSearchResponse = new DeXuatLuanChuyenMaSearchResponse();
            }

            modelFilter.Permission = GetPermission(modelFilter, dexuat);

            return View(modelFilter);
        }

        public async Task<IActionResult> OnCreateDeXuat(DeXuatCreateRequest request)
        {
            request.Creator = GetUserId();
            request.CreatorName = GetUserSyncId();
            request.MaDonViDeXuat = GetUserUnit().UnitCode;
            request.TenCongTy = GetUserUnit().UnitName;
            var response = await _deXuatRepository.Create(request);

            return Json(response);
        }
        public async Task<IActionResult> OnCreateDeXuatChiTiet(DeXuatChiTietCreateRequest request)
        {
            request.Creator = GetUserId();
            request.CreatorName = GetUserSyncId();
            var response = await _deXuatChiTietRepository.Create(request);

            return Json(response);
        }

        public async Task<IActionResult> OnRemoveDeXuatChiTiet(DeXuatChiTietCreateRequest request)
        {
            request.Creator = GetUserId();
            request.CreatorName = GetUserSyncId();
            var response = await _deXuatChiTietRepository.Remove(request);

            return Json(response);
        }
        public async Task<IActionResult> OnCreateDeXuatKhoaMaCTV(DeXuatKhoaMaCTVCreateRequest request)
        {
            request.Creator = GetUserId();
            request.CreatorName = GetUserSyncId();
            var response = await _deXuatKhoaMaCTVRepository.Create(request);

            return Json(response);
        }
        public async Task<IActionResult> OnRemoveDeXuatKhoaMaCTV(DeXuatKhoaMaCTVCreateRequest request)
        {
            request.Creator = GetUserId();
            request.CreatorName = GetUserSyncId();
            var response = await _deXuatKhoaMaCTVRepository.Remove(request);

            return Json(response);
        }
        public async Task<IActionResult> OnCreateDeXuatLuanChuyenMa(DeXuatLuanChuyenMaCreateRequest request)
        {
            request.Creator = GetUserId();
            request.CreatorName = GetUserSyncId();
            var response = await _deXuatLuanChuyenMaRepository.Create(request);

            return Json(response);
        }

        public async Task<IActionResult> OnRemoveDeXuatLuanChuyenMa(DeXuatLuanChuyenMaCreateRequest request)
        {
            request.Creator = GetUserId();
            request.CreatorName = GetUserSyncId();
            var response = await _deXuatLuanChuyenMaRepository.Remove(request);

            return Json(response);
        }
        public async Task<IActionResult> PushDeXuat(DeXuatCreateRequest request)
        {
            request.Creator = GetUserId();
            request.CreatorName = GetUserSyncId();

            request.CreateGhiChuByCode = GetUserSyncId();
            request.CreateGhiChuByName = GetUserName();
            var response = await _deXuatRepository.PushDeXuat(request);

            return Json(response);
        }
        public async Task<IActionResult> RejectDeXuat(DeXuatCreateRequest request)
        {
            request.Creator = GetUserId();
            request.CreatorName = GetUserSyncId();
            request.CreateGhiChuByCode = GetUserSyncId();
            request.CreateGhiChuByName = GetUserName();
            var response = await _deXuatRepository.RejectDeXuat(request);

            return Json(response);
        }
        
        public async Task<IActionResult> ExportPdf(string dexuat)
        {
            DeXuatSearchResponseData itemdx = await _deXuatRepository.GetByCode(dexuat);
            var processId = itemdx.ProcessId;
            var processStepConfigs = await _processStepRepository.GetProcessStepDetail(processId);
            var actualSteps = await _dexuatGhichuRepository.FindAllByCode(dexuat);
            var projectRootPath = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot", "metadata", "PdfTemplates");
            
            List<ApprovedUserInfo>  approvedUsers = new List<ApprovedUserInfo>();
            if (AppConstant.DeXuatCode.MO_MA.Equals(itemdx.LoaiDeXuatCode))
            {
                for(int i=0; i < actualSteps.Count(); i++)
                {
                    var step = actualSteps[i];
                    var approvedUserInfo = new ApprovedUserInfo
                    {
                        Role = "", //TODO: thieu role, thieu chu ky
                        FullName = step.CreateByName,
                        ApprovedTime = step.CreateDate == null ? "" : step.CreateDate.Value.ToString("dd/MM/yyyy HH:mm"),
                        Note = step.Note,
                        Signature = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b5/Nguy%E1%BB%85n_V%C4%83n_B%C3%ACnh_%2C_Nguyen_Van_Binh_signature.png/1200px-Nguy%E1%BB%85n_V%C4%83n_B%C3%ACnh_%2C_Nguyen_Van_Binh_signature.png",
                    };
                    approvedUsers.Add(approvedUserInfo);
                }
            }

            if (itemdx.LoaiDeXuatCode.Equals("DeXuatKhoaMa"))
            {
                var deXuat = new DeXuat
                {
                    Stt = itemdx.Index,
                    MaBacSi = itemdx.MaBacsi,
                    TenBacSi = itemdx.TenBacsi,
                    ThoiGianKhoa = itemdx.ThoiGianKhoa == null ? "" : itemdx.ThoiGianKhoa.Value.ToString("dd/MM/yyyy HH:mm"),
                    LyDoKhoa = itemdx.LyDoKhoa
                };
                var parseData = new ExportPdfDeXuatKhoaMa
                {
                    CompanyName = itemdx.TenCongTy,
                    DeXuatNumber = itemdx.TenDeXuatCode,
                    DsDeXuat = new List<DeXuat> { deXuat },
                    ApprovedUsers = approvedUsers,
                    SentToUsers = string.Join(", </br> ", processStepConfigs.Where(x => x.IsLastStep == false).Select(x => x.ProcessRoleName)),
                    SubmitToUsers = processStepConfigs.Where(x => x.IsLastStep == true).FirstOrDefault().ProcessRoleName
                };

                var templateOptions = new TemplateOptions();
                templateOptions.MemberAccessStrategy.Register<DeXuat>();
                templateOptions.MemberAccessStrategy.Register<ApprovedUserInfo>();
                templateOptions.MemberAccessStrategy.Register<ExportPdfDeXuatKhoaMa>();
                using (var memoryStream = new MemoryStream())
                {
                    await PdfService.ExportPdfFromFilePath(projectRootPath + DE_XUAT_KHOA_MA_CTVBS, parseData, memoryStream, templateOptions).ConfigureAwait(false);
                    byte[] pdfData = memoryStream.ToArray();
                    return File(pdfData, HttpContentMediaTypes.Pdf, "DeXuatKhoaMa.pdf");
                }
            }   
            else if (itemdx.LoaiDeXuatCode.Equals("DeXuatMoMa"))
            {
                var deXuat = new DeXuat
                {
                    Stt = itemdx.Index,
                    MaBacSi = itemdx.MaBacsi,
                    TenBacSi = itemdx.TenBacsi,
                    ThoiGianKhoa = itemdx.ThoiGianKhoa == null ? "" : itemdx.ThoiGianKhoa.Value.ToString("dd/MM/yyyy HH:mm"),
                    LyDoKhoa = itemdx.LyDoKhoa
                };
                var parseData = new ExportPdfDeXuatKhoaMa
                {
                    CompanyName = itemdx.TenCongTy,
                    DeXuatNumber = itemdx.TenDeXuatCode,
                    DsDeXuat = new List<DeXuat> { deXuat },
                    ApprovedUsers = approvedUsers,
                    SentToUsers = string.Join(", </br> ", processStepConfigs.Where(x => x.IsLastStep == false).Select(x => x.ProcessRoleName)),
                    SubmitToUsers = processStepConfigs.Where(x => x.IsLastStep == true).FirstOrDefault().ProcessRoleName
                };

                var templateOptions = new TemplateOptions();
                templateOptions.MemberAccessStrategy.Register<DeXuat>();
                templateOptions.MemberAccessStrategy.Register<ApprovedUserInfo>();
                templateOptions.MemberAccessStrategy.Register<ExportPdfDeXuatKhoaMa>();
                using (var memoryStream = new MemoryStream())
                {
                    await PdfService.ExportPdfFromFilePath(projectRootPath + DE_XUAT_MO_MA_CTVBS, parseData, memoryStream, templateOptions).ConfigureAwait(false);
                    byte[] pdfData = memoryStream.ToArray();
                    return File(pdfData, HttpContentMediaTypes.Pdf, "DeXuatMoMa.pdf");
                }
            }
            else if (itemdx.LoaiDeXuatCode.Equals("DeXuatLuanChuyenMa"))
            {
                var deXuat = new DeXuat
                {
                    Stt = itemdx.Index,
                    MaBacSi = itemdx.MaBacsi,
                    TenBacSi = itemdx.TenBacsi,
                    ThoiGianKhoa = itemdx.ThoiGianKhoa == null ? "" : itemdx.ThoiGianKhoa.Value.ToString("dd/MM/yyyy HH:mm"),
                    LyDoKhoa = itemdx.LyDoKhoa
                };
                var parseData = new ExportPdfDeXuatKhoaMa
                {
                    CompanyName = itemdx.TenCongTy,
                    DeXuatNumber = itemdx.TenDeXuatCode,
                    DsDeXuat = new List<DeXuat> { deXuat },
                    ApprovedUsers = approvedUsers,
                    SentToUsers = string.Join(", </br> ", processStepConfigs.Where(x => x.IsLastStep == false).Select(x => x.ProcessRoleName)),
                    SubmitToUsers = processStepConfigs.Where(x => x.IsLastStep == true).FirstOrDefault().ProcessRoleName
                };

                var templateOptions = new TemplateOptions();
                templateOptions.MemberAccessStrategy.Register<DeXuat>();
                templateOptions.MemberAccessStrategy.Register<ApprovedUserInfo>();
                templateOptions.MemberAccessStrategy.Register<ExportPdfDeXuatKhoaMa>();
                using (var memoryStream = new MemoryStream())
                {
                    await PdfService.ExportPdfFromFilePath(projectRootPath + DE_XUAT_LC_MA_CTVBS, parseData, memoryStream, templateOptions).ConfigureAwait(false);
                    byte[] pdfData = memoryStream.ToArray();
                    return File(pdfData, HttpContentMediaTypes.Pdf, "DeXuatLuanChuyenMa.pdf");
                }
            }    
            else if (itemdx.LoaiDeXuatCode.Equals("DeXuatSuaMa"))
            {
                var deXuat = new DeXuat
                {
                    Stt = itemdx.Index,
                    MaBacSi = itemdx.MaBacsi,
                    TenBacSi = itemdx.TenBacsi,
                    ThoiGianKhoa = itemdx.ThoiGianKhoa == null ? "" : itemdx.ThoiGianKhoa.Value.ToString("dd/MM/yyyy HH:mm"),
                    LyDoKhoa = itemdx.LyDoKhoa
                };
                var parseData = new ExportPdfDeXuatKhoaMa
                {
                    CompanyName = itemdx.TenCongTy,
                    DeXuatNumber = itemdx.TenDeXuatCode,
                    DsDeXuat = new List<DeXuat> { deXuat },
                    ApprovedUsers = approvedUsers,
                    SentToUsers = string.Join(", </br> ", processStepConfigs.Where(x => x.IsLastStep == false).Select(x => x.ProcessRoleName)),
                    SubmitToUsers = processStepConfigs.Where(x => x.IsLastStep == true).FirstOrDefault().ProcessRoleName
                };

                var templateOptions = new TemplateOptions();
                templateOptions.MemberAccessStrategy.Register<DeXuat>();
                templateOptions.MemberAccessStrategy.Register<ApprovedUserInfo>();
                templateOptions.MemberAccessStrategy.Register<ExportPdfDeXuatKhoaMa>();
                using (var memoryStream = new MemoryStream())
                {
                    await PdfService.ExportPdfFromFilePath(projectRootPath + DE_XUAT_SUA_MA_CTVBS, parseData, memoryStream, templateOptions).ConfigureAwait(false);
                    byte[] pdfData = memoryStream.ToArray();
                    return File(pdfData, HttpContentMediaTypes.Pdf, "DeXuatSuaMa.pdf");
                }
            }    
            else if (itemdx.LoaiDeXuatCode.Equals("DeXuatTaoMa"))
            {
                var deXuat = new DeXuat
                {
                    Stt = itemdx.Index,
                    MaBacSi = itemdx.MaBacsi,
                    TenBacSi = itemdx.TenBacsi,
                    ThoiGianKhoa = itemdx.ThoiGianKhoa == null ? "" : itemdx.ThoiGianKhoa.Value.ToString("dd/MM/yyyy HH:mm"),
                    LyDoKhoa = itemdx.LyDoKhoa
                };
                var parseData = new ExportPdfDeXuatKhoaMa
                {
                    CompanyName = itemdx.TenCongTy,
                    DeXuatNumber = itemdx.TenDeXuatCode,
                    DsDeXuat = new List<DeXuat> { deXuat },
                    ApprovedUsers = approvedUsers,
                    SentToUsers = string.Join(", </br> ", processStepConfigs.Where(x => x.IsLastStep == false).Select(x => x.ProcessRoleName)),
                    SubmitToUsers = processStepConfigs.Where(x => x.IsLastStep == true).FirstOrDefault().ProcessRoleName
                };

                var templateOptions = new TemplateOptions();
                templateOptions.MemberAccessStrategy.Register<DeXuat>();
                templateOptions.MemberAccessStrategy.Register<ApprovedUserInfo>();
                templateOptions.MemberAccessStrategy.Register<ExportPdfDeXuatKhoaMa>();
                using (var memoryStream = new MemoryStream())
                {
                    await PdfService.ExportPdfFromFilePath(projectRootPath + DE_XUAT_TAO_MA_CTVBS, parseData, memoryStream, templateOptions).ConfigureAwait(false);
                    byte[] pdfData = memoryStream.ToArray();
                    return File(pdfData, HttpContentMediaTypes.Pdf, "DeXuatTaoMa.pdf");
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<IActionResult> ExportExcel(DeXuatSearchRequest @base)
        {
            DeXuatSearchResponse data = new DeXuatSearchResponse();

            @base.Draw = Request.Query["draw"].ToString().ToInt32();
            @base.RequestPage = DepartmentConst.PublicKey;
            @base.MaDonViDeXuat = GetUserUnit().UnitCode;
            @base.IDRole = GetProcessRoleSessionModel();
            data = await _deXuatRepository.SearchAll(@base).ConfigureAwait(false);

            var mappingHeader = new Dictionary<string, string>
            {
                ["Index"] = "STT",
                ["DeXuatCode"] = "Mã đề xuất",
                ["DeXuatName"] = "Tên đề xuất",
                ["TenDeXuatCode"] = "Loại đề xuất",
                ["TrangThai"] = "Trạng thái",
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

        public async Task<IActionResult> OnRemove(DeXuatCreateRequest request)
        {
            var response = await _deXuatRepository.Remove(request);

            return Json(response);
        }

        public int GetPermission(DeXuatTaoMaFilterModel modelFilter, DeXuatSearchResponseData dexuat)
        {
            //_permission
            //-1: Delete
            //0: Tạo mới hoặc chỉnh sửa, chưa đẩy luồng duyệt
            //1: Done
            //2: Không có quyền tương tác
            //3: Chờ duyệt

            int _permission = 0;
            if (modelFilter.IDRole != 0)
            {
                if (dexuat != null)
                {
                    if (dexuat.IsDone == 1)
                        _permission = 1;
                    else if (dexuat.IsDone == -1)
                        _permission = -1;
                    else
                    {
                        if (dexuat.ProcessStepId == 0)
                            _permission = 0;
                        else if (dexuat.ProcessStepId == modelFilter.IDRole)
                            _permission = 3;
                        else
                            _permission = 2;
                    }
                }
                else
                {
                    _permission = 0;
                }
            }
            else
            {
                _permission = 2;
            }
            return _permission;
        }
    }

    public class ExportPdfDeXuatKhoaMa
    {
        public string CompanyName { get; set; }
        public string DeXuatNumber { get; set; }
        public IList<DeXuat> DsDeXuat { get; set; }
        public string SentToUsers { get; set; }
        public string SubmitToUsers { get; set; }
        public IList<ApprovedUserInfo> ApprovedUsers { get; set; }
    }

    public class DeXuat
    {
        public int Stt { get; set; }
        public string MaBacSi { get; set; }
        public string TenBacSi { get; set; }
        public string DonViCu { get; set; }
        public string DonViMoi { get; set; }
        public string ThoiGianKhoa { get; set; }
        public string ThoiGianMo { get; set; }
        public string LyDoKhoa { get; set; }
        public string LyDoMo { get; set; }
    }

    public class ApprovedUserInfo
    {
        public string FullName { get; set; }
        public string Signature { get; set; }
        public string Role { get; set; }
        public string ApprovedTime { get; set; }
        public string Note { get; set; }
    }
}
