using AutoMapper;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.DMCTV;
using GPLX.Core.DTO.Request.DMCTV;
using GPLX.Core.DTO.Request.DMCVT;
using GPLX.Core.DTO.Response.DM;
using GPLX.Core.DTO.Response.DMCTV;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPLX.Core.Data.DMCTV
{
    public class DMCTVRepository : IDMCTVRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DMCTVRepository> _logger;
        private readonly IActionLogsRepository _actionLogsRepository;

        public DMCTVRepository(Context context, IMapper mapper, ILogger<DMCTVRepository> logger, IActionLogsRepository actionLogsRepository)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _actionLogsRepository = actionLogsRepository;
        }

        public async Task<List<DiscountRespone>> GetDiscount()
        {
            var query = _context.DMGG.AsQueryable();
            var respone = await query.Select(x => new DiscountRespone()
            {
                DiscountCode = x.MaGG,
                DiscountName = x.TenGG
            }).ToListAsync().ConfigureAwait(true);
            return respone;
        }

        public async Task<List<DistrictRespone>> GetDistrict(string provinceCode = "", string districtCode = "")
        {
            var query = _context.DMHuyen.AsQueryable();
            if (!string.IsNullOrEmpty(districtCode))
            {
                query = query.Where(x => x.MaHuyen.ToLower().Contains(districtCode.ToLower()));
            }
            if (!string.IsNullOrEmpty(provinceCode))
            {
                query = query.Where(x => x.MaTinh.ToLower().Contains(provinceCode.ToLower()));
            }
            var respone = await query.Select(x => new DistrictRespone()
            {
                DistrictCode = x.MaHuyen,
                DistrictName = x.TenHuyen
            }).ToListAsync().ConfigureAwait(true);
            return respone;
        }

        public async Task<List<JobTitleRespone>> GetJobTitle()
        {
            var query = _context.DM.AsQueryable().Where(x => x.IDTree == 81);
            var respone = await query.Select(x => new JobTitleRespone()
            {
                JobTitleCode = x.MaDM,
                JobTitleName = x.TenDM
            }).ToListAsync().ConfigureAwait(true);
            return respone;
        }

        public async Task<List<PartnerObjectRespone>> GetPartnerObject()
        {
            var query = _context.DM.AsQueryable().Where(x => x.IDTree == 83);
            var respone = await query.Select(x => new PartnerObjectRespone()
            {
                PartnerObjectCode = x.MaDM,
                PartnerObjectName = x.TenDM
            }).ToListAsync().ConfigureAwait(true);
            return respone;
        }

        public async Task<List<ProvinceRespone>> GetProvince(string provinceCode = "")
        {
            var query = _context.DM.AsQueryable().Where(x => x.IDTree == 59);
            if (!string.IsNullOrEmpty(provinceCode))
            {
                query = query.Where(x => x.MaDM.ToLower().Contains(provinceCode.ToLower()));
            }
            var respone = await query.Select(x => new ProvinceRespone()
            {
                ProvinceCode = x.MaDM,
                ProvinceName = x.TenDM
            }).ToListAsync().ConfigureAwait(true);
            return respone;
        }

        public async Task<DMCTVSearchResponse> Search(int skip, int length, DMCTVSearchRequest request)
        {
            var response = new DMCTVSearchResponse { Draw = request.Draw };
            try
            {
                var query = _context.DMCTV.AsQueryable();
                if (!string.IsNullOrEmpty(request.MaBS))
                    query = query.Where(x => x.MaBS.ToLower().Contains(request.MaBS.ToLower()));
                if (!string.IsNullOrEmpty(request.Keywords))
                    query = query.Where(x => x.TenBS.ToLower().Contains(request.Keywords.Trim().ToLower()));
                if (request.Status != -1)
                    query = query.Where(x => x.IsActive == request.Status);
                var data = await query.OrderBy(x => x.MaBS).ToListAsync().ConfigureAwait(true);
                response.RecordsFiltered = data.Count;
                response.RecordsTotal = data.Count;
                var dataResponse = new List<DMCTVSearchResponseData>();
                foreach (var d in data.Skip(skip).Take(length))
                {
                    var dMap = _mapper.Map<DMCTVSearchResponseData>(d);
                    dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);

                    dataResponse.Add(dMap);
                }
                response.Data = dataResponse;
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);

                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.NoContentMessage;
            }

            return response;
        }

        public async Task<DMCTVSearchResponse> SearchAll(DMCTVSearchRequest request)
        {
            var response = new DMCTVSearchResponse { Draw = request.Draw };
            try
            {
                var query = _context.DMCTV.AsQueryable();
                if (!string.IsNullOrEmpty(request.MaBS))
                    query = query.Where(x => x.MaBS.ToLower().Contains(request.MaBS.ToLower()));
                if (!string.IsNullOrEmpty(request.Keywords))
                    query = query.Where(x => x.TenBS.ToLower().Contains(request.Keywords.Trim().ToLower()));
                if (request.Status != -1)
                    query = query.Where(x => x.IsActive == request.Status);

                var data = await query.OrderBy(x => x.MaBS).ToListAsync().ConfigureAwait(true);

                response.RecordsFiltered = data.Count;
                response.RecordsTotal = data.Count;
                var dataResponse = new List<DMCTVSearchResponseData>();
                foreach (var d in data)
                {
                    var dMap = _mapper.Map<DMCTVSearchResponseData>(d);
                    dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);
                    //dMap.TenBS = dMap.MaBS + "-" + dMap.TenBS;
                    dataResponse.Add(dMap);
                }
                response.Data = dataResponse;
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);

                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.NoContentMessage;
            }

            return response;
        }

        /// <summary>
        /// Thêm sửa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<DMCTVCreateResponse> Create(DMCTVCreateRequest request)
        {
            var response = new DMCTVCreateResponse();

            response = Validation(request);
            request.MaBS = String.IsNullOrEmpty(request.MaBS) ? "" : request.MaBS.Trim();
            request.TenBS = String.IsNullOrEmpty(request.TenBS) ? "" : request.TenBS.Trim();

            if (response.ListError.Count == 0)
            {
                var query = _context.DMCTV.AsQueryable();
                query = query.Where(x => x.MaBS == request.MaBS);
                var data = await query.OrderBy(x => x.MaBS).ToListAsync();

                if (string.IsNullOrEmpty(request.Record))
                {
                    if (data.Count > 0)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        response.ListError.Add(new ItemError { FieldError = "txtmabs", Message = "Mã bác sĩ đã tồn tại trong hệ thống." });
                    }
                    else
                    {
                        //Add New
                        var item = new Database.Models.DMCTV();
                        item.MaBS = request.MaBS;
                        item.TenBS = request.TenBS;
                        item.NguoiDaiDien = request.NguoiDaiDien;
                        item.NS = request.NS.Value;
                        item.MaChucDanh = request.MaChucDanh;
                        item.ChuyenKhoa = request.ChuyenKhoa;
                        item.DC1 = request.DC1;
                        item.MaTinh = request.MaTinh;
                        item.MaHuyen = request.MaHuyen;
                        item.DC2 = request.DC2;
                        item.MaTinh2 = request.MaTinh2;
                        item.MaHuyen2 = request.MaHuyen2;
                        item.Mobi = request.Mobi;
                        item.Tel = request.Tel;
                        item.CK = request.CK;
                        item.HTCKDoiTuong = request.HTCKDoiTuong;
                        item.CMND = request.CMND;
                        item.NgayCapCMND = request.NgayCapCMND.Value;
                        item.NoiCapCMND = request.NoiCapCMND;
                        item.Email = request.Email;
                        item.SoTK = request.SoTK;
                        item.Bank = request.Bank;
                        item.TenChuTK = request.TenChuTK;
                        item.SoHD = request.SoHD;
                        item.MaDVCT = request.MaDVCT;
                        //item.TenDVCT = request.TenDVCT;
                        item.Fax = request.Fax;
                        item.IsActive = request.IsActive;
                        //item.IsActiveName = request.IsActiveName;
                        item.LyDoIsActive = request.LyDoIsActive;
                        item.TraSau = request.TraSau;
                        item.MaDTCTV = request.MaDTCTV;
                        //item.TenDTCTV = request.TenDTCTV;
                        item.UserWeb = request.UserWeb;
                        item.PassWeb = request.PassWeb;
                        item.ChiNhanh = request.ChiNhanh;
                        //item.TenCN = request.TenCN;
                        item.GhiChu = request.GhiChu;
                        item.MaSoThue = request.MaSoThue;
                        item.ChungChi_So = request.ChungChi_So;
                        item.ChungChi_NgayCap = request.ChungChi_NgayCap == null ? DateTime.Now: request.ChungChi_NgayCap.Value;
                        item.ChungChi_NoiCap = request.ChungChi_NoiCap;
                        item.NgayKyHD = request.NgayKyHD == null ? DateTime.Now : request.NgayKyHD.Value;
                        item.TenVietTat = request.TenVietTat;
                        item.TTDeXuat = request.TTDeXuat;
                        item.GT = request.GT;
                        item.CKKH = request.CKKH;
                        item.KetThucHD = request.KetThucHD == null ? DateTime.Now : request.KetThucHD.Value;
                        item.DaHoanThienHoSo = request.DaHoanThienHoSo;
                        item.BH_Ma_Khoa = request.BH_Ma_Khoa;
                        item.ChuKy = request.ChuKy;
                        item.MaSap = request.MaSap;
                        item.SoPhuLuc = request.SoPhuLuc;
                        item.NgayKyPL = request.NgayKyPL == null ? DateTime.Now : request.NgayKyPL.Value;
                        item.NgayKetThucPL = request.NgayKetThucPL == null ? DateTime.Now : request.NgayKetThucPL.Value;
                        item.Createdate = DateTime.Now;
                        item.Createby = request.CreatorName;
                        _context.DMCTV.Add(item);
                        await _context.SaveChangesAsync();
                        await _actionLogsRepository.AddLogAsync(new ActionLogs
                        {
                            Id = Guid.NewGuid(),
                            CreatedDate = DateTime.Now,
                            UserName = request.CreatorName,
                            Content = JsonConvert.SerializeObject(request),
                            Action = "Add",
                            FunctionUnique = "DMCTV",
                            UserId = request.Creator
                        }).ConfigureAwait(false);
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = "Thêm mới thông tin DMCTV thành công!";
                    }
                }
                else
                {
                    //Edit
                    var item = data.FirstOrDefault();
                    //item.MaBS = request.MaBS;
                    item.TenBS = request.TenBS;
                    item.NguoiDaiDien = request.NguoiDaiDien;
                    item.NS = request.NS.Value;
                    item.MaChucDanh = request.MaChucDanh;
                    item.ChuyenKhoa = request.ChuyenKhoa;
                    item.DC1 = request.DC1;
                    item.MaTinh = request.MaTinh;
                    item.MaHuyen = request.MaHuyen;
                    item.DC2 = request.DC2;
                    item.MaTinh2 = request.MaTinh2;
                    item.MaHuyen2 = request.MaHuyen2;
                    item.Mobi = request.Mobi;
                    item.Tel = request.Tel;
                    item.CK = request.CK;
                    item.HTCKDoiTuong = request.HTCKDoiTuong;
                    item.CMND = request.CMND;
                    item.NgayCapCMND = request.NgayCapCMND.Value;
                    item.NoiCapCMND = request.NoiCapCMND;
                    item.Email = request.Email;
                    item.SoTK = request.SoTK;
                    item.Bank = request.Bank;
                    item.TenChuTK = request.TenChuTK;
                    item.SoHD = request.SoHD;
                    item.MaDVCT = request.MaDVCT;
                    //item.TenDVCT = request.TenDVCT;
                    item.Fax = request.Fax;
                    item.IsActive = request.IsActive;
                    //item.IsActiveName = request.IsActiveName;
                    item.LyDoIsActive = request.LyDoIsActive;
                    item.TraSau = request.TraSau;
                    item.MaDTCTV = request.MaDTCTV;
                    //item.TenDTCTV = request.TenDTCTV;
                    item.UserWeb = request.UserWeb;
                    item.PassWeb = request.PassWeb;
                    item.ChiNhanh = request.ChiNhanh;
                    //item.TenCN = request.TenCN;
                    item.GhiChu = request.GhiChu;
                    item.MaSoThue = request.MaSoThue;
                    item.ChungChi_So = request.ChungChi_So;
                    item.ChungChi_NgayCap = request.ChungChi_NgayCap.Value;
                    item.ChungChi_NoiCap = request.ChungChi_NoiCap;
                    item.NgayKyHD = request.NgayKyHD.Value;
                    item.TenVietTat = request.TenVietTat;
                    item.TTDeXuat = request.TTDeXuat;
                    item.GT = request.GT;
                    item.CKKH = request.CKKH;
                    item.KetThucHD = request.KetThucHD.Value;
                    item.DaHoanThienHoSo = request.DaHoanThienHoSo;
                    item.BH_Ma_Khoa = request.BH_Ma_Khoa;
                    item.ChuKy = request.ChuKy;
                    item.MaSap = request.MaSap;
                    item.SoPhuLuc = request.SoPhuLuc;
                    item.NgayKyPL = request.NgayKyPL.Value;
                    item.NgayKetThucPL = request.NgayKetThucPL.Value;
                    item.Updateby = request.CreatorName;
                    item.Updatedate = DateTime.Now;
                    await _context.SaveChangesAsync();
                    await _actionLogsRepository.AddLogAsync(new ActionLogs
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        UserName = request.CreatorName,
                        Content = JsonConvert.SerializeObject(request),
                        Action = "Edit",
                        FunctionUnique = "DMCTV",
                        UserId = request.Creator
                    }).ConfigureAwait(false);
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Cập nhật thông tin DMCTV thành công!";
                }
            }


            if (response.ListError.Count > 0)
            {
                foreach (var item in response.ListError)
                {
                    response.Message = response.Message + "</br>" + item.Message;
                }

                response.Message = response.Message.Substring(5);
            }

            return response;
        }

        public DMCTVCreateResponse Validation(DMCTVCreateRequest request)
        {
            var response = new DMCTVCreateResponse();
            response.ListError = new List<ItemError>();

            //mã bác sĩ
            if (string.IsNullOrEmpty(request.MaBS))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtMaBS", Message = "Mã bác sĩ không được bỏ trống." });
            }
            else if (request.MaBS.Contains(" "))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtMaBS", Message = "Mã bác sĩ không được có khoảng trắng." });
            }
            else if (request.MaBS.Length > 10)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtMaBS", Message = "Mã bác sĩ có độ dài vượt quá số ký tự cho phép." });
            }
            else if (!GlobalEnums.ValidateTextSpeciallyFull(request.MaBS))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtMaBS", Message = "Mã bác sĩ có ký tự đặc biệt." });
            }

            //tên bác sĩ
            if (string.IsNullOrEmpty(request.TenBS))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtTenBS", Message = "Tên bác sĩ không được bỏ trống." });
            }
            else if (request.TenBS.Length > 200)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtTenBS", Message = "Tên bác sĩ có độ dài vượt quá số ký tự cho phép." });
            }
            else if (!GlobalEnums.ValidateTextSpecially(request.TenBS))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtTenBS", Message = "Tên bác sĩ có ký tự đặc biệt." });
            }

            // txtSoCMND
            if (!string.IsNullOrEmpty(request.TenBS))
            {
                if (request.TenBS.Length != 9 && request.TenBS.Length != 12 )
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    response.ListError.Add(new ItemError { FieldError = "txtSoCMND", Message = "Số CMND/CCCD không đúng." });
                }
            }

            return response;
        }
    }
}