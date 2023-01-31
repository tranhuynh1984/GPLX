using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.DTO.Response.InvestmentPlan
{
    public class SearchInvestmentPlanResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }

        public List<SearchInvestmentPlanResponseData> Data { get; set; }
    }
    public class SearchInvestmentPlanResponseData
    {
        public string Record { get; set; }
        public string CreatorName { get; set; }
        public string PathPdf { get; set; }
        public double TotalExpectCostInvestment { get; set; }

        /// <summary>
        /// Vốn tự có
        /// </summary>
        public double TotalExpenditureCapital { get; set; }
        /// <summary>
        /// Đầu tư của MG
        /// </summary>
        public double TotalCapitalMedGroup { get; set; }
        /// <summary>
        /// Vốn vay lưu động
        /// </summary>
        public double TotalSpendingLoan { get; set; }

        public bool Editable { get; set; }
        public bool Viewable { get; set; }

        public bool Approvalable { get; set; }
        public bool Declineable { get; set; }
        public bool Deleteable { get; set; }
        public string StatusName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UnitName { get; set; }
        public int  Year { get; set; }
    }
}
