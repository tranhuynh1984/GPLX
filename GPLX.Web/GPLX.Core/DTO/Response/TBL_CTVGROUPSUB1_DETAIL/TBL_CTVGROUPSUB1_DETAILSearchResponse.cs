using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.TBL_CTVGROUPSUB1_DETAIL
{
    public class TBL_CTVGROUPSUB1_DETAILSearchResponseData : UpdateTimeResponseData
    {
        public int Index { get; set; }
        public int SubId { get; set; }
        public string MaCP { get; set; }
        public string TenCP { get; set; }
        public float BP1 { get; set; }
        public float BP2 { get; set; }
        public float BP3 { get; set; }
        public float BP4 { get; set; }
        public float BP5 { get; set; }
        public float BP6 { get; set; }
        public float BP7 { get; set; }
        public float BP8 { get; set; }
        public float BP9 { get; set; }
        public float BP10 { get; set; }
        public float BP11 { get; set; }
        public int IsActive { get; set; }
        public string IsActiveName { get; set; }
    }

    public class TBL_CTVGROUPSUB1_DETAILSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<TBL_CTVGROUPSUB1_DETAILSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }
}
