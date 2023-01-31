using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class DMHuyen : UpdateTime
    {
        //Mã huyện
        [MaxLength(10)]
        [Required]
        public string MaHuyen { get; set; }
        //Tên huyện
        [MaxLength(50)]
        [Required]
        public string TenHuyen { get; set; }
        //Mã tỉnh
        [MaxLength(10)]
        [Required]
        public string MaTinh { get; set; }
        //1: Sử dụng 0:Không sử dụng
        [Required]
        public int IsActive { get; set; }
    }
}
