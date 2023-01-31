using System;
using System.Collections.Generic;

namespace GPLX.Core.DTO.Response.CostEstimateItem
{
    public class CostEstimateItemSearchResponseData
    {
        public string Id { get; set; }
        public string RequestContent { get; set; }
        public string RequestCode { get; set; }
        public string CostEstimateItemTypeName { get; set; }
        public int CostEstimatePaymentType { get; set; }
       
        public long Cost { get; set; }
        public string UnitName { get; set; }
        public string DepartmentName { get; set; }
        public string CreatorName { get; set; }
        public string PayWeekName { get; set; }
        public string SupplierName { get; set; }
        public string BillCode { get; set; }
        public DateTime BillDate { get; set; }
        public long BillCost { get; set; }

        public string Explanation { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string CreatedDate { get; set; }

        public string RequestImage { get; set; }
        public string RequesterName { get; set; }
        public string PayForm { get; set; }
        public string CostEstimateGroupName { get; set; }

        public bool Editable { get; set; }
        public bool Viewable { get; set; }

        public bool Approvalable { get; set; }
        public bool Declineable { get; set; }
        public bool Deleteable { get; set; }

        public int Type { get; set; }

        public int IsDeleted { get; set; }

        public string UpdaterName { get; set; }
        public int Updater { get; set; }

        public string UpdatedDate { get; set; }
    }

    public class CostEstimateItemSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IEnumerable<CostEstimateItemSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
    }
}
