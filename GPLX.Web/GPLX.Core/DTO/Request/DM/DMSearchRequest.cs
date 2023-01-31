namespace GPLX.Core.DTO.Request.DM
{
    public class DMSearchRequest
    {
        public int Status { get; set; }

        public int Draw { get; set; }

        public string MaDM { get; set; }
        public string TenDM { get; set; }
        public string RequestPage { get; set; }
        public int? IdTree { get; set; } = 59; //Danh mục tỉnh	59
    }
}
