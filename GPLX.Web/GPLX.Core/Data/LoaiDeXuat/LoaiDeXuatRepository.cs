using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.LoaiDeXuat;
using GPLX.Core.DTO.Request.LoaiDeXuat;
using GPLX.Core.DTO.Response.DM;
using GPLX.Core.DTO.Response.LoaiDeXuat;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace GPLX.Core.Data.LoaiDeXuat
{
    public class LoaiDeXuatRepository : ILoaiDeXuatRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<LoaiDeXuatRepository> _logger;
        private readonly IActionLogsRepository _actionLogsRepository;

        public LoaiDeXuatRepository(Context context, IMapper mapper, ILogger<LoaiDeXuatRepository> logger, IActionLogsRepository actionLogsRepository)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
            _actionLogsRepository = actionLogsRepository;
        }

        public async Task<LoaiDeXuatSearchResponse> Search(int skip, int length, LoaiDeXuatSearchRequest request)
        {
            var response = new LoaiDeXuatSearchResponse { Draw = request.Draw };

            var query = _context.LoaiDeXuat.AsNoTracking();
            if (!string.IsNullOrEmpty(request.LoaiDeXuatCode))
                query = query.Where(x => x.LoaiDeXuatCode.Contains(request.LoaiDeXuatCode.Trim().ToLower()));
            if (!string.IsNullOrEmpty(request.LoaiDeXuatName))
                query = query.Where(x => x.LoaiDeXuatName.Contains(request.LoaiDeXuatName.Trim().ToLower()));
            if (request.Status != -1)
                query = query.Where(x => x.IsActive == request.Status);


            var data = await query.OrderBy(x => x.LoaiDeXuatName).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<LoaiDeXuatSearchResponseData>();

            foreach (var d in data.Skip(skip).Take(length))
            {
                var dMap = _mapper.Map<LoaiDeXuatSearchResponseData>(d);
                dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);
                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<LoaiDeXuatSearchResponse> GetAllLoaiDeXuat()
        {
            var response = new LoaiDeXuatSearchResponse { };

            var query = _context.LoaiDeXuat.AsQueryable();
            query = query.Where(x => x.IsActive == 1);
            var data = await query.OrderBy(x => x.Stt).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<LoaiDeXuatSearchResponseData>();

            foreach (var d in data)
            {
                var dMap = _mapper.Map<LoaiDeXuatSearchResponseData>(d);
                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<LoaiDeXuatSearchResponse> SearchAll(LoaiDeXuatSearchRequest request)
        {
            var response = new LoaiDeXuatSearchResponse { Draw = request.Draw };

            var query = _context.LoaiDeXuat.AsQueryable();
            if (!string.IsNullOrEmpty(request.LoaiDeXuatCode))
                query = query.Where(x => x.LoaiDeXuatCode.Contains(request.LoaiDeXuatCode.Trim().ToLower()));
            if (!string.IsNullOrEmpty(request.LoaiDeXuatName))
                query = query.Where(x => x.LoaiDeXuatName.Contains(request.LoaiDeXuatName.Trim().ToLower()));
            if (request.Status != -1)
                query = query.Where(x => x.IsActive == request.Status);

            var data = await query.OrderBy(x => x.LoaiDeXuatName).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<LoaiDeXuatSearchResponseData>();

            for (var i = 0; i < data.Count; i++)
            {
                var d = data[i];
                var dMap = _mapper.Map<LoaiDeXuatSearchResponseData>(d);
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

        public async Task<LoaiDeXuatSearchResponseData> GetById(string loaiDeXuatCode)
        {
            var record = await _context.LoaiDeXuat.FirstOrDefaultAsync(x => x.LoaiDeXuatCode == loaiDeXuatCode);
            if (record == null)
                return null;
            var response = _mapper.Map<LoaiDeXuatSearchResponseData>(record);
            return response;
        }

        /// <summary>
        /// Thêm sửa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<LoaiDeXuatCreateResponse> Create(LoaiDeXuatCreateRequest request)
        {
            var response = new LoaiDeXuatCreateResponse();

            response = Validation(request);

            request.LoaiDeXuatCode = String.IsNullOrEmpty(request.LoaiDeXuatCode) ? "" : request.LoaiDeXuatCode.Trim();
            request.LoaiDeXuatName = String.IsNullOrEmpty(request.LoaiDeXuatName) ? "" : request.LoaiDeXuatName.Trim();

            if (response.ListError.Count == 0)
            {
                var query = _context.LoaiDeXuat.AsQueryable();
                query = query.Where(x => x.LoaiDeXuatCode == request.LoaiDeXuatCode);
                var data = await query.OrderBy(x => x.LoaiDeXuatName).ToListAsync();

                if (string.IsNullOrEmpty(request.Record))
                {
                    if (data.Count > 0)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        response.ListError.Add(new ItemError { FieldError = "txtmaloaidexuat", Message = "Mã loại đề xuất đã tồn tại trong hệ thống." });
                    }
                    else
                    {
                        //Add New
                        var item = new Database.Models.LoaiDeXuat();
                        item.LoaiDeXuatCode = request.LoaiDeXuatCode;
                        item.LoaiDeXuatName = request.LoaiDeXuatName;
                        item.IsActive = request.IsActive;
                        item.Createby = request.CreatorName;
                        item.Createdate = DateTime.Now;
                        item.Stt = request.Stt;
                        _context.LoaiDeXuat.Add(item);
                        await _context.SaveChangesAsync();
                        await _actionLogsRepository.AddLogAsync(new ActionLogs
                        {
                            Id = Guid.NewGuid(),
                            CreatedDate = DateTime.Now,
                            UserName = request.CreatorName,
                            Content = JsonConvert.SerializeObject(request),
                            Action = "Add",
                            FunctionUnique = "LoaiDeXuat",
                            UserId = request.Creator
                        }).ConfigureAwait(false);
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = "Thêm mới thông tin danh mục loại đề xuất thành công!";
                    }
                }
                else
                {
                    //Edit
                    var item = data.FirstOrDefault();
                    item.LoaiDeXuatCode = request.LoaiDeXuatCode;
                    item.LoaiDeXuatName = request.LoaiDeXuatName;
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
                        FunctionUnique = "LoaiDeXuat",
                        UserId = request.Creator
                    }).ConfigureAwait(false);
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Cập nhật thông tin danh mục loại đề xuất thành công!";
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
        public async Task<LoaiDeXuatCreateResponse> Remove(LoaiDeXuatCreateRequest request)
        {
            var response = new LoaiDeXuatCreateResponse();

            if (string.IsNullOrEmpty(response.Message))
            {
                var query = _context.LoaiDeXuat.AsQueryable();
                query = query.Where(x => x.LoaiDeXuatCode == request.LoaiDeXuatCode);
                var data = await query.OrderBy(x => x.LoaiDeXuatName).ToListAsync();

                if (data.Count > 0)
                {
                    //Remove
                    var item = data.FirstOrDefault();
                    _context.LoaiDeXuat.Remove(item);
                    await _context.SaveChangesAsync();
                    await _actionLogsRepository.AddLogAsync(new ActionLogs
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        UserName = request.CreatorName,
                        Content = JsonConvert.SerializeObject(request),
                        Action = "Delete",
                        FunctionUnique = "LoaiDeXuat",
                        UserId = request.Creator
                    }).ConfigureAwait(false);
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Xóa thông tin loại đề xuất thành công!";
                }
            }

            return response;
        }

        public LoaiDeXuatCreateResponse Validation(LoaiDeXuatCreateRequest request)
        {
            var response = new LoaiDeXuatCreateResponse();
            response.ListError = new List<ItemError>();

            if (string.IsNullOrEmpty(request.LoaiDeXuatCode))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.ListError.Add(new ItemError { FieldError = "txtmaloaidexuat", Message = "Mã loại đề xuất không được trống." });
            }
             else if (request.LoaiDeXuatCode.Contains(" "))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtmaloaidexuat", Message = "Mã loại đề xuất không được có khoảng trắng." });
            }
            else if (request.LoaiDeXuatCode.Length > 20)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.ListError.Add(new ItemError { FieldError = "txtmaloaidexuat", Message = "Mã loại đề xuất có độ dài vượt quá số ký tự cho phép." });
            }
            else if (!GlobalEnums.ValidateTextSpeciallyFull(request.LoaiDeXuatCode))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtmaloaidexuat", Message = "Mã loại đề xuất có ký tự đặc biệt." });
            }
            if (string.IsNullOrEmpty(request.LoaiDeXuatName))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.ListError.Add(new ItemError { FieldError = "txttenloaidexuat", Message = "Tên loại đề xuất không được trống." });
            }
            else if (request.LoaiDeXuatName.Length > 200)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.ListError.Add(new ItemError { FieldError = "txttenloaidexuat", Message = "Tên loại đề xuất có độ dài vượt quá số ký tự cho phép." });
            }
            else if (!GlobalEnums.ValidateTextSpecially(request.LoaiDeXuatName))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txttenloaidexuat", Message = "Tên loại đề xuất có ký tự đặc biệt." });
            }
            if (request.Stt == 0)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtstt", Message = "Thứ tự loại đề xuất không được bỏ trống." });
            }

            return response;
        }

        public async Task<int> GetMaxStt( )
        {
            int maxstt = 0;
            maxstt = await _context.LoaiDeXuat.MaxAsync(e => e.Stt) + 1;
            return maxstt;
        }
    }
}
