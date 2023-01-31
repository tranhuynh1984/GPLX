using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.DTO.Response.InvestmentPlan
{
    public class InvestmentPlanApproveResponse
    {
        public SearchInvestmentPlanResponseData Data { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
