using System;

namespace GPLX.Core.DTO.Request.DeXuat
{
    public class DeXuatSearchRequest
    {
        public int Status { get; set; }
        public int Draw { get; set; }
        public string DeXuatCode { get; set; }
        public string DeXuatName { get; set; }
        public string MaBacsi { get; set; }
        public string LoaiDeXuatCode { get; set; }
        public int ProcessId { get; set; }
        public int ProcessStepId { get; set; }
        public string Note { get; set; }
        public int IsDone { get; set; }
        public DateTime? ThoiGianKhoa { get; set; }
        public string LyDoKhoa { get; set; }
        public string MaDonViCu { get; set; }
        public string MaDonViMoi { get; set; }
        public string Keywords { get; set; }
        public string RequestPage { get; set; }
        public string MaDonViDeXuat { get; set; }

        public int IDRole { get; set; }
        public DateTime? ND { get; set; }
        public DateTime? NS { get; set; }
        public string NguoiTao { get; set; }
        public string TenCongTy { get; set; }
    }
}
