using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.DeXuatGhiChu;
using GPLX.Core.DTO.Request.DeXuatGhiChu;
using GPLX.Core.DTO.Response.DeXuatGhiChu;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace GPLX.Core.Data.DeXuatGhiChu
{
    public class DeXuatGhiChuRepository : IDeXuatGhiChuRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DeXuatGhiChuRepository> _logger;
        private readonly IActionLogsRepository _actionLogsRepository;

        public DeXuatGhiChuRepository(Context context, IMapper mapper, ILogger<DeXuatGhiChuRepository> logger, IActionLogsRepository actionLogsRepository)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
            _actionLogsRepository = actionLogsRepository;
        }

        public async Task<DeXuatGhiChuSearchResponse> Search(DeXuatGhiChuSearchRequest request)
        {
            var response = new DeXuatGhiChuSearchResponse { Draw = request.Draw };
            var query = _context.DeXuatGhiChu.AsNoTracking();
            var data = await query.OrderBy(x => x.DeXuatCode).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<DeXuatGhiChuSearchResponseData>();

            foreach (var d in data)
            {
                var dMap = _mapper.Map<DeXuatGhiChuSearchResponseData>(d);
                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<DeXuatGhiChuCreateResponse> Create(DeXuatGhiChuCreateRequest request)
        {
            var response = new DeXuatGhiChuCreateResponse();

            var query = _context.DeXuatGhiChu.AsQueryable();
            query = query.Where(x => x.DeXuatCode == request.DeXuatCode && x.ProcessStepId == request.ProcessStepId);
            var data = await query.OrderBy(x => x.ProcessStepId).ToListAsync();

            if (data.Count > 0)
            {
                //Edit
                var item = data.FirstOrDefault();
                item.DeXuatCode = request.DeXuatCode;
                item.ProcessStepId = request.ProcessStepId;
                item.Note = request.Note;
                item.CreateByCode = request.CreateByCode;
                item.CreateByName = request.CreateByName;
                item.CreateDate = System.DateTime.Now;
                await _context.SaveChangesAsync();
                await _actionLogsRepository.AddLogAsync(new ActionLogs
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    UserName = request.CreatorName,
                    Content = JsonConvert.SerializeObject(request),
                    Action = "Edit",
                    FunctionUnique = "DeXuatGhiChu",
                    UserId = request.Creator
                }).ConfigureAwait(false);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Message = "Cập nhật thông tin đề xuất ghi chú thành công!";
            }
            else
            {
                //Add New
                var item = new Database.Models.DeXuatGhiChu();

                item.DeXuatCode = request.DeXuatCode;
                item.ProcessStepId = request.ProcessStepId;
                item.Note = request.Note;
                item.CreateByCode = request.CreateByCode;
                item.CreateByName = request.CreateByName;
                item.CreateDate = System.DateTime.Now;
                _context.DeXuatGhiChu.Add(item);
                await _context.SaveChangesAsync();
                await _actionLogsRepository.AddLogAsync(new ActionLogs
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    UserName = request.CreatorName,
                    Content = JsonConvert.SerializeObject(request),
                    Action = "Add",
                    FunctionUnique = "DeXuatGhiChu",
                    UserId = request.Creator
                }).ConfigureAwait(false);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Message = "Thêm mới thông tin đề xuất ghi chú thành công!";
            }

            return response;
        }
        
        public async Task<List<Database.Models.DeXuatGhiChu>> FindAllByCode(string code)
        {
            var query = _context.DeXuatGhiChu.AsNoTracking();
            return await query
                .Where(x => x.DeXuatCode == code.Trim())
                .OrderBy(x => x.DeXuatCode).ToListAsync().ConfigureAwait(false);
        }
    }
}
