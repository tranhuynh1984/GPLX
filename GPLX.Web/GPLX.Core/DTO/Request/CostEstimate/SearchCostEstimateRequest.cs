using System.Collections.Generic;
using GPLX.Core.DTO.Response.CostStatus;
using GPLX.Database.Models;

namespace GPLX.Core.DTO.Request.CostEstimate
{
    public class SearchCostEstimateRequest
    {
        public int Status { get; set; }

        public int ReportForWeek { get; set; }
        public int Draw { get; set; }

        // Mã ID của đơn vị
        // Đơn vị nào chỉ nhìn thấy thực chi của đơn vị đó

        /// <summary>
        /// Đối với cấp giám đốc đơn vị trở xuống
        /// Hoặc tìm kiếm theo đơn vị
        /// </summary>
        public int UserUnit { get; set; }   

        // Key của page
        // Dùng để mã hóa ID
        public string PageRequest { get; set; }

       /// <summary>
       /// Loại dự trù
       /// </summary>
        
        public string RequestType { get; set; }
       /// <summary>
       /// Dự trù năm / tuần
       /// </summary>
        public int CostEstimateTypeId { get; set; }

        /// <summary>
        /// Trạng thái mà user hiện tại được xem
        /// </summary>
        public IEnumerable<StatusesGranted> StatusAllowsSeen { get; set; }

        public bool IsSub { get; set; }

        public bool PermissionEdit { get; set; }
        public bool PermissionApprove { get; set; }
        public bool PermissionDelete { get; set; }

        public string HostFileView { get; set; }

        public IList<UserUnitsManages> UnitsManages { get; set; }
    }
}
