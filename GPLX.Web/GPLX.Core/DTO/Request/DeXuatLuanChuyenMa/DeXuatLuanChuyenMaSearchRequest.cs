using System;

namespace GPLX.Core.DTO.Request.DeXuatLuanChuyenMa
{
    public class DeXuatLuanChuyenMaSearchRequest
    {
        public int Status { get; set; }
        public int Draw { get; set; }
        public string DeXuatCode { get; set; }
        public string MaCTV { get; set; }
        public DateTime ThoiGianKhoa { get; set; }
        public string LyDoKhoa { get; set; }
        public string Note { get; set; }
        public int ProcessId { get; set; }
        public int ProcessStepId { get; set; }
    }
}
