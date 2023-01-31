using System;

namespace GPLX.Database.Models
{
    public class CostEstimateLogs
    {
        public Guid Id { get; set; }
        public Guid CostEstimateId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int FromStatus { get; set; }
        public int ToStatus { get; set; }
        public string ToStatusName { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }
        //Lý do từ chối
        public string Reason { get; set; }

        //Chức vụ
        public int PositionId { get; set; }
        public string PositionName { get; set; }

    }
}
