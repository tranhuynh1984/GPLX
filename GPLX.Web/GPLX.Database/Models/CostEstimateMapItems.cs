using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Database.Models
{
    // Bảng mapping giữa yêu cầu & dự trù
    // 1 yêu cầu có thể đẩy sang dự trù của tuần kế tiếp
    public class CostEstimateMapItems
    {
        public Guid Id { get; set; }
        public Guid CostEstimateId { get; set; }
        public Guid CostEstimateItemId { get; set; }
        // Mã dự trù
        public string RequestCode { get; set; }
        public int Status { get; set; }

        // Người tạo phiếu
        // Không phải người tạo yêu cầu
        // Xem mẫu của kế toán viên
        public int CreatorId { get; set; }
        public string CreatorName { get; set; }

        // Bản ghi bị xóa
        public int IsDeleted { get; set; }

        public string UpdaterName { get; set; }
        public int Updater { get; set; }

        public DateTime UpdatedDate { get; set; }

    }
}
