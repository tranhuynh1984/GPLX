using System;
using System.Collections.Generic;
using GPLX.Core.DTO.Response.CostStatus;

namespace GPLX.Core.DTO.Request.CostEstimate
{
    public class CostEstimateUpdate
    {
        public Guid id { get; set; }
        /// <summary>
        /// Số dư khả dụng đầu kỳ (1)
        /// </summary>
        public long BeginAvailableBalance { get; set; }
		/// <summary>
		/// Số dư TK thanh toán
		/// </summary>
		public long AccountBalance { get; set; }
		/// <summary>
		/// Số tiền thu dự kiến (2)
		/// </summary>
		public long ExpectRevenue { get; set; }
		/// <summary>
		/// Số tiền chi phí dự kiến (3)
		/// </summary>
		public long EstimatedCost { get; set; }
		/// <summary>
		/// Các khoản chi hoạt động
		/// </summary>
		public long OperatingCost { get; set; }
		/// <summary>
		/// Chi hoạt động thường quy
		/// </summary>
		public long RoutineCost { get; set; }
		/// <summary>
		/// Chi hoạt động không thường quy
		/// </summary>
		public long NonRoutineCost { get; set; }
		/// <summary>
		/// Các khoản chi đầu tư
		/// </summary>
		public long InvestmentCost { get; set; }
		/// <summary>
		/// Các khoản chi hoạt động tài chính
		/// </summary>
		public long FinancialCost { get; set; }
		/// <summary>
		/// Tổng chi bằng vốn tự có
		/// </summary>
		public long TotalExpenditureCapital { get; set; }
		/// <summary>
		/// Tổng chi bằng vay lưu động
		/// </summary>
		public long TotalSpendingLoan { get; set; }

		/// <summary>
		/// Số dư khả dụng cuối kỳ (1+2-3)
		/// </summary>
		public long EndAvailableBalance { get; set; }

		/// <summary>
		/// Vốn tự có (4)
		/// </summary>
		public long Funds { get; set; }
		/// <summary>
		/// Số dư khả dụng vay lưu động
		/// </summary>
		public long WorkingBalanceCost { get; set; }

		/// <summary>
		/// Định mức tồn quỹ (5)
		/// </summary>
		public long EquityCost { get; set; }

		/// <summary>
		/// Dự kiến cắt tiền về dòng tiền tập trung (4-5)
		/// </summary>
		public long PlanCutCost { get; set; }
		/// <summary>
		/// Đề xuất luân chuyển tiền về ĐVTV
		/// </summary>
		public long RotationProposal { get; set; }

        public string ExcelFile { get; set; }

        public string PdfFile { get; set; }

        public string CostEstimateBody { get; set; }

		public IList<StatusesGranted> StatusAllowsSeen { get; set; }
        public bool PermissionEdit { get; set; }

        public bool IsSub { get; set; }

	}

	//public class CostEstimateUpdateRequestValidator : AbstractValidator<CostEstimateUpdateRequest>
	//{
	//	public CostEstimateUpdateRequestValidator()
	//	{
	//		RuleFor(m => m.BeginAvailableBalance).NotEmpty().WithMessage("Số dư khả dụng đầu kỳ là bắt buộc");
	//		RuleFor(m => m.AccountBalance).NotEmpty().WithMessage("Số dư TK thanh toán là bắt buộc");
	//		RuleFor(m => m.ExpectRevenue).NotEmpty().WithMessage("Số tiền thu dự kiến là bắt buộc");
	//		RuleFor(m => m.EstimatedCost).NotEmpty().WithMessage("Số dư TK thanh toán là bắt buộc");
	//		RuleFor(m => m.OperatingCost).NotEmpty().WithMessage("Số dư TK thanh toán là bắt buộc");
	//		RuleFor(m => m.RoutineCost).NotEmpty().WithMessage("Số dư TK thanh toán là bắt buộc");
	//		RuleFor(m => m.NonRoutineCost).NotEmpty().WithMessage("Số dư TK thanh toán là bắt buộc");
	//		RuleFor(m => m.InvestmentCost).NotEmpty().WithMessage("Số dư TK thanh toán là bắt buộc");
	//		RuleFor(m => m.FinancialCost).NotEmpty().WithMessage("Số dư TK thanh toán là bắt buộc");
	//		RuleFor(m => m.TotalExpenditureCapital).NotEmpty().WithMessage("Số dư TK thanh toán là bắt buộc");
	//		RuleFor(m => m.TotalSpendingLoan).NotEmpty().WithMessage("Tổng chi bằng vay lưu động là bắt buộc");
	//		RuleFor(m => m.EndAvailableBalance).NotEmpty().WithMessage("Số dư khả dụng cuối kỳ là bắt buộc");
	//		RuleFor(m => m.Funds).NotEmpty().WithMessage("Số dư khả dụng vay lưu động là bắt buộc");
	//		RuleFor(m => m.WorkingBalanceCost).NotEmpty().WithMessage("Số dư TK thanh toán là bắt buộc");
	//		RuleFor(m => m.EquityCost).NotEmpty().WithMessage(" Định mức tồn quỹ là bắt buộc");
	//		RuleFor(m => m.PlanCutCost).NotEmpty().WithMessage("Dự kiến cắt tiền về dòng tiền tập trung là bắt buộc");
	//		RuleFor(m => m.RotationProposal).NotEmpty().WithMessage("Đề xuất luân chuyển tiền về ĐVTV là bắt buộc");

	//	}
	//}
}
