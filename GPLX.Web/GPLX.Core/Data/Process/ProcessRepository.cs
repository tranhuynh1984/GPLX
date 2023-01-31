using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.Process;
using GPLX.Core.DTO.Request.Process;
using GPLX.Core.DTO.Response.Process;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace GPLX.Core.Data.Process
{
    public class ProcessRepository : IProcessRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ProcessRepository> _logger;
        private readonly IActionLogsRepository _actionLogsRepository;

        public ProcessRepository(Context context, IMapper mapper, ILogger<ProcessRepository> logger, IActionLogsRepository actionLogsRepository)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
            _actionLogsRepository = actionLogsRepository;
        }

        public async Task<ProcessSearchResponse> Search()
        {
            var response = new ProcessSearchResponse ();
           
            var query = _context.Process.AsNoTracking();
            var data = await query.OrderBy(x => x.ProcessId).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<ProcessSearchResponseData>();

            foreach (var d in data)
            {
                var dMap = _mapper.Map<ProcessSearchResponseData>(d);
                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        
    }
}
