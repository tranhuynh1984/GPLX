using GPLX.Core.Extensions;

namespace GPLX.Core.DTO.Request.CostEstimateItem
{
    public class ViewHistoryRequest
    {
        public string CostEstimateId { get; set; }
        public string PageRequest { get; set; }

        public string CostEstimateRawId => CostEstimateId.StringAesDecryption(PageRequest);
    }
}
