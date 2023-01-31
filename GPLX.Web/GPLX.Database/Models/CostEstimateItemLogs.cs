using System;

namespace GPLX.Database.Models
{
    public class CostEstimateItemLogs
    {
        public Guid Id { get; set; }
        public Guid CostEstimateItemId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int FromStatus { get; set; }
        public int FromStatusName { get; set; }
        public int ToStatus { get; set; }
        public string ToStatusName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }

        //Lý do từ chối
        public string Reason { get; set; }

        //Chức vụ
        public int PositionId { get; set; }
        public string PositionName { get; set; }

        // Request: là log của yêu cầu - hiển thị trong danh sách & chi tiết yêu cầu
        // CostEstimate: là log của dự trù trong phiếu
        // 2 trạng thái này là khác nhau
        // vì trong danh sách dự trù của phiếu, Kế toán trưởng vẫn phải tích duyệt dự trù
        
        public string LogType { get; set; }
    }
}
