using System;
using System.Globalization;

namespace GPLX.Database.Models
{
    public class InvestmentPlanAggregate
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Id của bảng chính
        /// </summary>

        public Guid InvestmentPlanId { get; set; }
        /// <summary>
        /// Nội dung
        /// Mapping với bảng
        /// InvestmentPlanContents
        /// </summary>
        public int InvestmentPlanContentId { get; set; }
        public string InvestmentPlanContentName { get; set; }
        /// <summary>
        /// Số tiền đầu tư dự kiến
        /// </summary>
        public double ExpectCostInvestment { get; set; }

        /// <summary>
        /// Tổng hợp phương án vốn
        /// Lưu lại dưới dạng JSON
        /// </summary>
        public string CapitalPlanAggregate { get; set; }

        /// <summary>
        /// Vốn tự có
        /// </summary>
        public double ExpenditureCapital { get; set; }
        /// <summary>
        /// Đầu tư của MG
        /// </summary>
        public double CapitalMedGroup { get; set; }
        /// <summary>
        /// Vốn vay lưu động
        /// </summary>
        public double SpendingLoan { get; set; }
    }
}
