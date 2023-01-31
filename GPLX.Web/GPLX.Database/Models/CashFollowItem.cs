using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Database.Models
{
    /// <summary>
    /// Bảng lưu chi tiết kế hoạch dòng tiền
    /// </summary>
    public class CashFollowItem
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Loại dữ liệu - tương đương với các chỉ mục lớn
        /// Tiền và tương đương tiền đầu kỳ
        /// Tiền thu được trong kỳ
        /// Tiền chi trong kỳ
        /// Lưu chuyển tiền thuần trong kỳ
        /// Tiền và tương đương tiền cuối kỳ
        /// </summary>
        public int CashFollowGroupId { get; set; }
        public string CashFollowGroupName { get; set; }
      
        public double M1 { get; set; }
        public double M2 { get; set; }
        public double M3 { get; set; }
        public double M4 { get; set; }
        public double M5 { get; set; }
        public double M6 { get; set; }
        public double M7 { get; set; }
        public double M8 { get; set; }
        public double M9 { get; set; }
        public double M10 { get; set; }
        public double M11 { get; set; }
        public double M12 { get; set; }
        public double Total { get; set; }

        public Guid CashFollowId { get; set; }
        public string Migrate { get; set; }
    }
}
