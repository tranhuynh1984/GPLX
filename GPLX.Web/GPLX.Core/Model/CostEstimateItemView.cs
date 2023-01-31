using System;

namespace GPLX.Core.Model
{
    public class CostEstimateItemView
    {
        public string RequestCode { get; set; }
        public string Record { get; set; }
        public string RequestContent { get; set; }
        public string RequestContentNonUnicode { get; set; }
        public int CostEstimateItemTypeId { get; set; }
        public string CostEstimateItemTypeName { get; set; }
        public long Cost { get; set; }
        public string UnitName { get; set; }
        public string DepartmentName { get; set; }
        public string CreatorName { get; set; }
        public string RequesterId { get; set; }
        public string RequesterName { get; set; }


        public string PayForm { get; set; }
        public string CostEstimateGroupName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int PayWeek { get; set; }
        public string PayWeekName { get; set; }
        public string SupplierName { get; set; }
        public string BillCode { get; set; }
        public DateTime BillDate { get; set; }
        public long BillCost { get; set; }
        public string RequestImage { get; set; }
        public string AccountImage { get; set; }
        public string Explanation { get; set; }
        public int Status { get; set; }

        // Đánh dấu bản ghi bị khóa
        public int IsLock { get; set; }
    }
}
