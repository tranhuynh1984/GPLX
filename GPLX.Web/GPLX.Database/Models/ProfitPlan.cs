using System;

namespace GPLX.Database.Models
{
    public class ProfitPlan
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Creator { get; set; }
        public string CreatorName { get; set; }
        public int Year { get; set; }
        public string StatusName { get; set; }
        public int Status { get; set; }
        public string ProfitPlanType { get; set; }

        public string PathExcel { get; set; }
        public string PathPdf { get; set; }

        public bool IsSub { get; set; }
        public string UnitName { get; set; }
        public int UnitId { get; set; }
        /// <summary>
        /// Tống doanh thu
        /// </summary>
        public double TotalRevenue { get; set; }
        /// <summary>
        /// Tổng chi phí
        /// </summary>
        public double TotalExpense { get; set; }
        /// <summary>
        /// Lợi nhuận sau thuế
        /// </summary>
        public double TotalProfitTax { get; set; }
        /// <summary>
        /// TL/DT của chi phí
        /// </summary>
        public double ProportionExpense { get; set; }
        /// <summary>
        /// TL/DT lợi nhuận
        /// </summary>
        public double ProportionProfitTax { get; set; }
    }
}
