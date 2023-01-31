using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class CLSCT : UpdateTime
    {
        //ID dòng, khóa chính, tự tăng
        [Required]
        public int IDCLSCT { get; set; }
        //ID Phiếu thu
        [Required]
        public int IDCLS { get; set; }
        //Mã dịch vụ, mã chi phí
        [MaxLength(15)]
        [Required]
        public string MaCP { get; set; }
        //Số lượng
        [Required]
        public decimal SL { get; set; }
        //Đơn giá
        [Required]
        public decimal DG { get; set; }
        //1: Xóa 0: Không xóa
        [Required]
        public int Del { get; set; }
        //Đánh dấu đã đồng bộ sang LIS
        public int SendLab { get; set; }
        //Tiền giảm giá
        public float TongTienGG { get; set; }
        //Tiền sau giảm giá
        public float TongTien { get; set; }
        //Tiền điểm PID
        public float GGDiemPID { get; set; }
        //Tiền thực thu
        public float TongTienSau { get; set; }
        //Đánh dấu đã thanh toán. 0: chưa thanh toán, còn lại là đã thanh toán
        public int CLS_ThanhToan { get; set; }
        //Mã nhóm dịch vụ
        [MaxLength(10)]
        public string MaNhCP { get; set; }
    }
}