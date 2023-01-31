using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class DMBS_ChuyenKhoa : UpdateTime
    {
        //Mã chuyên khoa
        [MaxLength(20)]
        [Required]
        public string Ma { get; set; }
        //Tên chuyên khoa
        [MaxLength(50)]
        public string Ten { get; set; }
        //1: Sử dụng 0:Không sử dụng
        public int IsActive { get; set; }
        //STT
        public int Stt { get; set; }
    }
}
