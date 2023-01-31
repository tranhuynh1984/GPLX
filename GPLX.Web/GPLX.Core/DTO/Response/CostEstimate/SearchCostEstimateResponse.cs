using System;
using System.Collections.Generic;

namespace GPLX.Core.DTO.Response.CostEstimate
{
    public class SearchCostEstimateResponseData
    {
		public string Record { get; set; }
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
		/// Chi hoạt động thường quy
		/// </summary>
		public double RoutineCost { get; set; }
		/// <summary>
		/// Chi hoạt động không thường quy
		/// </summary>
		public double NonRoutineCost { get; set; }
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
		/// Số dư khả dụng cuối kỳ (1+2-3)
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


		public int UserId { get; set; }
		public string UserName { get; set; }

		public DateTime CreatedDate { get; set; }
		public DateTime UpdatedDate { get; set; }
		public string Status { get; set; }

		/// <summary>
		/// Tuần báo cáo
		/// </summary>
		public int ReportForWeek { get; set; }
		public string ReportForWeekName { get; set; }
		 
        public bool Viewable { get; set; }
        public bool Editable { get; set; }
        public bool ApproveAble { get; set; }
        public bool DeclineAble { get; set; }
		/// <summary>
		/// Loại dự trù năm - tuần
		/// </summary>
        public int Type { get; set; }

        public string PathPdf { get; set; }
        public string UnitName { get; set; }
	}

    public class SearchCostEstimateResponse
    {
        public int Code { get; set; }
        public string  Message { get; set; }
        public IList<SearchCostEstimateResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
	}
}
