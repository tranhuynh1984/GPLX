using GPLX.Core.Extensions;

namespace GPLX.Core.DTO.Request.ProfileCK
{
    public class ProfileCKCreateRequest
    {
        public string Record { get; set; }
        public int Status { get; set; }
        public string ProfileCKMa { get; set; }
        public string ProfileCKTen { get; set; }
        public string ChuyenKhoaMa { get; set; }
        public int IsActive { get; set; }
        public string Note { get; set; }
        public string CreatorName { get; set; }
        public int Creator { get; set; }
    }
}
