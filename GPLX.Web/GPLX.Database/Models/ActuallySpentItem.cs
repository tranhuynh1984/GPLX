using System;

namespace GPLX.Database.Models
{
    /// <summary>
    /// Chi tiết record thực chi
    /// </summary>
    public class ActuallySpentItem
    {
        public Guid Id { get; set; }
        public string RequestCode { get; set; }
        /// <summary>
        /// Trường hợp không có mã dự trù sẽ hiển thị theo trường này
        /// </summary>
        public string RequestContent { get; set; }
        /// <summary>
        /// Số tiền đã chi
        /// </summary>
        public long ActualSpent { get; set; }
        /// <summary>
        /// Số còn phải chi
        /// </summary>
        public long AmountLeft { get; set; }

        /// <summary>
        /// Số tiền chi thực tế đến thời điểm hiện tại
        /// Công thức: ActualSpent + AmountLeft
        /// </summary>
        public long ActualSpentAtTime { get; set; }
        /// <summary>
        /// Thòi gian chi thực
        /// </summary>
        public int ActualPayWeek { get; set; }
        public string ActualPayWeekName { get; set; }
        /// <summary>
        /// Số chứng từ kế toán
        /// </summary>
        public string AccountantCode { get; set; }
        /// <summary>
        /// Giải trình
        /// </summary>
        public string Explanation { get; set; }
        /// <summary>
        /// Loại - gồm có chi cho dự trù và chi ngoài dự trù
        /// Fix 2 loại: In - Out
        /// </summary>
        public string ActualSpentType { get; set; }

        public string RequestPayWeekName { get; set; }
        public int RequestPayWeek { get; set; }

        /// <summary>
        /// Nhóm chi phí
        /// </summary>
        public int CostEstimateItemTypeId { get; set; }
        public string CostEstimateItemTypeName { get; set; }

       
        /// <summary>
        /// Nhóm dự trù
        /// </summary>
        public string CostEstimateGroupName { get; set; }
        /// <summary>
        /// Đối với thực chi trong dự trù sẽ có 
        /// </summary>
        public long Cost { get; set; }
    }
}
