using System;

namespace GPLX.Database.Models
{
    public class CostEstimate
    {
		public Guid Id { get; set; }
		/// <summary>
		/// Tiền mặt
		/// </summary>
		public double Cash { get; set; }

		/// <summary>
		/// Thu từ HĐ kinh doanh
		/// </summary>
        public double OperatingRevenue { get; set; }
		/// <summary>
		/// Thu từ góp vốn
		/// </summary>
        public double ContributionRevenue { get; set; }
		/// <summary>
		/// Thu khác
		/// </summary>
        public double DiffRevenue { get; set; }
		/// <summary>
		/// Ngân hàng TCB công ty
		/// </summary>
        public double BankTCB { get; set; }
		/// <summary>
		/// Ngân hàng BIDV cty
		/// </summary>
		public double BankBIDV { get; set; } 
		/// <summary>
		/// Tài khoản BIDV chuyên thu
		/// </summary>
		public double BankBIDVRevenue { get; set; }
		/// <summary>
		/// Vay lưu động – số dư khả dụng
		/// </summary>
		public double LoanAvailable { get; set; }

		/// <summary>
		/// Số dư khả dụng đầu kỳ (1)
		/// </summary>
		public double BeginAvailableBalance { get; set; }
		/// <summary>
		/// Số dư TK thanh toán
		/// </summary>
		public double AccountBalance { get; set; }
		/// <summary>
		/// Số tiền thu dự kiến (2)
		/// </summary>
		public double ExpectRevenue { get; set; }
		/// <summary>
		/// Số tiền chi phí dự kiến (3)
		/// </summary>
		public double EstimatedCost { get; set; }
		/// <summary>
		/// Các khoản chi hoạt động
		/// </summary>
		public double OperatingCost { get; set; }
		/// <summary>
		/// Các khoản chi đầu tư
		/// </summary>
		public double InvestmentCost { get; set; }
		/// <summary>
		/// Các khoản chi hoạt động tài chính
		/// </summary>
		public double FinancialCost { get; set; }
		/// <summary>
		/// Tổng chi bằng vốn tự có
		/// </summary>
		public double TotalExpenditureCapital { get; set; }
		/// <summary>
		/// Tổng chi bằng vay lưu động
		/// </summary>
		public double TotalSpendingLoan { get; set; }

		/// <summary>
		/// Số dư khả dụng cuối kỳ 
		/// </summary>
		public double EndAvailableBalance { get; set; }

		/// <summary>
		/// Vốn tự có (4)
		/// </summary>
		public double Funds { get; set; }
		/// <summary>
		/// Số dư khả dụng vay lưu động
		/// </summary>
		public double WorkingBalanceCost { get; set; }

		/// <summary>
		/// Định mức tồn quỹ (5)
		/// </summary>
		public double EquityCost { get; set; }

		/// <summary>
		/// Dự kiến cắt tiền về dòng tiền tập trung (4-5)
		/// </summary>
		public double PlanCutCost { get; set; }
		/// <summary>
		/// Đề xuất luân chuyển tiền về ĐVTV
		/// </summary>
		public double RotationProposal { get; set; }
		/// <summary>
		/// Chuyển về T3
		/// </summary>
		public double RotationProposalTue { get; set; }
		/// <summary>
		/// Chuyển về T5
		/// </summary>
		public double RotationProposalWe { get; set; }


		public int UserId { get; set; }
		public string UserName { get; set; }
        public int? UpdaterId { get; set; }
        public string UpdaterName { get; set; }

		public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
		public int Status { get; set; }
		public string StatusName { get; set; }
		public string CostEstimateBody { get; set; }
		/// <summary>
		/// Tuần báo cáo
		/// </summary>
        public int ReportForWeek { get; set; }
        public string ReportForWeekName { get; set; }

        public string UnitName { get; set; }
        public int UnitId { get; set; }
        public bool IsSub { get; set; }
        /// <summary>
        /// Loại dự trù
        /// Tuần - Năm
        /// </summary>
        public int CostEstimateType { get; set; }
		public string CostEstimateTypeName { get; set; }

        public string PathExcel { get; set; }

        public string PathPdf { get; set; }
    }
}
