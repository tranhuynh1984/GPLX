using GPLX.Core.Extensions;
using System;

namespace GPLX.Core.DTO.Request.DeXuatLuanChuyenMa
{
    public class DeXuatLuanChuyenMaCreateRequest
    {
        public string Record { get; set; }
        public int Status { get; set; }
        public int Stt { get; set; }
        public int Draw { get; set; }
        public string DeXuatCode { get; set; }
        public string MaCTV { get; set; }
        public DateTime ThoiGianKhoa { get; set; }
        public string LyDoKhoa { get; set; }
        public string Note { get; set; }
        public string RequestPage { get; set; }
        public string CreatorName { get; set; }
        public int Creator { get; set; }
    }
}
