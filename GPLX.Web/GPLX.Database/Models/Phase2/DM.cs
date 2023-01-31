using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class DM : UpdateTime
    {
        //Id cây danh mục
        [Required]
        public int IDTree { get; set; }
        //Mã danh mục
        [MaxLength(10)]
        [Required]
        public string MaDM { get; set; }
        //Tên danh mục
        [MaxLength(200)]
        public string TenDM { get; set; }
        //1: Sử dụng 0:Không sử dụng
        public int IsActive { get; set; }
        //Đánh thứ tự hiển thị nếu cần
        public int Stt { get; set; }
    }
}