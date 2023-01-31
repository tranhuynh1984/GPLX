using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPLX.Core.DTO.Entities
{
    public class ExportCostEstimateModel
    {
        /// <summary>
        /// Tên dự trù
        /// Dự trù tuần 30  - ...
        /// </summary>
        public string HeaderName { get; set; }

        // format: _(* #,##0_);_(* (#,##0);_(* "-"??_);_(@_)
        /// <summary>
        /// Số dư khả dụng đầu kỳ
        /// </summary>
        public long BeginAvailableBalance => Cash + AccountBalance + SpendingLoanAvailable;

        public IList<ExportingChild> BeginAvailableBalanceChild { get; set; }

        /// <summary>
        /// Số dư tài khoản thanh toán
        /// </summary>
        public long AccountBalance => AccountBalanceChild == null || !AccountBalanceChild.Any() ? 0 : AccountBalanceChild.Sum(x => x.Cost);
        // todo
        public IList<ExportingChild> AccountBalanceChild { get; set; }

        /// <summary>
        /// Số tiền thu dự kiến (2)
        /// </summary>
        public long ExpectRevenue => ExpectRevenueChild == null || !ExpectRevenueChild.Any() ? 0 : ExpectRevenueChild.Sum(x => x.Cost);
        /// <summary>
        /// Các mục nhỏ của số tiền thu dự kiến
        /// </summary>
        public IList<ExportingChild> ExpectRevenueChild { get; set; }

        /// <summary>
        /// Số tiền chi phí dự kiến (3)
        /// </summary>
        public long EstimatedCost => OperatingCost + InvestmentCost + FinancialCost;

        /// <summary>
        /// Các khoản chi hoạt động
        /// </summary>
        public long OperatingCost => OperatingChild == null || !OperatingChild.Any() ? 0 : OperatingChild.Sum(x => x.Cost);

        /// <summary>
        /// Chi hoạt động 
        /// </summary>
        public IList<ExportingChild> OperatingChild { get; set; }

        


        /// <summary>
        /// Các khoản chi đầu tư
        /// </summary>
        public long InvestmentCost => InvestmentChild == null || !InvestmentChild.Any() ? 0 : InvestmentChild.Sum(x => x.Cost);
        public IList<ExportingChild> InvestmentChild { get; set; }

        /// <summary>
        /// Các khoản chi hoạt động tài chính
        /// </summary>
        public long FinancialCost => FinancialChild == null || !FinancialChild.Any() ? 0 : FinancialChild.Sum(x => x.Cost);
        public IList<ExportingChild> FinancialChild { get; set; }

        /// <summary>
        /// Tổng chi bằng vốn tự có
        /// </summary>
        public long TotalExpenditureCapital => SumWithType("Vốn tự có");
        /// <summary>
        /// Tổng chi bằng vay lưu động
        /// </summary>
        public long TotalSpendingLoan => SumWithType("Vốn vay");

        /// <summary>
        /// Số dư khả dụng cuối kỳ
        /// </summary>
        public long EndAvailableBalance => Funds + WorkingBalanceCost;

        /// <summary>
        /// Tiền mặt
        /// </summary>
        public long Cash { get; set; }

        //todo
        /// <summary>
        /// Vay lưu động số dư khả dụng
        /// </summary>
        public long SpendingLoanAvailable { get; set; }

        /// <summary>
        /// Vốn tự có (4)
        /// </summary>
        public long Funds => AccountBalance + Cash + ExpectRevenue - TotalExpenditureCapital;

        /// <summary>
        /// Số dư khả dụng vay lưu động
        /// </summary>
        public long WorkingBalanceCost => SpendingLoanAvailable - TotalSpendingLoan;

        /// <summary>
        /// Định mức tồn quỹ (5)
        /// </summary>
        public long EquityCost { get; set; }

        /// <summary>
        /// Dự kiến cắt tiền về dòng tiền tập trung (4-5)
        /// </summary>
        public long PlanCutCost => Funds - EquityCost;
        /// <summary>
        /// Đề xuất luân chuyển tiền về đơn vị thành viên
        /// </summary>
        public long RotationProposal => RotationChild == null || !RotationChild.Any() ? 0 : RotationChild.Sum(x => x.Cost);

        public IList<ExportingChild> RotationChild { get; set; }

        long SumWithType(string type)
        {
            var fromOperating = OperatingChild != null && OperatingChild.Any() ?
                OperatingChild.Where(x => x.PayForm?.Equals(type, StringComparison.OrdinalIgnoreCase) == true).Sum(x => x.Cost) : 0.0;
            var fromInvestment = InvestmentChild != null && InvestmentChild.Any() ?
                InvestmentChild.Where(x => x.PayForm?.Equals(type, StringComparison.OrdinalIgnoreCase) == true).Sum(x => x.Cost) : 0.0;
            var fromFinancial = FinancialChild != null && FinancialChild.Any() ?
                FinancialChild.Where(x => x.PayForm?.Equals(type, StringComparison.OrdinalIgnoreCase) == true).Sum(x => x.Cost) : 0.0;

            return (long)(fromOperating + fromInvestment + fromFinancial);
        }

        public IList<ExportingChild> ListNoPentChild { get; set; }
        public string NoSpentTitle { get; set; }

        public bool IsSub { get; set; }

    }

    public class ExportingChild
    {
        /// <summary>
        /// Số thứ tự
        /// 2 2.1 ...
        /// </summary>

        public string No { get; set; }
        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// Số tiền
        /// </summary>
        public long Cost { get; set; }
        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public string Supplier { get; set; }
        /// <summary>
        /// Hình thức chi
        /// </summary>
        public string PayForm { get; set; }
    }
}
