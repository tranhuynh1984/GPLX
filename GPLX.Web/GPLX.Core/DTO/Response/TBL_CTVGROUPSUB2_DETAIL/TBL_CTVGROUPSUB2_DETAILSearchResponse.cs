using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.TBL_CTVGROUPSUB2_DETAIL
{
    public class TBL_CTVGROUPSUB2_DETAILSearchResponseData : UpdateTimeResponseData
    {
        public int Index { get; set; }
        public int SubId { get; set; }
        public string MaCP { get; set; }
        public string TenCP { get; set; }
        public float FixedPrice { get; set; }
        public int IsActive { get; set; }
        public string IsActiveName { get; set; }
    }

    public class TBL_CTVGROUPSUB2_DETAILSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<TBL_CTVGROUPSUB2_DETAILSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }
}
