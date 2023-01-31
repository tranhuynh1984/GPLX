using System.Collections.Generic;
using GPLX.Core.DTO.Response.ProfitPlan;

namespace GPLX.Core.DTO.Request.Profit
{
    public class ProfitPlanCreateRequest
    {
        public IList<ProfitPlanAggregatesExcel> ProfitPlanAggregates { get; set; }
        public IList<ProfitPlanDetailExcel> ProfitPlanDetails { get; set; }
        public Database.Models.ProfitPlan ProfitPlan { get; set; }

        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string CreatorName { get; set; }
        public int Creator { get; set; }
        public bool IsSub { get; set; }
    }
}
