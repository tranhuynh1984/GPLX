using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Database.Models
{
    /// <summary>
    /// Bảng map giữa thực chi và yêu cầu
    /// Chỉ insert vào bảng này khi kế toán trưởng duyệt BC thực chi
    /// </summary>
    public class CostEstimateItemMapActuallySpentItem
    {
        public Guid Id { get; set; }
        public Guid ActuallySpentItemId { get; set; }
        public Guid ActuallySpentId { get; set; }
        public string RequestCode { get; set; }
        public Guid CostEstimateItemId { get; set; }

        //Trạng thái của thực chi
        // Todo: đã chi hết thì = 1
        // Đã chi nhưng chưa chi hết: = 2
        // 
        public int Status { get; set; }
        public string StatusName { get; set; }
    }
}
