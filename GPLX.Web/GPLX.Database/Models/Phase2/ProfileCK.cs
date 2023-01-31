using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class ProfileCK : UpdateTime
    {
        public int Id { get; set; }
        //Mã profile
        [MaxLength(20)]
        [Required]
        public string ProfileCKMa { get; set; }
        //Tên profile
        [MaxLength(100)]
        public string ProfileCKTen { get; set; }
        //Mã chuyên khoa
        [MaxLength(20)]
        public string ChuyenKhoaMa { get; set; }
        //1: Có hiệu lực 0: Không có hiệu lực
        public int IsActive { get; set; }

        //Ghi chú
        [MaxLength(3000)]
        public string Note { get; set; }
    }
}
