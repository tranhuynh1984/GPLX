using GPLX.Core.Extensions;
using System;

namespace GPLX.Core.DTO.Request.TBL_CTVGROUPSUB
{
    public class TBL_CTVGROUPSUBCreateRequest
    {
        public string Record { get; set; }
        public int Status { get; set; }
        public int SubId { get; set; }
        public int CTVGroupID { get; set; }
        public string SubName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Note { get; set; }
        public int IsUse { get; set; }
        public string WhyNotUse { get; set; }
        public DateTime DateI { get; set; }
        public string UserI { get; set; }
        public float DisCount { get; set; }

        public string CreatorName { get; set; }
        public int Creator { get; set; }
        public float CustomerPrice { get; set; }
    }
}
