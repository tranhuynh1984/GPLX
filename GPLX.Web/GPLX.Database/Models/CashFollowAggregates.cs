using System;

namespace GPLX.Database.Models
{
    public class CashFollowAggregates
    {
        public Guid Id  { get; set; }
        public Guid CashFollowId  { get; set; }
        public int CashFollowGroupId { get; set; }
        public string CashFollowGroupName { get; set; }
        public string No { get; set; }
        public double Q1 { get; set; }
        public double Q2 { get; set; }
        public double Q3 { get; set; }
        public double Q4 { get; set; }
        public double Total { get; set; }
    }
}
