using System.Collections.Generic;
using GPLX.Database.Models;

namespace GPLX.Core.DTO.Response.InvestmentPlan
{
    public class InvestmentPlanExcelResponse
    {
        public IList<InvestmentPlanAggregate> InvestmentPlanAggregates { get; set; }
        public IList<InvestmentPlanDetails> InvestmentPlanDetails { get; set; }

        public Database.Models.InvestmentPlan InvestmentPlan { get; set; }

    }
}
