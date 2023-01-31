using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GPLX.Database.Models
{
    /// <summary>
    /// Số lượng khách hàng - kế hoạch doanh thu khách hàng
    /// </summary>
    public class RevenuePlanCustomerDetails
    {
        public Guid Id { get; set; }
        public Guid RevenuePlanId { get; set; }
        [NotMapped]
        public string No { get; set; }

        public int RevenuePlanContentId { get; set; }
        public string RevenuePlanContentName { get; set; }

        /// <summary>
        /// Phân loại
        /// Chỉ áp dụng với đơn vị y tế
        /// Chia theo đơn vị
        /// Chia theo địa bàn
        /// </summary>
        /// 
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
        /// <summary>
        /// Tỉ lệ /doanh thu
        /// </summary>
        public float ProPortion { get; set; }
    }
}
