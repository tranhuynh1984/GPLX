using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class LoaiDeXuat : UpdateTime
    {
        //Mã loại đề xuất
        [MaxLength(20)]
        [Required]
        public string LoaiDeXuatCode { get; set; }
        //Tên loại đề xuất
        [MaxLength(100)]
        public string LoaiDeXuatName { get; set; }
        //1: Có hiệu lực 0: Không có hiệu lực
        public int IsActive { get; set; }
        //STT
        public int Stt { get; set; }
    }
}
