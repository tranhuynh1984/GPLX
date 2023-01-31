using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.DTO.Response.DMCP;
using GPLX.Core.DTO.Response.TBL_CTVGROUPSUB1_DETAIL;
using GPLX.Core.DTO.Response.TBL_CTVGROUPSUB2_DETAIL;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.TBL_CTVGROUPSUB
{
    public class TBL_CTVGROUPSUBSearchResponseData : UpdateTimeResponseData
    {
        public int Index;
        public int SubId { get; set; }
        public int CTVGroupID { get; set; }
        public string CTVGroupName { get; set; }
        public string SubName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Note { get; set; }
        public int IsUse { get; set; }
        public string IsUseName { get; set; }
        public string WhyNotUse { get; set; }
        public DateTime DateI { get; set; }
        public string UserI { get; set; }
        public float DisCount { get; set; }
        public float CustomerPrice { get; set; }
    }

    public class TBL_CTVGROUPSUBSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<TBL_CTVGROUPSUBSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }


    public class HDCTVDetailSearchResponse
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int SubId { get; set; }
        public IList<TBL_CTVGROUPSUB1_DETAILSearchResponseData> CTVGROUPSUB1_DETAIL { get; set; }
        public IList<TBL_CTVGROUPSUB2_DETAILSearchResponseData> CTVGROUPSUB2_DETAIL { get; set; }
        public TBL_CTVGROUPSUBSearchResponseData CTVGROUPSUB { get; set; }
        public DMCPSearchResponse ListDMCP { get; set; }

    }
}
