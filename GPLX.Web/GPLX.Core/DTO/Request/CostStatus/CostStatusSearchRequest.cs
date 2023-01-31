using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.DTO.Request.CostStatus
{
    public class CostStatusSearchRequest
    {
        public int Status { get; set; }
        public int Draw { get; set; }
        public string Keywords { get; set; }
        public string Type { get; set; }
        public string StatusForCostEstimateType { get; set; }
        public string StatusForSubject { get; set; }
        public string RequestPage { get; set; }
    }
}
