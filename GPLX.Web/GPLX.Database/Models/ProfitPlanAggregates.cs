using System;

namespace GPLX.Database.Models
{
    public class ProfitPlanAggregates
    {
        public Guid Id { get; set; }
        public Guid ProfitPlanId { get; set; }
        public int ProfitPlanGroupId { get; set; }
        public string ProfitPlanGroupName { get; set; }
        /// <summary>
        /// Tống doanh thu
        /// </summary>
        public double TotalCosh { get; set; }
        /// <summary>
        /// Tổng chi phí
        /// </summary>
        public double Proportion { get; set; }
    }
}
