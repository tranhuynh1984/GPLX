using System;

namespace GPLX.Database.Models
{
    public class ActuallySpent
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Tuần báo cáo
        /// </summary>
        public int ReportForWeek { get; set; }
        /// <summary>
        /// Tên tuần báo cáo
        /// </summary>
        public string ReportForWeekName { get; set; }
        /// <summary>
        /// ID của đơn vị
        /// </summary>
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int Creator { get; set; }
        public string CreatorName { get; set; }

        public int Status { get; set; }
        public string StatusName { get; set; }

        public long TotalEstimateCost { get; set; }
        public long TotalActuallySpent { get; set; }
        public long TotalAmountLeft { get; set; }
        public long TotalActualSpentAtTime { get; set; }

        public DateTime CreatedDate  { get; set; }
        public DateTime UpdatedDate  { get; set; }
        public bool IsSub { get; set; }

    }
}
