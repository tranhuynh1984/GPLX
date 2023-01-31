using GPLX.Core.Extensions;

namespace GPLX.Core.DTO.Request.DM
{
    public class DMCreateRequest
    {
        public string Record { get; set; }
        public int Status { get; set; }
        public int Stt { get; set; }

        public int Draw { get; set; }

        public string MaDM { get; set; }
        public string TenDM { get; set; }
        public int IsActive { get; set; }
        public string RequestPage { get; set; }
        public int? IdTree { get; set; } = 59; //Danh mục tỉnh	59

        public string CreatorName { get; set; }
        public int Creator { get; set; }
    }
}
