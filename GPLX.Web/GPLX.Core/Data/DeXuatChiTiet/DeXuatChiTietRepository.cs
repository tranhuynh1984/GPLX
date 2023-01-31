using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.DeXuatChiTiet;
using GPLX.Core.DTO.Request.DeXuatChiTiet;
using GPLX.Core.DTO.Response.DeXuatChiTiet;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace GPLX.Core.Data.DeXuatChiTiet
{
    public class DeXuatChiTietRepository : IDeXuatChiTietRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DeXuatChiTietRepository> _logger;
        private readonly IActionLogsRepository _actionLogsRepository;

        public DeXuatChiTietRepository(Context context, IMapper mapper, ILogger<DeXuatChiTietRepository> logger, IActionLogsRepository actionLogsRepository)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
            _actionLogsRepository = actionLogsRepository;
        }

        public async Task<DeXuatChiTietSearchResponse> Search(DeXuatChiTietSearchRequest request)
        {
            var response = new DeXuatChiTietSearchResponse { Draw = request.Draw };
            var query = _context.DeXuatChiTiet.AsNoTracking();
            query = query.Where(x => x.DeXuatCode == request.DeXuatCode);
            var data = await query.OrderBy(x => x.DeXuatCode).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<DeXuatChiTietSearchResponseData>();

            foreach (var d in data)
            {
                var dMap = _mapper.Map<DeXuatChiTietSearchResponseData>(d);
                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<DeXuatChiTietCreateResponse> Create(DeXuatChiTietCreateRequest request)
        {
            var response = new DeXuatChiTietCreateResponse();

            //response = Validation(request);

            //request.SubName = String.IsNullOrEmpty(request.SubName) ? "" : request.SubName.Trim();
            //request.Note = String.IsNullOrEmpty(request.Note) ? "" : request.Note.Trim();
            //request.WhyNotUse = String.IsNullOrEmpty(request.WhyNotUse) ? "" : request.WhyNotUse.Trim();

            //if (response.ListError.Count == 0)
            //{
            var query = _context.DeXuatChiTiet.AsQueryable();
            query = query.Where(x => x.DeXuatCode == request.DeXuatCode && x.FieldName == request.FieldName);
            var data = await query.OrderBy(x => x.DeXuatCode).ToListAsync();

            if (data.Count > 0)
            {
                //Edit
                var item = data.FirstOrDefault();
                item.DeXuatCode = request.DeXuatCode;
                item.FieldName = request.FieldName;
                item.ValueOld = request.ValueOld;
                item.ValueNew = request.ValueNew;
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
                    FunctionUnique = "DeXuatChiTiet",
                    UserId = request.Creator
                }).ConfigureAwait(false);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Message = "Cập nhật thông tin đề xuất chi tiết thành công!";
            }
            else
            {
                //Add New
                var item = new Database.Models.DeXuatChiTiet();

                item.DeXuatCode = request.DeXuatCode;
                item.FieldName = request.FieldName;
                item.ValueOld = request.ValueOld;
                item.ValueNew = request.ValueNew;
                item.Note = request.Note;

                item.Createby = request.CreatorName;
                item.Createdate = DateTime.Now;
                _context.DeXuatChiTiet.Add(item);
                await _context.SaveChangesAsync();
                await _actionLogsRepository.AddLogAsync(new ActionLogs
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    UserName = request.CreatorName,
                    Content = JsonConvert.SerializeObject(request),
                    Action = "Add",
                    FunctionUnique = "DeXuatChiTiet",
                    UserId = request.Creator
                }).ConfigureAwait(false);
                //response.ProcessStepId = item.SubId;
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Message = "Thêm mới thông tin đề xuất chi tiết thành công!";
            }
            

            return response;
        }

        public async Task<DeXuatChiTietCreateResponse> Remove(DeXuatChiTietCreateRequest request)
        {
            var response = new DeXuatChiTietCreateResponse();
        
            var query = _context.DeXuatChiTiet.AsQueryable();
            query = query.Where(x => x.DeXuatCode == request.DeXuatCode && x.FieldName == request.FieldName);
            var data = await query.OrderBy(x => x.DeXuatCode).ToListAsync();

            if (data.Count > 0)
            {
                //Edit
                var item = data.FirstOrDefault();
                _context.DeXuatChiTiet.Remove(item);
                await _context.SaveChangesAsync();
                await _actionLogsRepository.AddLogAsync(new ActionLogs
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    UserName = request.CreatorName,
                    Content = JsonConvert.SerializeObject(request),
                    Action = "Remove",
                    FunctionUnique = "DeXuatChiTiet",
                    UserId = request.Creator
                }).ConfigureAwait(false);
            }

            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
            response.Message = "Xóa thông tin đề xuất chi tiết thành công!";

            return response;
        }
    }
}
