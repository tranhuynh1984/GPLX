using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class GiaKCB
    {
        //ID dòng tự tăng
        [Required]
        public int IDGiaCT { get; set; }
        //ID Hợp đồng khám chữa bệnh
        [Required]
        public int IDHD { get; set; }
        //Mã dịch vụ
        [MaxLength(15)]
        [Required]
        public string MaCP { get; set; }
        //Số lượng
        public decimal SL { get; set; }
        //Đơn giá
        public decimal DG { get; set; }
        //1: Xóa 0: Không xóa
        [Required]
        public int Del { get; set; }
        //Ngày thêm
        public DateTime? NgayAuto { get; set; }
        
    }
}
