using System.Collections.Generic;
using GPLX.Core.DTO.Response.CostEstimateItem;
using GPLX.Core.Model;

namespace GPLX.Core.DTO.Request.CostEstimate
{
    public class PreviewCostEstimateRequest
    {
        public string Record { get; set; }
        public List<LuckySheetCellModel> Data { get; set; }
        public List<CostEstimateItemSearchResponseData> Request { get; set; }

        public int Week { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string PageRequest { get; set; }
        public bool IsSub { get; set; }
        public string Type { get; set; }
        public bool IsApprove { get; set; }
        public string Reason { get; set; }
    }
}
