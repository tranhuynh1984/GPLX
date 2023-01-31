using System.Collections.Generic;

namespace GPLX.Core.DTO.Response.Actually
{
    public class SearchActuallySpentResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }

        public List<SearchActuallySpentData> Data { get; set; }
    }

    public class SearchActuallySpentData
    {
        public string Record { get; set; }
        public int ReportForWeek { get; set; }
        public string ReportForWeekName { get; set; }
        public float TotalEstimateCost { get; set; }
        public float TotalActuallySpent { get; set; }
        public float TotalAmountLeft { get; set; }
        public float TotalActualSpentAtTime { get; set; }
        public bool Editable { get; set; }
        public bool Viewable { get; set; }

        public bool Approvalable { get; set; }
        public bool Declineable { get; set; }
        public string Status { get; set; }
        public string CreatedDate { get; set; }
    }
}
