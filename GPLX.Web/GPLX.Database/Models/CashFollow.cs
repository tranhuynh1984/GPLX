using System;

namespace GPLX.Database.Models
{
    /// <summary>
    /// Bảng lưu kế hoạch dòng tiền
    /// </summary>
    public class CashFollow
    {
        public Guid Id { get; set; }

        public int Year { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int Creator { get; set; }
        public string CreatorName { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }

        public string PathExcel { get; set; }
        public string PathPdf { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsSub { get; set; }
        /// <summary>
        /// Loại
        /// In;Out;SUB
        /// </summary>
        public string CashFollowType { get; set; }
        /// <summary>
        /// Tổng doanh thu
        /// </summary>
        public double TotalRevenue { get; set; }
        /// <summary>
        /// Tổng chi phí
        /// </summary>
        public double TotalSpending { get; set; }
        /// <summary>
        /// Tổng luân chuyển
        /// </summary>
        public double TotalCirculation { get; set; }

        public string Migrate { get; set; }
    }
}
