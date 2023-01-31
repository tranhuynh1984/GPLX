using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class HDKCB : UpdateTime
    {
        //ID hợp đồng
        [Required]
        public int IDHD { get; set; }
        //Mã loại HĐ
        [Required]
        [MaxLength(15)]
        public string MaLoai { get; set; }
        //Mã hợp đồng
        [MaxLength(20)]
        [Required]
        public string MaHD { get; set; }
        //Tên hợp đồng
        [MaxLength(100)]
        [Required]
        public string TenHD { get; set; }
        //Ngày ký hợp đồng
        public DateTime? Ngay { get; set; }
        //1: Xóa 0: Không xóa
        [Required]
        public int Del { get; set; }
        //1: Có hiệu lực 0: Không có hiệu lực
        [Required]
        public int IsActive { get; set; }
        //Ngày bắt đầu hợp đồng
        public DateTime? ND { get; set; }
        //Ngày kết thúc hợp đồng
        public DateTime? NS { get; set; }
    }
}
