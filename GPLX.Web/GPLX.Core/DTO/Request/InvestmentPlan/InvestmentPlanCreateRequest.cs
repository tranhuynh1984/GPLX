using System.Collections.Generic;
using GPLX.Database.Models;

namespace GPLX.Core.DTO.Request.InvestmentPlan
{
    public class InvestmentPlanCreateRequest
    {
        public IList<InvestmentPlanAggregate> InvestmentPlanAggregates { get; set; }
        public IList<InvestmentPlanDetails> InvestmentPlanDetails { get; set; }

        public Database.Models.InvestmentPlan InvestmentPlan { get; set; }

        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string UnitType { get; set; }
        public string CreatorName { get; set; }
        public int Creator { get; set; }
        public bool IsSub { get; set; }
    }
}
