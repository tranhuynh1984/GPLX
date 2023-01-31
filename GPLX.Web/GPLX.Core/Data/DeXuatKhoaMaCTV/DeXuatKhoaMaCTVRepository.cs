using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contants;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.DeXuatKhoaMaCTV;
using GPLX.Core.DTO.Request.DeXuatKhoaMaCTV;
using GPLX.Core.DTO.Response.DeXuatKhoaMaCTV;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace GPLX.Core.Data.DeXuatKhoaMaCTV
{
    public class DeXuatKhoaMaCTVRepository : IDeXuatKhoaMaCTVRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DeXuatKhoaMaCTVRepository> _logger;
        private readonly IActionLogsRepository _actionLogsRepository;

        public DeXuatKhoaMaCTVRepository(Context context, IMapper mapper, ILogger<DeXuatKhoaMaCTVRepository> logger, IActionLogsRepository actionLogsRepository)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
            _actionLogsRepository = actionLogsRepository;
        }

        public async Task<DeXuatKhoaMaCTVSearchResponse> Search(DeXuatKhoaMaCTVSearchRequest request)
        {
            var response = new DeXuatKhoaMaCTVSearchResponse { Draw = request.Draw };
            var query = _context.DeXuatKhoaMaCTV.AsQueryable()
                .LeftJoin(_context.DMCTV, dx => dx.MaCTV, ctv => ctv.MaBS, (dx, ctv) => new
                {
                    DeXuatCode = dx.DeXuatCode,
                    MaCTV = dx.MaCTV,
                    TenCTV = ctv.TenBS,
                    ThoiGianKhoa = dx.ThoiGianKhoa,
                    //ThoiGianKhoaString = dx.ThoiGianKhoaString,
                    //LyDoKhoa = dx.LyDoKhoa,
                    Note = dx.Note,
                    //ProcessId = dx.ProcessId,
                    //ProcessStepId = dx.ProcessStepId
                });
            
            query = query.Where(x => x.DeXuatCode == request.DeXuatCode);
            var data = await query.OrderBy(x => x.DeXuatCode).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<DeXuatKhoaMaCTVSearchResponseData>();

            foreach (var d in data)
            {
                DeXuatKhoaMaCTVSearchResponseData item = new DeXuatKhoaMaCTVSearchResponseData();
                
                item.DeXuatCode = d.DeXuatCode;
                item.MaCTV = d.MaCTV;
                item.TenCTV = d.TenCTV;
                item.ThoiGianKhoa = d.ThoiGianKhoa;
                item.ThoiGianKhoaString = d.ThoiGianKhoa.ToString("yyyy-MM-dd");
                //item.LyDoKhoa = d.LyDoKhoa;
                item.Note = d.Note;

                dataResponse.Add(item);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<DeXuatKhoaMaCTVCreateResponse> Create(DeXuatKhoaMaCTVCreateRequest request)
        {
            var response = new DeXuatKhoaMaCTVCreateResponse();

            if(request.ThoiGianKhoa.Date == DateTime.MinValue)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.Message = "Thời gian khóa không được bỏ trống!";
                return response;
            }
            else if (request.ThoiGianKhoa.Date < DateTime.Now.Date)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.Message = "Thời gian khóa nhỏ hơn ngày hiện tại!";
                return response;
            }

            if (request.MaCTV.Equals("-1"))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.Message = "Hãy chọn cộng tác viên!";
                return response;
            }
            
            if (request.LyDoKhoa.Equals("1") && string.IsNullOrEmpty(request.Note))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.Message = "Hãy nhập lý do cho trường hợp gian lận!";
                return response;
            }

            var query = _context.DeXuatKhoaMaCTV.AsQueryable();
            query = query.Where(x => x.DeXuatCode == request.DeXuatCode && x.MaCTV == request.MaCTV);
            var data = await query.OrderBy(x => x.DeXuatCode).ToListAsync();

            if (data.Count > 0)
            {
                //Edit
                var item = data.FirstOrDefault();
                item.DeXuatCode = request.DeXuatCode;
                item.MaCTV = request.MaCTV;
                item.ThoiGianKhoa = request.ThoiGianKhoa;
                //item.LyDoKhoa = request.LyDoKhoa;
                item.Note = request.Note;
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
                    FunctionUnique = "DeXuatKhoaMaCTV",
                    UserId = request.Creator
                }).ConfigureAwait(false);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Message = "Cập nhật thông tin đề xuất khóa mã ctv thành công!";
            }
            else
            {
                //Add New
                var item = new Database.Models.DeXuatKhoaMaCTV();

                item.DeXuatCode = request.DeXuatCode;
                item.MaCTV = request.MaCTV;
                item.ThoiGianKhoa = request.ThoiGianKhoa;
                //item.LyDoKhoa = request.LyDoKhoa;
                item.Note = request.Note;

                item.Createby = request.CreatorName;
                item.Createdate = DateTime.Now;
                _context.DeXuatKhoaMaCTV.Add(item);
                await _context.SaveChangesAsync();
                await _actionLogsRepository.AddLogAsync(new ActionLogs
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    UserName = request.CreatorName,
                    Content = JsonConvert.SerializeObject(request),
                    Action = "Add",
                    FunctionUnique = "DeXuatKhoaMaCTV",
                    UserId = request.Creator
                }).ConfigureAwait(false);
                //response.ProcessStepId = item.SubId;
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Message = "Thêm mới thông tin đề xuất khóa mã ctv thành công!";
            }

            return response;
        }

        public async Task<DeXuatKhoaMaCTVCreateResponse> Remove(DeXuatKhoaMaCTVCreateRequest request)
        {
            var response = new DeXuatKhoaMaCTVCreateResponse();

            var query = _context.DeXuatKhoaMaCTV.AsQueryable();
            query = query.Where(x => x.DeXuatCode == request.DeXuatCode && x.MaCTV == request.MaCTV);
            var data = await query.OrderBy(x => x.DeXuatCode).ToListAsync();

            if (data.Count > 0)
            {
                //Edit
                var item = data.FirstOrDefault();
                _context.DeXuatKhoaMaCTV.Remove(item);
                await _context.SaveChangesAsync();
                await _actionLogsRepository.AddLogAsync(new ActionLogs
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    UserName = request.CreatorName,
                    Content = JsonConvert.SerializeObject(request),
                    Action = "Remove",
                    FunctionUnique = "DeXuatKhoaMaCTV",
                    UserId = request.Creator
                }).ConfigureAwait(false);
            }

            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
            response.Message = "Xóa thông tin đề xuất khóa mã CTV thành công!";

            return response;
        }
    }
}
