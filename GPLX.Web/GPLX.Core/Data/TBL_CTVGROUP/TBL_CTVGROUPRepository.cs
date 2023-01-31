using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.TBL_CTVGROUP;
using GPLX.Core.DTO.Request.TBL_CTVGROUP;
using GPLX.Core.DTO.Response.TBL_CTVGROUP;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace GPLX.Core.Data.TBL_CTVGROUP
{
    public class TBL_CTVGROUPRepository : ITBL_CTVGROUPRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TBL_CTVGROUPRepository> _logger;
        private readonly IActionLogsRepository _actionLogsRepository;

        public TBL_CTVGROUPRepository(Context context, IMapper mapper, ILogger<TBL_CTVGROUPRepository> logger, IActionLogsRepository actionLogsRepository)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
            _actionLogsRepository = actionLogsRepository;
        }

        public async Task<TBL_CTVGROUPSearchResponse> Search(int skip, int length, TBL_CTVGROUPSearchRequest request)
        {
            var response = new TBL_CTVGROUPSearchResponse { Draw = request.Draw };

            var query = _context.TBL_CTVGROUP.AsNoTracking();
            if (!string.IsNullOrEmpty(request.CTVGroupID))
                query = query.Where(x => x.CTVGroupID.ToString().Contains(request.CTVGroupID.ToLower()));
            if (!string.IsNullOrEmpty(request.CTVGroupName))
                query = query.Where(x => x.CTVGroupName.Contains(request.CTVGroupName.ToLower()));

            var data = await query.OrderBy(x => x.CTVGroupName).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<TBL_CTVGROUPSearchResponseData>();

            foreach (var d in data.Skip(skip).Take(length))
            {
                var dMap = _mapper.Map<TBL_CTVGROUPSearchResponseData>(d);
                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<TBL_CTVGROUPSearchResponse> GetAllDM()
        {
            var response = new TBL_CTVGROUPSearchResponse { };

            var query = _context.TBL_CTVGROUP.AsQueryable();
            var data = await query.OrderBy(x => x.CTVGroupName).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<TBL_CTVGROUPSearchResponseData>();

            foreach (var d in data)
            {
                var dMap = _mapper.Map<TBL_CTVGROUPSearchResponseData>(d);
                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<TBL_CTVGROUPSearchResponse> SearchAll(TBL_CTVGROUPSearchRequest request)
        {
            var response = new TBL_CTVGROUPSearchResponse { Draw = request.Draw };

            var query = _context.TBL_CTVGROUP.AsQueryable();
            if (!string.IsNullOrEmpty(request.CTVGroupID))
                query = query.Where(x => x.CTVGroupID.ToString().Contains(request.CTVGroupID.ToLower()));
            if (!string.IsNullOrEmpty(request.CTVGroupName))
                query = query.Where(x => x.CTVGroupName.Contains(request.CTVGroupName.ToLower()));

            var data = await query.OrderBy(x => x.CTVGroupName).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<TBL_CTVGROUPSearchResponseData>();

            for (var i = 0; i < data.Count; i++)
            {
                var d = data[i];
                var dMap = _mapper.Map<TBL_CTVGROUPSearchResponseData>(d);

                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<TBL_CTVGROUPSearchResponseData> GetById(int ctvGroupID)
        {
            var record = await _context.TBL_CTVGROUP.FirstOrDefaultAsync(x => x.CTVGroupID == ctvGroupID);
            if (record == null)
                return null;
            var response = _mapper.Map<TBL_CTVGROUPSearchResponseData>(record);
            return response;
        }

        
    }
}
