using System.Collections.Generic;

namespace GPLX.Core.DTO.Response.CashFollow
{
    public class CashFollowExcelResponse
    {
        public IList<CashFollowItemExcel> CashFollowItemExcels { get; set; }
        public IList<CashFollowAggregateExcel> CashFollowAggregateExcels { get; set; }
        public Database.Models.CashFollow CashFollow { get; set; }

    }

    public class CashFollowItemExcel
    {
        public int CashFollowGroupId { get; set; }
        public string CashFollowGroupName { get; set; }

        public double M1 { get; set; }
        public double M2 { get; set; }
        public double M3 { get; set; }
        public double M4 { get; set; }
        public double M5 { get; set; }
        public double M6 { get; set; }
        public double M7 { get; set; }
        public double M8 { get; set; }
        public double M9 { get; set; }
        public double M10 { get; set; }
        public double M11 { get; set; }
        public double M12 { get; set; }
        public double Total { get; set; }
        public string No { get; set; }
        public string Style { get; set; }
    }

    public class CashFollowAggregateExcel
    {
        public int CashFollowGroupId { get; set; }
        public string CashFollowGroupName { get; set; }
        public string No { get; set; }
        public double Q1 { get; set; }
        public double Q2 { get; set; }
        public double Q3 { get; set; }
        public double Q4 { get; set; }
        public double Total { get; set; }
    }
}
