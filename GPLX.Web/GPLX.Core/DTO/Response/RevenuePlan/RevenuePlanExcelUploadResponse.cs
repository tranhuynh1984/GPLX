using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Database.Models;

namespace GPLX.Core.DTO.Response.RevenuePlan
{
    public class RevenuePlanExcelUploadResponse
    {
        public IList<RevenuePlanAggregate> RevenuePlanAggregates { get; set; }
        public IList<RevenuePlanCustomerDetails> RevenuePlanCustomers { get; set; }
        public IList<RevenuePlanCashDetails> RevenuePlanCash { get; set; }
        public Database.Models.RevenuePlan RevenuePlan { get; set; }
    }
}
