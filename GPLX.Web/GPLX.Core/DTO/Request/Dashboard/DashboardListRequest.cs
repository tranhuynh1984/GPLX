using System.Collections.Generic;

namespace GPLX.Core.DTO.Request.Dashboard
{
    public class DashboardListRequest
    {
        /// <summary>
        /// Năm tài chính
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// Đơn vị
        /// </summary>
        public List<int> UnitId { get; set; }
        /// <summary>
        /// Trạng thái
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Loại kế hoạch
        /// </summary>
        public List<string> Type { get; set; }

        public string HostFileView { get; set; }
        public int Draw { get; set; }
    }
}
