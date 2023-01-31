using AutoMapper;
using GPLX.Core.Contracts.NhCP;
using GPLX.Core.DTO.Response.NhCP;
using GPLX.Core.Enum;
using GPLX.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPLX.Core.Data.NhCP
{
    public class NhCPRepository : INhCPRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<NhCPRepository> _logger;

        public NhCPRepository(Context context, IMapper mapper, ILogger<NhCPRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<NhCPSearchResponse> GetAll()
        {
            var response = new NhCPSearchResponse();
            try
            {
                var query = _context.NhCP.AsQueryable();

                var data = await query.OrderBy(x => x.MaNhCP).ToListAsync().ConfigureAwait(true);

                response.RecordsFiltered = data.Count;
                response.RecordsTotal = data.Count;

                var dataResponse = new List<NhCPSearchResponseData>();

                foreach (var d in data)
                {
                    var dMap = _mapper.Map<NhCPSearchResponseData>(d);
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

        public async Task<List<NhCPSearchResponseData>> GetCategory()
        {
            var query = _context.NhCP.AsQueryable();
            var data = await query.OrderBy(x => x.MaNhCP).Select(x => new NhCPSearchResponseData() { MaNhCP = x.MaNhCP, TenNhCP = x.TenNhCP, TenNhCPFull = x.MaNhCP + "-" + x.TenNhCP }).ToListAsync().ConfigureAwait(true);
            return data;
        }
    }
}