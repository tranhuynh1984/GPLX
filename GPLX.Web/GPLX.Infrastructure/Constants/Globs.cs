using System.Collections.Generic;

namespace GPLX.Infrastructure.Constants
{
    public static class Globs
    {
        public const int DefaultSearchAllStats = -100;
        public const string DefaultSearchAllStatsName = "Tất cả";
    }

    public static class CostElementItemConst
    {
        public const string PublicKey = "cost-element-item";
        //public const string RequestTypeUnit = "unit";
        public const int RequestTypeUnitValue = 0;
        public const int RequestTypeSubValue = 1;
        public const string RequestTypeSub = "sub";
    }

    public static class CostElementConst
    {
        public const string PublicKey = "cost-element";
    }

    public static class ActuallySpent
    {
        public const string PublicKey = "actully-spent";
    }

    public static class CashFollowConst
    {
        public const string PublicKey = "cash-follow";
    }

    public static class DepartmentConst
    {
        public const string PublicKey = "deparment";
    }

    public static class StatusesConst
    {
        public const string PublicKey = "statuses";
    }

    public static class UsersConst
    {
        public const string PublicKey = "users";
    }
    public static class GroupsConst
    {
        public const string PublicKey = "groups";
    }

    public static class SctDataConst
    {
        public static readonly string[] FileImportSheetNamesRequire = new[] {"SCT 111", "SCT 112"};
    }

    public static class InvestmentPlanConst
    {
        public const string PublicKey = "investment-plan";

    }
    public static class RevenuePlanConst
    {
        public const string PublicKey = "revenue-plan";

    }

    public static class ProfitPlanConst
    {
        public const string PublicKey = "profit-plan";

    }
    
    public static class HdctvDataConst
    {
        public static class SheetType1
        {
            public static readonly string name = "N1";
            public static readonly int MaCP = 0;
            public static readonly int TenCP = 1;
            public static readonly int BP1 = 2;
            public static readonly int BP2 = 3;
            public static readonly int BP3 = 4;
            public static readonly int BP4 = 5;
            public static readonly int BP5 = 6;
            public static readonly int BP6 = 7;
            public static readonly int BP7 = 8;
            public static readonly int BP8 = 9;
            public static readonly int BP9 = 10;
            public static readonly int BP10 = 11;
            public static readonly int BP11 = 12;
        }
        
        public static class SheetType2
        {
            public static readonly string name = "N2";
            public static readonly int MaCP = 0;
            public static readonly int TenCP = 1;
            public static readonly int FixedPrice = 2;
        }
    }

    public static class ExcelCellValueFix
    {
        /// <summary>
        /// cell số dư khả dụng đầu kỳ
        /// </summary>
        public const string BeginAvailableBalanceCell = "số dư khả dụng đầu kỳ (1)";

        /// <summary>
        /// cell tiền mặt
        /// </summary>
        public const string CashCell = "tiền mặt";
        /// <summary>
        /// cell số dư tài khoản thanh toán
        /// </summary>
        public const string AccountBalanceCell = "số dư tk thanh toán";
        /// <summary>
        /// vay lưu động – số dư khả dụng
        /// </summary>
        public const string LoanAvailableCell = "vay lưu động – số dư khả dụng";

        public const string ExpectRevenueCell = "số tiền thu dự kiến (2)";


        /// <summary>
        /// Chi hoạt động
        /// </summary>
        public const string OperatingCell = "các khoản chi hoạt động";
       

        /// <summary>
        /// Các khoản chi đầu tư
        /// </summary>
        public const string InvestmentCell = "các khoản chi đầu tư";

        /// <summary>
        /// Các khoản chi hoạt động tài chính
        /// </summary>
        public const string FinancialCell = "các khoản chi hoạt động tài chính";
        /// <summary>
        /// Số dư khả dụng cuối kỳ (1+2-3)
        /// </summary>
        public const string EndAvailableBalanceCell = "số dư khả dụng cuối kỳ (1+2-3)";

        /// <summary>
        /// Số dư khả dụng cuối kỳ (1+2)
        /// Dùng cho sub
        /// </summary>
        public const string EndAvailableBalanceSubCell = "số dư khả dụng cuối kỳ (1+2)";

        /// <summary>
        /// Định mức tồn quỹ (5)
        /// </summary>
        public const string EquityCostCell = "định mức tồn quỹ (5)";
        /// <summary>
        /// Dự kiến cắt tiền về dòng tiền tập trung (4-5)
        /// </summary>
        public const string PlanCutCostCell = "dự kiến cắt tiền về dòng tiền tập trung (4-5)";
        /// <summary>
        /// Đề xuất luân chuyển tiền về đơn vị thành viên
        /// </summary>
        public const string RotationProposalCell = "đề xuất luân chuyển tiền về đvtv";
        /// <summary>
        /// Số tiền chi phí dự kiến (3)
        /// </summary>
        public const string EstimatedCostCell = "số tiền chi phí dự kiến (3)";

        public const string TotalExpenditureCapitalCell = "tổng chi bằng vốn tự có";

        public const string TableNoSpent = "ii.báo cáo";

    }
}
