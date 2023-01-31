using GPLX.Core.Extensions;

namespace GPLX.Core.DTO.Request.DMBS_ChuyenKhoa
{
    public class DMBS_ChuyenKhoaCreateRequest
    {
        public string Record { get; set; }
        public int Status { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public int IsActive { get; set; }
        public int Stt { get; set; }
        public string CreatorName { get; set; }
        public int Creator { get; set; }
    }
}
