using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class NhCP
    {
        //Mã nhom DV
        [MaxLength(5)]
        [Required]
        public string MaNhCP { get; set; }
        //Tên nhom DV
        [MaxLength(50)]
        public string TenNhCP { get; set; }
        //STT
        public int Stt { get; set; }
        //1: Sử dụng 0:Không sử dụng
        public int IsActive { get; set; }
    }
}
