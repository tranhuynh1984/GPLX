using System.Collections.Generic;

namespace GPLX.Core.DTO.Request
{
    public class DataSeenRequest
    {
        /// <summary>
        /// Loại trạng thái
        /// Trạng thái của yêu cầu | Trạng thái của dự trù
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Đối tượng
        /// SUB | Đơn vị thành viên
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Dùng cho loại nào
        /// Năm - tuần
        /// </summary>
        public string CostEstimateType { get; set; }
        /// <summary>
        /// Phòng ban hoặc chức vụ của đối tượng
        /// </summary>
        public IList<string> GroupCodes { get; set; }
    }
}
