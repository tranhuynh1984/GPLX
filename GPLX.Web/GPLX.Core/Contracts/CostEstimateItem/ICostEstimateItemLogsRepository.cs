using System.Collections.Generic;
using System.Threading.Tasks;
using GPLX.Core.DTO.Request.CostEstimateItem;
using GPLX.Core.DTO.Response.CostEstimateItem;

namespace GPLX.Core.Contracts.CostEstimateItem
{
    public interface ICostEstimateItemLogsRepository
    {
        public Task<IList<CostEstimateItemLogResponse>> ViewHistories(ViewHistoryRequest request);
    }
}
