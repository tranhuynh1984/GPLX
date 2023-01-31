using System;
using System.Collections.Generic;
using GPLX.Core.DTO.Response.CostEstimateItem;

namespace GPLX.Core.DTO.Request.CostEstimate
{
    /// <summary>
    /// Dùng để tạo excel dự trù
    /// </summary>
    public class GenerateExcelCreateCostEstimateModel
    {
        public List<CostEstimateItemSearchResponseData> Operating { get; set; }
        public List<CostEstimateItemSearchResponseData> Investment { get; set; }
        public List<CostEstimateItemSearchResponseData> Finance { get; set; }
        public List<CostEstimateItemSearchResponseData> NOperating { get; set; }
        public List<CostEstimateItemSearchResponseData> NInvest { get; set; }
        public List<CostEstimateItemSearchResponseData> NFinance { get; set; }
        public List<Database.Models.CostEstimateItem> MissingSpent { get; set; }
    }
}
