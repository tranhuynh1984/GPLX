using System;

namespace GPLX.Database.Models
{
    public class InvestmentPlanLogs
    {
        public Guid Id { get; set; }
        public Guid InvestmentPlanId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Creator { get; set; }
        public string CreatorName { get; set; }
        public string PositionName { get; set; }
        public int PositionId { get; set; }

        public int FromStatus { get; set; }
        public int ToStatus { get; set; }
        public string ToStatusName { get; set; }
        public string Reason { get; set; }
    }
}
