namespace GPLX.Database.Models
{
    /// <summary>
    /// Các danh mục cho danh mục kế hoạch đầu tư
    /// </summary>
    public class InvestmentPlanContents
    {
        public int InvestmentPlanContentId { get; set; }
        public string InvestmentPlanContentName { get; set; }
        /// <summary>
        /// Áp dụng cho đối tượng nào
        /// </summary>
        public string ForSubject { get; set; }

        public int Order { get; set; }

        public string RulesCellOnRow { get; set; }
        public string SkipCellAts { get; set; }
        public string Style { get; set; }
        public string GroupFor { get; set; }
    }
}
