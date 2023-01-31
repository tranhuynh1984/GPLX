using System.Collections.Generic;
using System.Threading.Tasks;
using GPLX.Core.DTO.Request.CostEstimate;
using GPLX.Core.DTO.Response.CostEstimate;

namespace GPLX.Core.Contracts.CostEstimate
{
    public interface ICostEstimateLogRepository
    {
        public Task<IList<CostEstimateLogResponse>> ViewHistories(CostEstimateLogRequest request);

    }
}
