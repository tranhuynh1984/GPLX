using System.Collections.Generic;

namespace GPLX.Core.DTO.Response.CostEstimate
{
    public class CompareCFAndActuallyResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<CompareCFAndActuallyResponseData> Data { get; set; }
    }

    public class CompareCFAndActuallyResponseData
    {
        public int No { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// Ngân sách lũy kế
        /// </summary>
        public double CumulativeBudget  { get; set; }
        /// <summary>
        /// Thực chi lũy kế
        /// </summary>
        public double CumulativeActuallySpent { get; set; }
        /// <summary>
        /// Vượt ngân sách
        /// </summary>
        public double OverBudget { get; set; }
        /// <summary>
        /// Tỉ lệ ngân sách
        /// </summary>
        public double BudgetRate { get; set; }
        /// <summary>
        /// Tỉ lệ thực tế
        /// </summary>
        public double ActuallyRate { get; set; }
        /// <summary>
        /// Phân loại
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// Tỉ lệ vượt chi
        /// </summary>
        public double OverBudgetRate { get; set; }

        public bool IsBold { get; set; }

    }

}
