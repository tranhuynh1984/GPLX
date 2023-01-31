namespace GPLX.Core.DTO.Request.TBL_CTVGROUPSUB2_DETAIL
{
    public class TBL_CTVGROUPSUB2_DETAILSearchRequest
    {
        public int Status { get; set; }

        public int Draw { get; set; }
        public int IsActive { get; set; }
        public int SubId { get; set; }
        public string MaCP { get; set; }
        public float FixedPrice { get; set; }
        public string Keywords { get; set; }
        public string RequestPage { get; set; }
    }
}
