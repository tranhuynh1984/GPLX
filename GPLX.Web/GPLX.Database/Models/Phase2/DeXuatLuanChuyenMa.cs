using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class DeXuatLuanChuyenMa : UpdateTime
    {
        //Mã đề xuất
        [MaxLength(20)]
        public string DeXuatCode { get; set; }
        //Mã CTV
        [MaxLength(10)]
        public string MaCTV { get; set; }
        //Tên CTV
        [MaxLength(100)]
        public string TenCTV { get; set; }
        public DateTime ThoiGianKhoa { get; set; }

        [MaxLength(500)]
        public string Note { get; set; }
    }
}
