namespace GPLX.Core.DTO.Request.TBL_CTVGROUP
{
    public class TBL_CTVGROUPSearchRequest
    {
        public int Status { get; set; }

        public int Draw { get; set; }

        public string CTVGroupID { get; set; }
        public string CTVGroupName { get; set; }
        public string Keywords { get; set; }
        public string RequestPage { get; set; }
    }
}
