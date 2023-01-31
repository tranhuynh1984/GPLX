using System;

namespace GPLX.Database.Models
{
    /// <summary>
    /// Kế hoạch doanh thu
    /// </summary>
    public class RevenuePlan
    {
        public Guid Id { get; set; }
        public int Year { get; set; }
        public int UnitId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UnitName { get; set; }
        public int Creator { get; set; }
        public string CreatorName { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }


        /// <summary>
        /// loại: Y tế - Ngoài Y tế
        /// In - Out
        /// </summary>
        public string RevenuePlanType { get; set; }

        public string PathExcel { get; set; }
        public string PathPdf { get; set; }

        public bool IsSub { get; set; }
    }
}
