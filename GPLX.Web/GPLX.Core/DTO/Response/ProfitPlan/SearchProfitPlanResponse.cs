using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.DTO.Response.ProfitPlan
{
    public class SearchProfitPlanResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }

        public List<SearchProfitPlanResponseData> Data { get; set; }
    }
    public class SearchProfitPlanResponseData
    {
        public string Record { get; set; }
        public string CreatorName { get; set; }
        public string PathPdf { get; set; }
        public bool Editable { get; set; }
        public bool Viewable { get; set; }

        public bool Approvalable { get; set; }
        public bool Declineable { get; set; }
        public bool Deleteable { get; set; }


        public double TotalRevenue { get; set; }
        /// <summary>
        /// Tổng chi phí
        /// </summary>
        public double TotalExpense { get; set; }
        /// <summary>
        /// Lợi nhuận sau thuế
        /// </summary>
        public double TotalProfitTax { get; set; }
        /// <summary>
        /// TL/DT của chi phí
        /// </summary>
        public double ProportionExpense { get; set; }
        /// <summary>
        /// TL/DT lợi nhuận
        /// </summary>
        public double ProportionProfitTax { get; set; }

        public string StatusName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UnitName { get; set; }
        public int Year { get; set; }
    }
}
