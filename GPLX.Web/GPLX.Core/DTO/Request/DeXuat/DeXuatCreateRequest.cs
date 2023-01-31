using GPLX.Core.Extensions;
using System;

namespace GPLX.Core.DTO.Request.DeXuat
{
    public class DeXuatCreateRequest
    {
        public string Record { get; set; }
        public int Status { get; set; }
        public int Stt { get; set; }
        public int Draw { get; set; }
        public string DeXuatCode { get; set; }
        public string DeXuatName { get; set; }
        public string MaBacsi { get; set; }
        public string TenBacsi { get; set; }
        public string LoaiDeXuatCode { get; set; }
        public int ProcessId { get; set; }
        public int ProcessStepId { get; set; }
        public string Note { get; set; }
        public string RequestPage { get; set; }
        public string CreatorName { get; set; }
        public int Creator { get; set; }
        public int IDRole { get; set; }
        public int IsDone { get; set; }
        public DateTime? ThoiGianKhoa { get; set; }
        public string LyDoKhoa { get; set; }
        public string MaDonViCu { get; set; }
        public string MaDonViMoi { get; set; }
        public string GhiChuStep { get; set; }
        public string MaDonViDeXuat { get; set; }

        public string CreateGhiChuByCode { get; set; }
        public string CreateGhiChuByName { get; set; }
        public DateTime? CreateGhiChuDate { get; set; }
        public string TenCongTy { get; set; }
    }
}
