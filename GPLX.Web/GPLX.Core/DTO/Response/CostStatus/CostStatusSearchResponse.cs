using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.DTO.Response.CostStatus
{
    public class CostStatusSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IEnumerable<CostStatusSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
    }

    public class CostStatusSearchResponseData
    {
        public string Record { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string StatusForCostEstimateType { get; set; }

        public string StatusForSubject { get; set; }

        public int Order { get; set; }

        public string StatusName { get; set; }

        public bool IsApprove { get; set; }
        public int Value { get; set; }
    }
}
