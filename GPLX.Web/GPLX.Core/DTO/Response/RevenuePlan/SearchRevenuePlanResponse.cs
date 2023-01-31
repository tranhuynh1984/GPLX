using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.DTO.Response.RevenuePlan
{
    public class SearchRevenuePlanResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }

        public List<SearchRevenuePlanResponseData> Data { get; set; }
    }

    public class SearchRevenuePlanResponseData
    {
        public string Record { get; set; }
        public string CreatorName { get; set; }
        public string PathPdf { get; set; }
        public bool Editable { get; set; }
        public bool Viewable { get; set; }

        public bool Approvalable { get; set; }
        public bool Declineable { get; set; }
        public bool Deleteable { get; set; }
        public string StatusName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UnitName { get; set; }
        public int Year { get; set; }

        public double TotalExpectRevenue { get; set; }
        public double TotalNumberCustomer { get; set; }
    }
}
