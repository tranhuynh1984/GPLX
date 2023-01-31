using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.CostEstimateItem;
using GPLX.Core.DTO.Request.CostEstimateItem;
using GPLX.Core.DTO.Response.CostEstimateItem;
using GPLX.Core.Enum;
using GPLX.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GPLX.Core.Data.CostEstimateItem
{
    public class CostEstimateItemLogsRepository : ICostEstimateItemLogsRepository
    {
        private readonly ILogger<CostEstimateItemLogsRepository> _logger;
        private readonly Context _ctx;

        public CostEstimateItemLogsRepository(ILogger<CostEstimateItemLogsRepository> logger, Context ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }
        public async Task<IList<CostEstimateItemLogResponse>> ViewHistories(ViewHistoryRequest request)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.CostEstimateRawId))
                {
                    Guid gParse = Guid.Parse(request.CostEstimateRawId);
                    var query = _ctx.CostEstimateItemLogs.
                        Where(x => x.CostEstimateItemId == gParse).
                        OrderByDescending(x => x.CreatedDate)
                        .Select(x => new CostEstimateItemLogResponse
                        {
                            PositionName = x.PositionName,
                            Status = x.ToStatusName,
                            TimeChange = x.CreatedDate.ToString("dd/MM/yyyy HH:mm"),
                            UserName = x.UserName,
                            Reason = x.Reason
                        });
                    return (await query.ToListAsync());
                }
                return new List<CostEstimateItemLogResponse>();
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message, request);
                return new List<CostEstimateItemLogResponse>(); ;
            }
        }
    }
}
