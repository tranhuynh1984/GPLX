namespace GPLX.Core.DTO.Request.DMBS_ChuyenKhoa
{
    public class DMBS_ChuyenKhoaSearchRequest
    {
        public int Status { get; set; }

        public int Draw { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }

        public string Keywords { get; set; }
        public string RequestPage { get; set; }
    }
}
