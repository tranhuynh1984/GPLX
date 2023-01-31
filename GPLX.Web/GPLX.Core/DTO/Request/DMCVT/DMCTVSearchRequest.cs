namespace GPLX.Core.DTO.Request.DMCVT
{
    public class DMCTVSearchRequest
    {
        public int Status { get; set; }
        public int Draw { get; set; }
        public string MaBS { get; set; }
        public string Keywords { get; set; }
        public string RequestPage { get; set; }
    }
}