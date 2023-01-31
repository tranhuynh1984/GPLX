using System;

namespace GPLX.Core.DTO.Response.CostEstimateItem
{
    public class CostEstimateItemApprovalResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public CostEstimateItemSearchResponseData Data { get; set; }
    }

    public class CostEstimateItemFromExcel
    {
        public string Record { get; set; }
        // Mã tự tạo khi upload excel
        public string RequestCode { get; set; }
        public string RequestContent { get; set; }
        public string RequestContentNonUnicode { get; set; }
        public string CostEstimateItemTypeName { get; set; }
        public long Cost { get; set; }
        public string UnitName { get; set; }
        public string DepartmentName { get; set; }
        // Người tạo dự trù (không phải là người tạo yêu cầu)
        public string CreatorName { get; set; }
        public int CreatorId { get; set; }
        // Người yêu cầu
        public int RequesterId { get; set; }
        public string RequesterName { get; set; }
        public int PayWeek { get; set; }
        public string PayWeekName { get; set; }
        public string SupplierName { get; set; }
        public string BillCode { get; set; }
        public DateTime BillDate { get; set; }
        public long BillCost { get; set; }
        public string AccountImage { get; set; }
        public string Explanation { get; set; }

        public string CostEstimateGroupName { get; set; }
        // Hình thức chi
        public string PayForm { get; set; }

        // Loại dự trù
        // Năm - tuần

        public int CostEstimateType { get; set; }

        public bool Approved { get; set; }
    }
}
