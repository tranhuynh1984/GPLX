namespace GPLX.Core.DTO.Request.DMHuyen
{
    public class DMHuyenSearchRequest
    {
        public int Status { get; set; }

        public int Draw { get; set; }

        public string MaHuyen { get; set; }
        public string TenHuyen { get; set; }
        public string MaTinh { get; set; }
        public string RequestPage { get; set; }
    }
}
