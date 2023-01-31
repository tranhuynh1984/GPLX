using System;

namespace GPLX.Database.Models
{
    /// <summary>
    /// Bảng chính lưu dữ liệu kế hoạch đầu tư
    /// </summary>
    public class InvestmentPlan
    {
        public Guid Id { get; set; }
        public int UnitId { get; set; }

        /// <summary>
        /// Năm kế hoạch
        /// </summary>
        public int Year { get; set; }
        public string UnitName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Creator { get; set; }
        public string CreatorName { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        /// <summary>
        /// Nếu là SUB thì không có cột vốn của MG
        /// </summary>
        public bool IsSub { get; set; }

        public string PathPdf { get; set; }
        public string PathExcel { get; set; }


        /// <summary>
        /// Số tiền đầu tư dự kiến
        /// </summary>
        public double TotalExpectCostInvestment { get; set; }

        /// <summary>
        /// Vốn tự có
        /// </summary>
        public double TotalExpenditureCapital { get; set; }
        /// <summary>
        /// Đầu tư của MG
        /// </summary>
        public double TotalCapitalMedGroup { get; set; }
        /// <summary>
        /// Vốn vay lưu động
        /// </summary>
        public double TotalSpendingLoan { get; set; }
    }
}
