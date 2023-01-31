using GPLX.Core.Extensions;

namespace GPLX.Core.DTO.Request.LoaiDeXuat
{
    public class LoaiDeXuatCreateRequest
    {
        public string Record { get; set; }
        public int Status { get; set; }
        public string LoaiDeXuatCode { get; set; }
        public string LoaiDeXuatName { get; set; }
        public int IsActive { get; set; }
        public int Stt { get; set; }
        public string CreatorName { get; set; }
        public int Creator { get; set; }
    }
}
