using GPLX.Core.Extensions;

namespace GPLX.Core.DTO.Request.DeXuatChiTiet
{
    public class DeXuatChiTietCreateRequest
    {
        public string Record { get; set; }
        public int Status { get; set; }
        public int Stt { get; set; }
        public int Draw { get; set; }
        public string DeXuatCode { get; set; }
        public string FieldName { get; set; }
        public string ValueOld { get; set; }
        public string ValueNew { get; set; }
        public string Note { get; set; }
        public string RequestPage { get; set; }
        public string CreatorName { get; set; }
        public int Creator { get; set; }
    }
}
