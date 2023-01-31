namespace GPLX.Core.DTO.Request.LoaiDeXuat
{
    public class LoaiDeXuatSearchRequest
    {
        public int Status { get; set; }

        public int Draw { get; set; }

        public string LoaiDeXuatCode { get; set; }
        public string LoaiDeXuatName { get; set; }
        public string Keywords { get; set; }
        public string RequestPage { get; set; }
    }
}
