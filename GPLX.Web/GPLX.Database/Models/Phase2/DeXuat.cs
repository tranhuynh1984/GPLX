using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class DeXuat : UpdateTime
    {
        //Mã đề xuất
        [MaxLength(20)]
        [Required]
        public string DeXuatCode { get; set; }
        //Tên đề xuất
        [MaxLength(100)]
        public string DeXuatName { get; set; }
        [MaxLength(10)]
        public string MaBacsi { get; set; }
        [MaxLength(100)]
        public string TenBacsi { get; set; }

        [MaxLength(20)]
        public string LoaiDeXuatCode { get; set; }
        //Tiến Trình Id
        public int ProcessId { get; set; }
        //Process Id
        public int ProcessStepId { get; set; }
        //Ghi chú
        public string Note { get; set; }

        //Default = 0, Done: 1, Reject: -1
        public int IsDone { get; set; }
        //Thời gian khóa CTV. Chỉ phục vụ cho loại đề xuất khóa
        public DateTime? ThoiGianKhoa { get; set; }
        //Lý do khóa CTV. Chỉ phục vụ cho loại đề xuất khóa
        public string LyDoKhoa { get; set; }

        //Mã đơn vị cũ. Chỉ phục vụ cho loại đề xuất luân chuyển
        [MaxLength(20)]
        public string MaDonViCu { get; set; }
        //Mã đơn vị mới. Chỉ phục vụ cho loại đề xuất luân chuyển
        [MaxLength(20)]
        public string MaDonViMoi { get; set; }
        //Mã đơn vị đề xuất. Chỉ phục vụ cho loại đề xuất luân chuyển
        [MaxLength(20)]
        public string MaDonViDeXuat { get; set; }

        [MaxLength(200)]
        public string TenCongTy { get; set; }
    }
}
