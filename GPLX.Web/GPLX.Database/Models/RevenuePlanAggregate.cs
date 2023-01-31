using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GPLX.Database.Models
{
    /// <summary>
    /// Tổng hợp doanh thu khách hàng
    /// </summary>
    public class RevenuePlanAggregate
    {
        public Guid Id { get; set; }
        public Guid RevenuePlanId { get; set; }
        /// <summary>
        /// 
        /// Với đơn vị y tế
        /// Mục: Doanh thu và Khách hàng theo đơn vị sẽ được phân theo 1 số loại
        /// </summary>
        public int RevenuePlanContentId { get; set; }
        public string RevenuePlanContentContent { get; set; }
        /// <summary>
        /// Tỉ trọng khách hàng
        /// Chỉ dành cho ĐV y tế
        /// </summary>
        public double ProportionCustomer { get; set; }
        /// <summary>
        /// Doanh thu dự kiến
        /// </summary>
        public double ExpectRevenue { get; set; }
        /// <summary>
        /// Tỉ trọng doanh thu
        /// </summary>
        public double ProportionRevenue { get; set; }
        /// <summary>
        /// Số lượng khách hàng
        /// </summary>
        public double NumberCustomers { get; set; }
        [NotMapped]
        public string No { get; set; }
    }
}
