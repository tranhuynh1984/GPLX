

using System.Collections.Generic;

namespace GPLX.Core.DTO.Response.CostEstimateItem
{
    public class CostEstimateCreateResponse
    {
        public int Code { get; set; }
        public string Message{ get; set; }
        //Mã ID mới được tạo hoặc mã ID đang chỉnh sủa
        public string Record { get; set; }

        // Danh sách các mã dự trù không tồn tại
        // Hoặc các dự trù có thể chưa được duyệt vv...
        public List<string> RecordsNotMatching { get; set; }
    }
}
