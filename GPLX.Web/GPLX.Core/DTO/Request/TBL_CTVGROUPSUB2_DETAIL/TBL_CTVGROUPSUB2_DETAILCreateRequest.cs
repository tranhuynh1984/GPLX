using GPLX.Core.Extensions;

namespace GPLX.Core.DTO.Request.TBL_CTVGROUPSUB2_DETAIL
{
    public class TBL_CTVGROUPSUB2_DETAILCreateRequest
    {
        public string Record { get; set; }
        public int Status { get; set; }
        public int SubId { get; set; }
        public string MaCP { get; set; }
        public string TenCP { get; set; }
        public float FixedPrice { get; set; }
        public int IsActive { get; set; }
        public string CreatorName { get; set; }
        public int Creator { get; set; }
    }
}
