namespace GPLX.Core.DTO.Request.ProfileCK
{
    public class ProfileCKSearchRequest
    {
        public int Status { get; set; }

        public int Draw { get; set; }

        public string ProfileCKMa { get; set; }
        public string ProfileCKTen { get; set; }
        public string ChuyenKhoaMa { get; set; }
        public string Keywords { get; set; }
        public string RequestPage { get; set; }
    }
}
