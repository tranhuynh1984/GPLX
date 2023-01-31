using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.DeXuat;
using GPLX.Core.Contracts.DeXuatGhiChu;
using GPLX.Core.DTO.Request.DeXuat;
using GPLX.Core.DTO.Request.DeXuatGhiChu;
using GPLX.Core.DTO.Response.DeXuat;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace GPLX.Core.Data.DeXuat
{
    public class DeXuatRepository : IDeXuatRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DeXuatRepository> _logger;
        private readonly IActionLogsRepository _actionLogsRepository;
        private readonly IDeXuatGhiChuRepository _deXuatGhiChuRepository;

        public DeXuatRepository(Context context, IMapper mapper, IDeXuatGhiChuRepository deXuatGhiChuRepository, ILogger<DeXuatRepository> logger, IActionLogsRepository actionLogsRepository)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
            _actionLogsRepository = actionLogsRepository;
            _deXuatGhiChuRepository = deXuatGhiChuRepository;
        }

        public async Task<DeXuatSearchResponse> Search(int skip, int length, DeXuatSearchRequest request)
        {
            var response = new DeXuatSearchResponse { Draw = request.Draw };
            var query = _context.DeXuat.AsNoTracking();

            query = query.Where(x => x.ProcessStepId >= request.IDRole || (x.ProcessStepId == 0 && (request.IDRole == 1 || request.IDRole == 9)));

            if (request.IDRole != 7 && request.IDRole != 8 && request.IDRole != 9 && request.IDRole != 12)
                query = query.Where(x => x.MaDonViDeXuat == request.MaDonViDeXuat);
            if (!string.IsNullOrEmpty(request.DeXuatCode))
                query = query.Where(x => x.DeXuatCode.Contains(request.DeXuatCode.Trim()));
            if (!string.IsNullOrEmpty(request.DeXuatName))
                query = query.Where(x => x.DeXuatName.Contains(request.DeXuatName.Trim()));

            //Chờ duyệt
            if (request.Status == 0)
                query = query.Where(x => x.IsDone == 0 && x.ProcessStepId == request.IDRole);
            //Đã duyệt
            else if (request.Status == 1)
                query = query.Where(x => x.ProcessStepId > request.IDRole || (x.IsDone == 1 && x.ProcessStepId == request.IDRole));
            //Hoàn thành
            else if (request.Status == 2)
                query = query.Where(x => x.IsDone == 1 && x.ProcessStepId >= request.IDRole);
            //Quá hạn
            else if (request.Status == 3)
                query = query.Where(x => x.ThoiGianKhoa < DateTime.Now && x.ProcessStepId < request.IDRole);

            if (!string.IsNullOrEmpty(request.LoaiDeXuatCode) && (request.LoaiDeXuatCode != "-1"))
                query = query.Where(x => x.LoaiDeXuatCode == request.LoaiDeXuatCode);

            if (request.ND != null)
                query = query.Where(x => x.Createdate.Date >= request.ND.Value.Date);
            if (request.NS != null)
                query = query.Where(x => x.Createdate.Date <= request.NS.Value.Date);
            if (request.NguoiTao != "-1" && request.NguoiTao != "Tất cả")
                query = query.Where(x => x.Createby.Equals(request.NguoiTao));

            var queryldx = _context.LoaiDeXuat.AsNoTracking();

            var data = await query.OrderByDescending(x => x.Createdate).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<DeXuatSearchResponseData>();

            foreach (var d in data.Skip(skip).Take(length))
            {
                var dMap = _mapper.Map<DeXuatSearchResponseData>(d);

                if (dMap.IsDone == 1 && dMap.ProcessStepId >= request.IDRole)
                    dMap.TrangThai = "Hoàn thành";
                else if (dMap.ThoiGianKhoa?.Date < DateTime.Now.Date && dMap.ProcessStepId < request.IDRole)
                    dMap.TrangThai = "Quá hạn";
                else if (dMap.ProcessStepId > request.IDRole || (dMap.IsDone == 1 && dMap.ProcessStepId == request.IDRole))
                    dMap.TrangThai = "Đã duyệt";
                else
                    dMap.TrangThai = "Chờ duyệt";

                var itemqueryldx = queryldx.Where(x => x.LoaiDeXuatCode == dMap.LoaiDeXuatCode).FirstOrDefault();
                dMap.TenDeXuatCode = itemqueryldx == null? "" : itemqueryldx.LoaiDeXuatName;
                dMap.IDRole = request.IDRole;

                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<DeXuatSearchResponse> SearchAll(DeXuatSearchRequest request)
        {
            var response = new DeXuatSearchResponse { Draw = request.Draw };
            var query = _context.DeXuat.AsNoTracking();
            if (request.IDRole != 7 && request.IDRole != 8 && request.IDRole != 9 && request.IDRole != 12)
                query = query.Where(x => x.MaDonViDeXuat == request.MaDonViDeXuat);
            if (!string.IsNullOrEmpty(request.DeXuatCode))
                query = query.Where(x => x.DeXuatCode.Contains(request.DeXuatCode));
            if (!string.IsNullOrEmpty(request.DeXuatName))
                query = query.Where(x => x.DeXuatName.Contains(request.DeXuatName));

            if (!string.IsNullOrEmpty(request.LoaiDeXuatCode) && (request.LoaiDeXuatCode != "-1"))
                query = query.Where(x => x.LoaiDeXuatCode == request.LoaiDeXuatCode);

            if (request.ND != null)
                query = query.Where(x => x.Createdate.Date >= request.ND.Value.Date);
            if (request.NS != null)
                query = query.Where(x => x.Createdate.Date <= request.NS.Value.Date);
            var data = await query.OrderByDescending(x => x.Createdate).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<DeXuatSearchResponseData>();

            var queryldx = _context.LoaiDeXuat.AsNoTracking();
            for (int i=0; i< data.Count;i++)
            {
                var d = data[i];
                var dMap = _mapper.Map<DeXuatSearchResponseData>(d);
                dMap.Index = i + 1;
                dMap.CreatedateString = dMap.Createdate.ToString("HH:mm dd/MM/yyyy");
                dMap.UpdatedateString = dMap.Updatedate?.ToString("HH:mm dd/MM/yyyy");
                if (dMap.IsDone == 1 && dMap.ProcessStepId >= request.IDRole)
                    dMap.TrangThai = "Hoàn thành";
                else if (dMap.ThoiGianKhoa?.Date < DateTime.Now.Date && dMap.ProcessStepId < request.IDRole)
                    dMap.TrangThai = "Quá hạn";
                else if (dMap.ProcessStepId >request.IDRole || (dMap.IsDone == 1 && dMap.ProcessStepId == request.IDRole))
                    dMap.TrangThai = "Đã duyệt";
                else
                    dMap.TrangThai = "Chờ duyệt";

                var itemqueryldx = queryldx.Where(x => x.LoaiDeXuatCode == dMap.LoaiDeXuatCode).FirstOrDefault();
                dMap.TenDeXuatCode = itemqueryldx == null ? "" : itemqueryldx.LoaiDeXuatName;

                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<int> ChoDuyet(int IDRole)
        {
            var query = _context.DeXuat.AsNoTracking();
            query = query.Where(x => x.ProcessStepId >= IDRole || (x.ProcessStepId == 0 && (IDRole == 1 || IDRole == 9)));
            query = query.Where(x => x.IsDone == 0 && x.ProcessStepId == IDRole);
            var data = await query.OrderBy(x => x.DeXuatCode).ToListAsync();
            return data.Count;
        }
        public async Task<int> DaDuyet(int IDRole)
        {
            var query = _context.DeXuat.AsNoTracking();
            query = query.Where(x => x.ProcessStepId >= IDRole || (x.ProcessStepId == 0 && (IDRole == 1 || IDRole == 9)));
            query = query.Where(x => x.ProcessStepId > IDRole || (x.IsDone == 1 && x.ProcessStepId == IDRole));
            var data = await query.OrderBy(x => x.DeXuatCode).ToListAsync();
            return data.Count;
        }
        public async Task<int> HoanThanh(int IDRole)
        {
            var query = _context.DeXuat.AsNoTracking();
            query = query.Where(x => x.ProcessStepId >= IDRole || (x.ProcessStepId == 0 && (IDRole == 1 || IDRole == 9)));
            query = query.Where(x => x.IsDone == 1 && x.ProcessStepId >= IDRole);
            var data = await query.OrderBy(x => x.DeXuatCode).ToListAsync();
            return data.Count;
        }
        public async Task<int> QuaHan(int IDRole)
        {
            var query = _context.DeXuat.AsNoTracking();
            query = query.Where(x => x.ProcessStepId >= IDRole || (x.ProcessStepId == 0 && (IDRole == 1 || IDRole == 9)));
            query = query.Where(x => x.ThoiGianKhoa < DateTime.Now && x.ProcessStepId < IDRole);
            var data = await query.OrderBy(x => x.DeXuatCode).ToListAsync();
            return data.Count;
        }

        public async Task<DeXuatSearchResponseData> GetByCode(string dexuatcode)
        {
            var record = await _context.DeXuat.FirstOrDefaultAsync(x => x.DeXuatCode == dexuatcode);

            var response = _mapper.Map<DeXuatSearchResponseData>(record);
            if (!string.IsNullOrEmpty(dexuatcode)) //TODO: code sai, can luu cung thong tin ten bac si vao de xuat
            {
                var ctv = _context.DMCTV.Where(x => x.MaBS == response.MaBacsi).FirstOrDefault();
                if (ctv != null)
                    response.TenBacsi = ctv.TenBS;
            }
            
            if (response == null)
            {
                response = new DeXuatSearchResponseData();
                response.ThoiGianKhoa = DateTime.Now;
            }    
            return response;
        }

        public async Task<DeXuatCreateResponse> Create(DeXuatCreateRequest request)
        {
            var response = new DeXuatCreateResponse();

            response = Validation(request);

            if (response.ListError.Count == 0)
            {
                var query = _context.DeXuat.AsQueryable();
                query = query.Where(x => x.DeXuatCode == request.DeXuatCode);
                var data = await query.OrderBy(x => x.DeXuatCode).ToListAsync();

                //Tạo mới
                if (string.IsNullOrEmpty(request.Record))
                {
                    if (data.Count > 0)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        response.Message = "Mã đề xuất đã tồn tại trong hệ thống.";
                    }
                    else
                    {
                        //Add New
                        var item = new Database.Models.DeXuat();

                        item.DeXuatCode = request.DeXuatCode;
                        item.DeXuatName = request.DeXuatName;
                        item.MaBacsi = request.MaBacsi;
                        item.TenBacsi = request.TenBacsi;
                        item.LoaiDeXuatCode = request.LoaiDeXuatCode;
                        item.ProcessId = request.ProcessId;
                        item.Note = request.Note;
                        item.LyDoKhoa = request.LyDoKhoa;
                        item.Createby = request.CreatorName;
                        item.Createdate = DateTime.Now;
                        item.MaDonViDeXuat = request.MaDonViDeXuat;
                        item.TenCongTy = request.TenCongTy;
                        if (request.LoaiDeXuatCode.Equals("DeXuatMoMa"))
                        {
                            item.ThoiGianKhoa = request.ThoiGianKhoa;
                        }
                        _context.DeXuat.Add(item);
                        await _context.SaveChangesAsync();
                        await _actionLogsRepository.AddLogAsync(new ActionLogs
                        {
                            Id = Guid.NewGuid(),
                            CreatedDate = DateTime.Now,
                            UserName = request.CreatorName,
                            Content = JsonConvert.SerializeObject(request),
                            Action = "Add",
                            FunctionUnique = "DeXuat",
                            UserId = request.Creator
                        }).ConfigureAwait(false);
                        //response.ProcessStepId = item.SubId;
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = "Thêm mới thông tin đề xuất thành công!";
                    }
                }
                else
                {
                    //Edit
                    var item = data.FirstOrDefault();
                    item.DeXuatCode = request.DeXuatCode;
                    item.DeXuatName = request.DeXuatName;
                    item.MaBacsi = request.MaBacsi;
                    item.TenBacsi = request.TenBacsi;
                    item.LoaiDeXuatCode = request.LoaiDeXuatCode;
                    item.TenCongTy = request.TenCongTy;
                    if (request.LoaiDeXuatCode.Equals("DeXuatMoMa"))
                    {
                        item.ThoiGianKhoa = request.ThoiGianKhoa;
                    }    
                    //item.ProcessId = request.ProcessId;
                    item.Note = request.Note;
                    item.LyDoKhoa = request.LyDoKhoa;
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
                        FunctionUnique = "DeXuat",
                        UserId = request.Creator
                    }).ConfigureAwait(false);
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Cập nhật thông tin đề xuất thành công!";
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

        public async Task<DeXuatCreateResponse> PushDeXuat(DeXuatCreateRequest request)
        {
            var response = new DeXuatCreateResponse();
            
            var query = _context.DeXuat.AsQueryable();
            query = query.Where(x => x.DeXuatCode == request.DeXuatCode);
            var data = await query.OrderBy(x => x.DeXuatCode).ToListAsync();

            if (data.Count > 0)
            {
                var item = data.FirstOrDefault();

                if (item.LoaiDeXuatCode != null && (item.LoaiDeXuatCode.Equals("DeXuatKhoaMa") || item.LoaiDeXuatCode.Equals("DeXuatLuanChuyenMa")))
                {
                    int countKM = _context.DeXuatKhoaMaCTV.Where(x => x.DeXuatCode == request.DeXuatCode).Count();
                    if (countKM == 0)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        if (item.LoaiDeXuatCode.Equals("DeXuatKhoaMa"))
                        {
                            response.Message = "Chưa chọn thông tin CTV cần khóa!"; 
                        }
                        else if(item.LoaiDeXuatCode.Equals("DeXuatLuanChuyenMa"))
                        {
                            response.Message = "Chưa chọn thông tin CTV cần chuyển!";
                        }

                        return response;
                    }
                }

                //Edit
                int processIdNext = await GetNextProcessStepId(item.ProcessId, item.ProcessStepId);

                //item.Note = request.Note;
                if(item.ProcessStepId!=0 && item.ProcessStepId == processIdNext)
                {
                    item.IsDone = 1;
                }
                item.ProcessStepId = processIdNext;
                item.Updateby = request.CreatorName;
                item.Updatedate = DateTime.Now;
                await _context.SaveChangesAsync();


                //Update to DeXuatGhiChu
                DeXuatGhiChuCreateRequest requestghichu = new DeXuatGhiChuCreateRequest();
                requestghichu.DeXuatCode = item.DeXuatCode;
                requestghichu.ProcessStepId = item.ProcessStepId;
                requestghichu.Note = request.GhiChuStep;
                requestghichu.CreateByCode = request.CreateGhiChuByCode;
                requestghichu.CreateByName = request.CreateGhiChuByName;

                _deXuatGhiChuRepository.Create(requestghichu);

                await _actionLogsRepository.AddLogAsync(new ActionLogs
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    UserName = request.CreatorName,
                    Content = JsonConvert.SerializeObject(request),
                    Action = "Push",
                    FunctionUnique = "DeXuat",
                    UserId = request.Creator
                }).ConfigureAwait(false);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Message = "Đẩy duyệt thông tin đề xuất thành công!";
            }

            return response;
        }

        public async Task<DeXuatCreateResponse> RejectDeXuat(DeXuatCreateRequest request)
        {
            var response = new DeXuatCreateResponse();

            var query = _context.DeXuat.AsQueryable();
            query = query.Where(x => x.DeXuatCode == request.DeXuatCode);
            var data = await query.OrderBy(x => x.DeXuatCode).ToListAsync();

            if (data.Count > 0)
            {
                var item = data.FirstOrDefault();

                //Reject
                //item.Note = request.Note;
                item.IsDone = -1;
                item.Updateby = request.CreatorName;
                item.Updatedate = DateTime.Now;
                await _context.SaveChangesAsync();

                //Update to DeXuatGhiChu
                DeXuatGhiChuCreateRequest requestghichu = new DeXuatGhiChuCreateRequest();
                requestghichu.DeXuatCode = item.DeXuatCode;
                requestghichu.ProcessStepId = item.ProcessStepId;
                requestghichu.Note = request.GhiChuStep;
                requestghichu.CreateByCode = request.CreateGhiChuByCode;
                requestghichu.CreateByName = request.CreateGhiChuByName;

                _deXuatGhiChuRepository.Create(requestghichu);

                await _actionLogsRepository.AddLogAsync(new ActionLogs
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    UserName = request.CreatorName,
                    Content = JsonConvert.SerializeObject(request),
                    Action = "Reject",
                    FunctionUnique = "DeXuat",
                    UserId = request.Creator
                }).ConfigureAwait(false);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Message = "Từ chối đề xuất thành công!";
            }

            return response;
        }

        public async Task<int> GetNextProcessStepId(int processId, int processStepId)
        {
            int processNextStepId = 0;
            var query = _context.ProcessStep.AsQueryable();
            query = query.Where(x => x.ProcessId == processId && x.OrderStep > processStepId);
            var data = await query.OrderBy(x => x.OrderStep).ToListAsync();
            if(data.Count>0)
                if(processStepId>0)
                    processNextStepId = data.FirstOrDefault().OrderStep;
                else
                {
                    if(data.Count>1)
                        processNextStepId = data[1].OrderStep;
                }
            else
            {
                if (processStepId > 0)
                    return processStepId;
            }

            return processNextStepId;
        }

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<DeXuatCreateResponse> Remove(DeXuatCreateRequest request)
        {
            var response = new DeXuatCreateResponse();

            if (string.IsNullOrEmpty(response.Message))
            {
                var query = _context.DeXuat.AsQueryable();
                query = query.Where(x => x.DeXuatCode == request.DeXuatCode);
                var data = await query.OrderBy(x => x.DeXuatName).ToListAsync();

                if (data.Count > 0)
                {
                    //Remove
                    var item = data.FirstOrDefault();
                    if(item.ProcessStepId>0)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        response.Message = "Mã đề xuất "+ item .DeXuatCode+ " đã được đẩy duyệt lên hệ thống!";
                        return response;
                    }    
                    _context.DeXuat.Remove(item);
                    await _context.SaveChangesAsync();
                    await _actionLogsRepository.AddLogAsync(new ActionLogs
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        UserName = request.CreatorName,
                        Content = JsonConvert.SerializeObject(request),
                        Action = "Delete",
                        FunctionUnique = "DeXuat",
                        UserId = request.Creator
                    }).ConfigureAwait(false);
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Xóa thông tin đề xuất thành công!";
                }
            }

            return response;
        }

        public DeXuatCreateResponse Validation(DeXuatCreateRequest request)
        {
            var response = new DeXuatCreateResponse();
            response.ListError = new List<ItemError>();

            if (string.IsNullOrEmpty(request.DeXuatCode))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtmadexuat", Message = "Mã đề xuất không được bỏ trống." });
            }
            else if (request.DeXuatCode.Contains(" "))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtmadexuat", Message = "Mã đề xuất không được có khoảng trắng." });
            }
            else if (request.DeXuatCode.Length > 20)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtmadexuat", Message = "Mã đề xuất có độ dài vượt quá số ký tự cho phép." });
            }
            else if (!GlobalEnums.ValidateTextSpeciallyFull(request.DeXuatCode))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtmadexuat", Message = "Mã đề xuất có ký tự đặc biệt." });
            }

            if (string.IsNullOrEmpty(request.DeXuatName))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txttendexuat", Message = "Tên đề xuất không được bỏ trống." });
            }
            else if (request.DeXuatName.Length > 100)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txttendexuat", Message = "Tên đề xuất có độ dài vượt quá số ký tự cho phép." });
            }
            else if (!GlobalEnums.ValidateTextSpecially(request.DeXuatName))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txttendexuat", Message = "Tên đề xuất có ký tự đặc biệt." });
            }

            if(request.LoaiDeXuatCode.Equals("DeXuatMoMa") || request.LoaiDeXuatCode.Equals("DeXuatSuaMa") || request.LoaiDeXuatCode.Equals("DeXuatTaoMa"))
            {
                if (request.ThoiGianKhoa == null)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    response.ListError.Add(new ItemError { FieldError = "txtThoiGianKhoa", Message = "Hãy chọn thời gian!" });
                }
                else if (request.ThoiGianKhoa == DateTime.MinValue)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    response.ListError.Add(new ItemError { FieldError = "txtThoiGianKhoa", Message = "Hãy chọn thời gian!" });
                }
                else if (request.ThoiGianKhoa?.Date < DateTime.Now.Date)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    response.ListError.Add(new ItemError { FieldError = "txtThoiGianKhoa", Message = "Thời gian chọn nhỏ hơn ngày hiện tại!" });
                }

                if (request.MaBacsi.Equals("-1"))
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    response.ListError.Add(new ItemError { FieldError = "lblCTV", Message = "Hãy chọn cộng tác viên!", FieldType ="Dropdown" });
                }
            }    
            

            return response;
        }
    }
}
