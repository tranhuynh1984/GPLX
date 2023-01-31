using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Database.Models
{
    /// <summary>
    /// Bảng chi tiết kế hoạch đầu tư
    /// </summary>
    public class InvestmentPlanDetails
    {
        public Guid Id { get; set; }
        public Guid InvestmentPlanId { get; set; }
        /// <summary>
        /// Nội dung đầu tư
        /// </summary>
        public string InvestContent { get; set; }
        /// <summary>
        /// Nội dung
        /// Mapping với bảng
        /// InvestmentPlanContents
        /// </summary>
        public int InvestmentPlanContentId { get; set; }
        public string InvestmentPlanContentName { get; set; }

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        /// <summary>
        /// Thời gian đầu tư dự kiến
        /// </summary>
        public DateTime ExpectedTime { get; set; }

        /// <summary>
        /// Số tiền dự kiến
        /// </summary>
        public double CostExpected { get; set; }

        /// <summary>
        /// Phương án vốn
        /// </summary>
        public int CapitalPlanId { get; set; }
        public string CapitalPlanName { get; set; }
        /// <summary>
        /// Tỉ lệ vốn vay (nếu có)
        /// Mặc định -1
        /// </summary>
        public double SpendingLoanPercent { get; set; }
        /// <summary>
        /// Số tiền vay dự kiến
        /// </summary>
        public double SpendingLoan { get; set; }
        /// <summary>
        /// Chú thích
        /// </summary>
        public string Explanation { get; set; }
    }
}
