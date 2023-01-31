using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.DTO.Response.CostEstimateItem;
using GPLX.Core.Model;

namespace GPLX.Core.DTO.Request.CostEstimate
{
    public class CostEstimateRequest
    {
        public string record { get; set; }
        public List<LuckySheetCellModel> data { get; set; }
        public List<CostEstimateItemSearchResponseData> request { get; set; }
        public string type { get; set; }
        public int week { get; set; }
    }
}
