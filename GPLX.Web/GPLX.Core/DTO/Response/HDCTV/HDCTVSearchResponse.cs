using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.DTO.Response.TBL_CTVGROUPSUB1_DETAIL;
using GPLX.Core.DTO.Response.TBL_CTVGROUPSUB2_DETAIL;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.HDCTV
{
    public class HDCTVSearchResponseData : UpdateTimeResponseData
    {
        public int GroupId { get; set; }

        public TBL_CTVGROUPSUB1_DETAILSearchResponse TBL_CTVGROUPSUB1_DETAIL { get; set; }

        public TBL_CTVGROUPSUB2_DETAILSearchResponse TBL_CTVGROUPSUB2_DETAIL { get; set; }
    }

    public class HDCTVSearchResponse
    {
        
        public int Code { get; set; }
        public string Message { get; set; }
        public HDCTVSearchResponseData Data { get; set; }
        
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }
}
