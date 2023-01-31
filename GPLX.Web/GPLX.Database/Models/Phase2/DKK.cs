using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class DKK : UpdateTime
    {
        //Mã khách hàng, Khóa chính, Tự tăng
        [Required]
        public int MaBN { get; set; }
        //Mã hồ sơ khách hàng
        [MaxLength(12)]
        public string P_ID { get; set; }
        //Ngày đăng kí khám
        [Required]
        public DateTime Ngay { get; set; }
        //Họ tên khách hàng
        [MaxLength(100)]
        public string HoTen { get; set; }
        //Ngày sinh
        public DateTime NS { get; set; }
        //Năm sinh
        public int NS2 { get; set; }
        //Giới tính: Nam hoặc Nữ
        [MaxLength(3)]
        [Required]
        public string GT { get; set; }
        //Địa chỉ khách hàng
        [MaxLength(250)]
        public string DC { get; set; }
        //Mã tỉnh
        [MaxLength(5)]
        [Required]
        public string MaTinh { get; set; }
        //Mã huyện
        [MaxLength(10)]
        [Required]
        public string MaHuyen { get; set; }
        //Mã  đối tượng
        [MaxLength(10)]
        [Required]
        public string MaDT { get; set; }
        //Mã loại đối tượng
        [MaxLength(5)]
        [Required]
        public string MaLoaiDT { get; set; }
        //Số chứng minh/CCCD/Hộ chiếu
        [MaxLength(15)]
        public string SoCM { get; set; }

        //Số điện thoại khách hàng
        [MaxLength(25)]
        [Required]
        public string Tel { get; set; }
        //Email khách hàng
        [MaxLength(50)]
        [Required]
        public string Email { get; set; }
        //ID hợp đồng khám chữa bệnh
        [Required]
        public int IDHDKCB { get; set; }
        //Xóa dòng. 1: Xóa, 0: Không Xóa
        [Required]
        public int Del { get; set; }
        //Mã đơn vị vào sổ
        [MaxLength(10)]
        public string MaDV { get; set; }
        //Khối doanh thu
        public int KhoiDoanhThu { get; set; }
        //Mã quốc tịch
        [MaxLength(10)]
        public string NgoaiKieu { get; set; }
        //ID pháp nhân
        public int PhapNhanId { get; set; }
        //Tên pháp nhân
        [MaxLength(150)]
        public string PhapNhanName { get; set; }
    }
}