using System;
using System.Globalization;

namespace GPLX.Database.Models
{
    public class CostEstimateItem
    {
        // Mã yêu cầu
        // Dùng để mapping khi kế toán tạo dự trù

        public string RequestCode { get; set; }
        public Guid Id { get; set; }
        //Không sử dụng
        //public string EstimateId { get; set; }
        public string RequestContent { get; set; }
        public string RequestContentNonUnicode { get; set; }
        /// <summary>
        /// Nhóm chi phí
        /// Thanh toán cho NCC ...
        /// </summary>
        public int CostEstimateItemTypeId { get; set; }
        public string CostEstimateItemTypeName { get; set; }
        /// <summary>
        /// deprecated
        /// </summary>
        public int CostEstimatePaymentType { get; set; }
        public long Cost { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int CreatorId { get; set; }
        public string CreatorName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int PayWeek { get; set; }
        public string PayWeekName { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string BillCode { get; set; }
        public DateTime BillDate { get; set; }
        public long BillCost { get; set; }
        public string RequestImage { get; set; }
        public string AccountImage { get; set; }
        public string Explanation { get; set; }
        public int Status { get; set; }

        // Loại dự trù
        // Năm - tuần

        public int EstimateType { get; set; }
        // Đánh dấu bản ghi bị khóa
        // Khi được phê duyệt bởi trưởng phòng
        // 
        public int IsLock { get; set; }

        /// <summary>
        /// Nhóm dự trù
        /// </summary>
        public string CostEstimateGroupName { get; set; }
        // Hình thức chi
        public string PayForm { get; set; }

        public int  RequesterId { get; set; }
        public string RequesterName { get; set; }

        /// <summary>
        /// Loại yêu cầu Năm / Tuần
        /// </summary>
        public int Type { get; set; }
        public string TypeName { get; set; }
        /// <summary>
        /// của đơn vị sub hay đơn vị thành viên
        /// </summary>
        public bool IsSub { get; set; }
    }
}
