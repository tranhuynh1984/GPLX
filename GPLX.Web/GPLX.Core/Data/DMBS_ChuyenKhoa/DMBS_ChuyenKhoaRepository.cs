using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.DMBS_ChuyenKhoa;
using GPLX.Core.DTO.Request.DMBS_ChuyenKhoa;
using GPLX.Core.DTO.Response.DM;
using GPLX.Core.DTO.Response.DMBS_ChuyenKhoa;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace GPLX.Core.Data.DMBS_ChuyenKhoa
{
    public class DMBS_ChuyenKhoaRepository : IDMBS_ChuyenKhoaRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DMBS_ChuyenKhoaRepository> _logger;
        private readonly IActionLogsRepository _actionLogsRepository;

        public DMBS_ChuyenKhoaRepository(Context context, IMapper mapper, ILogger<DMBS_ChuyenKhoaRepository> logger, IActionLogsRepository actionLogsRepository)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
            _actionLogsRepository = actionLogsRepository;
        }

        public async Task<DMBS_ChuyenKhoaSearchResponse> Search(int skip, int length, DMBS_ChuyenKhoaSearchRequest request)
        {
            var response = new DMBS_ChuyenKhoaSearchResponse { Draw = request.Draw };
            
            var query = _context.DMBS_ChuyenKhoa.AsQueryable();
            if (!string.IsNullOrEmpty(request.Ma))
                query = query.Where(x => x.Ma.ToLower().Contains(request.Ma.Trim().ToLower()));
            if (!string.IsNullOrEmpty(request.Ten))
                query = query.Where(x => x.Ten.ToLower().Contains(request.Ten.Trim().ToLower()));
            if (request.Status != -1)
                query = query.Where(x => x.IsActive == request.Status);

            var data = await query.OrderByDescending(x => x.Createdate).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<DMBS_ChuyenKhoaSearchResponseData>();

            foreach (var d in data.Skip(skip).Take(length))
            {
                var dMap = _mapper.Map<DMBS_ChuyenKhoaSearchResponseData>(d);
                dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);

                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<DMBS_ChuyenKhoaSearchResponse> SearchAll(DMBS_ChuyenKhoaSearchRequest request)
        {
            var response = new DMBS_ChuyenKhoaSearchResponse { Draw = request.Draw };

            var query = _context.DMBS_ChuyenKhoa.AsQueryable();
            if (!string.IsNullOrEmpty(request.Ma))
                query = query.Where(x => x.Ma.ToLower().Contains(request.Ma.Trim().ToLower()));
            if (!string.IsNullOrEmpty(request.Ten))
                query = query.Where(x => x.Ten.ToLower().Contains(request.Ten.Trim().ToLower()));
            if (request.Status != -1)
                query = query.Where(x => x.IsActive == request.Status);

            var data = await query.OrderByDescending(x => x.Createdate).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<DMBS_ChuyenKhoaSearchResponseData>();

            for(var i = 0; i < data.Count; i ++)
            {
                var d = data[i];
                var dMap = _mapper.Map<DMBS_ChuyenKhoaSearchResponseData>(d);
                dMap.Index = i + 1;
                dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);
                dMap.CreatedateString = dMap.Createdate.ToString("HH:mm dd/MM/yyyy");
                dMap.UpdatedateString = dMap.Updatedate?.ToString("HH:mm dd/MM/yyyy");

                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<DMBS_ChuyenKhoaSearchResponseData> GetById(string ma)
        {
            var record = await _context.DMBS_ChuyenKhoa.FirstOrDefaultAsync(x => x.Ma == ma);
            if (record == null)
                return null;
            var response = _mapper.Map<DMBS_ChuyenKhoaSearchResponseData>(record);
            return response;
        }

        /// <summary>
        /// Thêm sửa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<DMBS_ChuyenKhoaCreateResponse> Create(DMBS_ChuyenKhoaCreateRequest request)
        {
            var response = new DMBS_ChuyenKhoaCreateResponse();

            response = Validation(request);

            request.Ma = String.IsNullOrEmpty(request.Ma) ? "" : request.Ma.Trim();
            request.Ten = String.IsNullOrEmpty(request.Ten) ? "" : request.Ten.Trim();

            if (response.ListError.Count == 0)
            {
                var query = _context.DMBS_ChuyenKhoa.AsQueryable();
                query = query.Where(x => x.Ma == request.Ma);
                var data = await query.OrderBy(x => x.Ten).ToListAsync();

                if (string.IsNullOrEmpty(request.Record))
                {
                    if (data.Count > 0)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                        response.ListError.Add(new ItemError { FieldError = "txtmachuyenkhoa", Message = "Mã chuyên khoa đã tồn tại trong hệ thống." });
                    }
                    else
                    {
                        //Add New
                        var item = new Database.Models.DMBS_ChuyenKhoa();
                        item.Ma = request.Ma;
                        item.Ten = request.Ten;
                        item.IsActive = request.IsActive;
                        item.Createby = request.CreatorName;
                        item.Createdate = DateTime.Now;
                        item.Stt = request.Stt;
                        _context.DMBS_ChuyenKhoa.Add(item);
                        await _context.SaveChangesAsync();
                        await _actionLogsRepository.AddLogAsync(new ActionLogs
                        {
                            Id = Guid.NewGuid(),
                            CreatedDate = DateTime.Now,
                            UserName = request.CreatorName,
                            Content = JsonConvert.SerializeObject(request),
                            Action = "Add",
                            FunctionUnique = "DMBS_ChuyenKhoa",
                            UserId = request.Creator
                        }).ConfigureAwait(false);
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = "Thêm mới thông tin danh mục chuyên khoa thành công!";
                    }
                }
                else
                {
                    //Edit
                    var item = data.FirstOrDefault();
                    item.Ma = request.Ma;
                    item.Ten = request.Ten;
                    item.IsActive = request.IsActive;
                    item.Updateby = request.CreatorName;
                    item.Updatedate = DateTime.Now;
                    item.Stt = request.Stt;
                    await _context.SaveChangesAsync();
                    await _actionLogsRepository.AddLogAsync(new ActionLogs
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        UserName = request.CreatorName,
                        Content = JsonConvert.SerializeObject(request),
                        Action = "Edit",
                        FunctionUnique = "DMBS_ChuyenKhoa",
                        UserId = request.Creator
                    }).ConfigureAwait(false);
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Cập nhật thông tin danh mục chuyên khoa thành công!";
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

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<DMBS_ChuyenKhoaCreateResponse> Remove(DMBS_ChuyenKhoaCreateRequest request)
        {
            var response = new DMBS_ChuyenKhoaCreateResponse();

            if (string.IsNullOrEmpty(response.Message))
            {
                var query = _context.DMBS_ChuyenKhoa.AsQueryable();
                query = query.Where(x => x.Ma == request.Ma);
                var data = await query.OrderBy(x => x.Ten).ToListAsync();

                if (data.Count > 0)
                {
                    //Remove
                    var item = data.FirstOrDefault();
                    _context.DMBS_ChuyenKhoa.Remove(item);
                    await _context.SaveChangesAsync();
                    await _actionLogsRepository.AddLogAsync(new ActionLogs
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        UserName = request.CreatorName,
                        Content = JsonConvert.SerializeObject(request),
                        Action = "Delete",
                        FunctionUnique = "DMBS_ChuyenKhoa",
                        UserId = request.Creator
                    }).ConfigureAwait(false);
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Xóa thông tin danh mục chuyên khoa thành công!";
                }
            }

            return response;
        }

        public DMBS_ChuyenKhoaCreateResponse Validation(DMBS_ChuyenKhoaCreateRequest request)
        {
            var response = new DMBS_ChuyenKhoaCreateResponse();
            response.ListError = new List<ItemError>();

            if (string.IsNullOrEmpty(request.Ma))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtmachuyenkhoa", Message = "Mã chuyên khoa không được trống." });
            }
            else if (request.Ma.Contains(" "))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtmachuyenkhoa", Message = "Mã chuyên khoa không được có khoảng trắng." });
            }
            else if (request.Ma.Length > 20)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtmachuyenkhoa", Message = "Mã chuyên khoa có độ dài vượt quá số ký tự cho phép." });
            }
            else if (!GlobalEnums.ValidateTextSpeciallyFull(request.Ma))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtmachuyenkhoa", Message = "Mã chuyên khoa có ký tự đặc biệt." });
            }
            if (string.IsNullOrEmpty(request.Ten))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txttenchuyenkhoa", Message = "Tên chuyên khoa không được trống." });
            }
            else if (request.Ten.Length > 50)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txttenchuyenkhoa", Message = "Tên chuyên khoa có độ dài vượt quá số ký tự cho phép." });
            }
            else if (!GlobalEnums.ValidateTextSpecially(request.Ten))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txttenchuyenkhoa", Message = "Tên chuyên khoa có ký tự đặc biệt." });
            }
            if (request.Stt == 0)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtstt", Message = "Thứ tự chuyên khoa không được bỏ trống." });
            }

            return response;
        }

        public async Task<int> GetMaxStt()
        {
            int maxstt = 1;
            if(_context.DMBS_ChuyenKhoa.Count()>0)
                maxstt = await _context.DMBS_ChuyenKhoa.MaxAsync(e => e.Stt) + 1;
            return maxstt;
        }
    }
}
