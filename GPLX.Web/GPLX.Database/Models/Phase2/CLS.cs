using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class CLS : UpdateTime
    {
        //ID Phiếu thu, tự tăng, khóa chính
        [Required]
        public int IDCLS { get; set; }
        //Mã định danh SID 10 số
        [MaxLength(15)]
        [Required]
        public string S_ID { get; set; }
        //Mã khách hàng, khóa ngoại DKK
        [Required]
        public int MaBN { get; set; }
        //Họ tên khách hàng
        [MaxLength(100)]
        public string HoTen { get; set; }
        //Ngày đăng kí
        [Required]
        public DateTime Ngay { get; set; }
        //Mã đối tượng
        [MaxLength(10)]
        [Required]
        public string MaDT { get; set; }
        //1: Ngoại trú 0: Nội trú
        [Required]
        public int NgTru { get; set; }
        //Ngày thu tiền
        public DateTime NgayThu { get; set; }
        //User tài chính (User thu ngân)
        [MaxLength(30)]
        [Required]
        public string UserTC { get; set; }
        //Xác nhận thu ngân
        [Required]
        public int QuaTC { get; set; }
        //Đã thu tiền
        [Required]
        public int DaTT { get; set; }
        //1: Xóa 0: Không xóa
        [Required]
        public int Del { get; set; }
        //Tiền trả trước
        [Required]
        public decimal TraTruoc { get; set; }

        //Tiền mặt
        [Required]
        public decimal TienMat { get; set; }
        //Tiền POS
        [Required]
        public decimal ThePOS { get; set; }
        //Tiền chuyển khoản
        [Required]
        public decimal ChKhoan { get; set; }
        //Mã bác sĩ chỉ định
        [MaxLength(30)]
        [Required]
        public string UserCD { get; set; }
        //Tiền giảm giá
        [Required]
        public decimal TienGG { get; set; }
        //Tiền thẻ tích điểm PID
        public decimal TTDiemPID { get; set; }
        //Khối doanh thu
        public int KhoiDoanhThu { get; set; }
        //Tiền Voucher
        public decimal TienVoucher { get; set; }
        //Mã định danh khách hàng SID Full
        [MaxLength(50)]
        public string SIDFull { get; set; }

        //Đánh dấu bác sĩ trả sau. 1: BS trả sau 0: Không phải trả sau
        public int IsBSTraSau { get; set; }
        //Đánh dấu đã đồng bộ sang LIS
        public int SendLab { get; set; }
        //Tiền điện tử QR
        public decimal TienQR { get; set; }
        //Tiền đi lại
        public decimal Tiendilai { get; set; }
        //Mã đơn vị quản lý bác sĩ
        [MaxLength(20)]
        public string MaDVQL { get; set; }
    }
}