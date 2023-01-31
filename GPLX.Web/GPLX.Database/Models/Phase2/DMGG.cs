using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class DMGG : UpdateTime
    {
        //Mã chương trình giảm giá
        [MaxLength(10)]
        [Required]
        public string MaGG { get; set; }

        //Tên giảm giá
        [MaxLength(200)]
        [Required]
        public string TenGG { get; set; }

        //% giảm giá
        [Required]
        public decimal GiamGia { get; set; }

        //1: Sử dụng 0: Không sử dụng
        [Required]
        public int IsActive { get; set; }
    }
}