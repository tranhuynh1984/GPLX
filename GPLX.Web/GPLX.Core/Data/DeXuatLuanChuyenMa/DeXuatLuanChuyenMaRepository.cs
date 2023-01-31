using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contants;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.DeXuatLuanChuyenMa;
using GPLX.Core.DTO.Request.DeXuatLuanChuyenMa;
using GPLX.Core.DTO.Response.DeXuatLuanChuyenMa;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace GPLX.Core.Data.DeXuatLuanChuyenMa
{
    public class DeXuatLuanChuyenMaRepository : IDeXuatLuanChuyenMaRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DeXuatLuanChuyenMaRepository> _logger;
        private readonly IActionLogsRepository _actionLogsRepository;

        public DeXuatLuanChuyenMaRepository(Context context, IMapper mapper, ILogger<DeXuatLuanChuyenMaRepository> logger, IActionLogsRepository actionLogsRepository)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
            _actionLogsRepository = actionLogsRepository;
        }

        public async Task<DeXuatLuanChuyenMaSearchResponse> Search(DeXuatLuanChuyenMaSearchRequest request)
        {
            var response = new DeXuatLuanChuyenMaSearchResponse { Draw = request.Draw };
            var query = _context.DeXuatLuanChuyenMa.AsQueryable()
                .LeftJoin(_context.DMCTV, dx => dx.MaCTV, ctv => ctv.MaBS, (dx, ctv) => new
                {
                    DeXuatCode = dx.DeXuatCode,
                    MaCTV = dx.MaCTV,
                    TenCTV = ctv.TenBS,
                    ThoiGianKhoa = dx.ThoiGianKhoa,
                    //ThoiGianKhoaString = dx.ThoiGianKhoaString,
                    Note = dx.Note,
                    //ProcessId = dx.ProcessId,
                    //ProcessStepId = dx.ProcessStepId
                });

            query = query.Where(x => x.DeXuatCode == request.DeXuatCode);
            var data = await query.OrderBy(x => x.DeXuatCode).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<DeXuatLuanChuyenMaSearchResponseData>();

            foreach (var d in data)
            {
                DeXuatLuanChuyenMaSearchResponseData item = new DeXuatLuanChuyenMaSearchResponseData();

                item.DeXuatCode = d.DeXuatCode;
                item.MaCTV = d.MaCTV;
                item.TenCTV = d.TenCTV;
                item.ThoiGianKhoa = d.ThoiGianKhoa;
                item.ThoiGianKhoaString = d.ThoiGianKhoa.ToString("yyyy-MM-dd");
                item.Note = d.Note;

                dataResponse.Add(item);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<DeXuatLuanChuyenMaCreateResponse> Create(DeXuatLuanChuyenMaCreateRequest request)
        {
            var response = new DeXuatLuanChuyenMaCreateResponse();

            if (request.ThoiGianKhoa.Date < DateTime.Now.Date)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.Message = "Thời gian luân chuyển nhỏ hơn ngày hiện tại!";
                return response;
            }

            if (request.MaCTV.Equals("-1"))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.Message = "Hãy chọn cộng tác viên!";
                return response;
            }

            var query = _context.DeXuatLuanChuyenMa.AsQueryable();
            query = query.Where(x => x.DeXuatCode == request.DeXuatCode && x.MaCTV == request.MaCTV);
            var data = await query.OrderBy(x => x.DeXuatCode).ToListAsync();

            if (data.Count > 0)
            {
                //Edit
                var item = data.FirstOrDefault();
                item.DeXuatCode = request.DeXuatCode;
                item.MaCTV = request.MaCTV;
                item.ThoiGianKhoa = request.ThoiGianKhoa;
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
                    FunctionUnique = "DeXuatLuanChuyenMa",
                    UserId = request.Creator
                }).ConfigureAwait(false);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Message = "Cập nhật thông tin đề xuất luân chuyên mã ctv thành công!";
            }
            else
            {
                //Add New
                var item = new Database.Models.DeXuatLuanChuyenMa();

                item.DeXuatCode = request.DeXuatCode;
                item.MaCTV = request.MaCTV;
                item.ThoiGianKhoa = request.ThoiGianKhoa;
                item.Note = request.Note;

                item.Createby = request.CreatorName;
                item.Createdate = DateTime.Now;
                _context.DeXuatLuanChuyenMa.Add(item);
                await _context.SaveChangesAsync();
                await _actionLogsRepository.AddLogAsync(new ActionLogs
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    UserName = request.CreatorName,
                    Content = JsonConvert.SerializeObject(request),
                    Action = "Add",
                    FunctionUnique = "DeXuatLuanChuyenMa",
                    UserId = request.Creator
                }).ConfigureAwait(false);
                //response.ProcessStepId = item.SubId;
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Message = "Thêm mới thông tin đề xuất luân chuyển mã ctv thành công!";
            }

            return response;
        }

        public async Task<DeXuatLuanChuyenMaCreateResponse> Remove(DeXuatLuanChuyenMaCreateRequest request)
        {
            var response = new DeXuatLuanChuyenMaCreateResponse();

            var query = _context.DeXuatLuanChuyenMa.AsQueryable();
            query = query.Where(x => x.DeXuatCode == request.DeXuatCode && x.MaCTV == request.MaCTV);
            var data = await query.OrderBy(x => x.DeXuatCode).ToListAsync();

            if (data.Count > 0)
            {
                //Edit
                var item = data.FirstOrDefault();
                _context.DeXuatLuanChuyenMa.Remove(item);
                await _context.SaveChangesAsync();
                await _actionLogsRepository.AddLogAsync(new ActionLogs
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    UserName = request.CreatorName,
                    Content = JsonConvert.SerializeObject(request),
                    Action = "Remove",
                    FunctionUnique = "DeXuatLuanChuyenMa",
                    UserId = request.Creator
                }).ConfigureAwait(false);
            }
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
            response.Message = "Xóa thông tin đề xuất luân chuyển mã thành công!";

            return response;
        }
    }
}
