namespace GPLX.Core.DTO.Request.ProfileCKCP
{
    public class ProfileCKCPSearchRequest
    {
        public int Status { get; set; }

        public int Draw { get; set; }

        public string ProfileCKMa { get; set; }
        public string CP { get; set; }
        public string Keywords { get; set; }
        public string RequestPage { get; set; }
    }
}
