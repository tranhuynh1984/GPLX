using System.Collections.Generic;
using GPLX.Database.Models;

namespace GPLX.Core.DTO.Request.RevenuePlan
{
    public class RevenuePlanCreateRequest
    {
        public IList<RevenuePlanAggregate> RevenuePlanAggregates { get; set; }
        public IList<RevenuePlanCustomerDetails> RevenuePlanCustomers { get; set; }
        public IList<RevenuePlanCashDetails> RevenuePlanCash { get; set; }
        public Database.Models.RevenuePlan RevenuePlan { get; set; }

        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string CreatorName { get; set; }
        public int Creator { get; set; }
        public bool IsSub { get; set; }
    }
}
