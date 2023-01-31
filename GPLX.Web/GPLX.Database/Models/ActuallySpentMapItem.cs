using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Database.Models
{
    /// <summary>
    /// Bảng map giữa báo cáo thực chi và
    /// chi tiết các bản ghi
    /// ActuallySpentItem
    /// </summary>
    public class ActuallySpentMapItem
    {
        public Guid Id { get; set; }
        public Guid ActuallySpentItemId { get; set; }
        public Guid ActuallySpentId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Creator { get; set; }
        public string CreatorName { get; set; }

    }
}
