using GPLX.Core.Extensions;

namespace GPLX.Core.DTO.Request.TBL_CTVGROUPSUB1_DETAIL
{
    public class TBL_CTVGROUPSUB1_DETAILCreateRequest
    {
        public string Record { get; set; }
        public int Status { get; set; }
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

        public string CreatorName { get; set; }
        public int Creator { get; set; }
        public int IsActive { get; set; }
    }
}
