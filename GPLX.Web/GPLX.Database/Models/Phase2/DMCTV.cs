using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class DMCTV : UpdateTime
    {
        //Mã bác sĩ
        [MaxLength(10)]
        [Required]
        public string MaBS { get; set; }

        //Tên bác sĩ
        [MaxLength(100)]
        public string TenBS { get; set; }

        //Người đại diện
        [MaxLength(100)]
        public string NguoiDaiDien { get; set; }

        //Ngày sinh
        public DateTime NS { get; set; }

        //Mã chức danh
        [MaxLength(20)]
        public string MaChucDanh { get; set; }

        //Mã chuyên khoa
        [MaxLength(20)]
        public string ChuyenKhoa { get; set; }

        //Địa chỉ nhà riêng
        [MaxLength(190)]
        public string DC1 { get; set; }

        //Mã tỉnh nhà riêng
        [MaxLength(10)]
        public string MaTinh { get; set; }

        //Mã huyện nhà riêng
        [MaxLength(10)]
        public string MaHuyen { get; set; }

        //Địa chỉ công ty
        [MaxLength(190)]
        public string DC2 { get; set; }

        //Mã tỉnh công ty
        [MaxLength(10)]
        public string MaTinh2 { get; set; }

        //Mã huyện công ty
        [MaxLength(10)]
        public string MaHuyen2 { get; set; }

        //Số điện thoại di động của CTV hoặc người đại diện đơn vị
        [MaxLength(50)]
        public string Mobi { get; set; }

        //Số điện thoại cố định của đơn vị
        [MaxLength(11)]
        public string Tel { get; set; }

        //Thu sau không? 1: có, 0: không
        [Required]
        public int CK { get; set; }

        //Mã hình thức chiết khấu đối tượng
        [MaxLength(20)]
        public string HTCKDoiTuong { get; set; }

        //Số CMNN/CCCD
        [MaxLength(20)]
        public string CMND { get; set; }

        //Ngày cấp CMND
        public DateTime NgayCapCMND { get; set; }

        //Nơi cấp CMND
        [MaxLength(100)]
        public string NoiCapCMND { get; set; }

        //Emai CTV
        [MaxLength(50)]
        public string Email { get; set; }

        //Số tài khoản CTV
        [MaxLength(30)]
        public string SoTK { get; set; }

        //Tên ngân hàng
        [MaxLength(110)]
        public string Bank { get; set; }

        //Tên chủ tài khoản
        [MaxLength(100)]
        public string TenChuTK { get; set; }

        //Số hợp đồng
        [MaxLength(50)]
        public string SoHD { get; set; }

        //Mã đơn vị cộng tác (đơn vị quản lý)
        [MaxLength(10)]
        public string MaDVCT { get; set; }

        //Mã số thuế của cá nhân và đơn vị
        [MaxLength(30)]
        public string Fax { get; set; }

        //Trạng thái on/off ctv? 1: on/sử dụng, off/không sử dụng
        [Required]
        public int IsActive { get; set; }

        //Lý do khoá mã CTV
        [MaxLength(150)]
        public string LyDoIsActive { get; set; }

        //Bác sĩ trả sau
        [Required]
        public int TraSau { get; set; }

        //Mã đối tượng CTV
        [MaxLength(20)]
        public string MaDTCTV { get; set; }

        //User đang nhập vào web của BS
        [MaxLength(50)]
        public string UserWeb { get; set; }

        //Pass web
        [MaxLength(30)]
        public string PassWeb { get; set; }

        //Mã chi nhánh
        [MaxLength(20)]
        public string ChiNhanh { get; set; }

        //Ghi chú
        [MaxLength(500)]
        public string GhiChu { get; set; }

        //Mã số thuế
        [MaxLength(50)]
        public string MaSoThue { get; set; }

        //Số chứng chỉ hành nghề (CCHN)
        [MaxLength(50)]
        public string ChungChi_So { get; set; }

        //Ngày cấp CCHN
        public DateTime ChungChi_NgayCap { get; set; }

        //Nơi cấp CCHN
        [MaxLength(50)]
        public string ChungChi_NoiCap { get; set; }

        //Ngày ký hợp đồng
        public DateTime NgayKyHD { get; set; }

        //Tên viết tắt của CTV
        [MaxLength(20)]
        public string TenVietTat { get; set; }

        //Thông tin người đề xuất
        [MaxLength(250)]
        public string TTDeXuat { get; set; }

        //Giới tính Nam hoặc Nữ
        [MaxLength(3)]
        public string GT { get; set; }

        //Chiết khấu khách hàng
        [MaxLength(30)]
        public string CKKH { get; set; }

        //Ngày kết thúc hợp đồng
        public DateTime KetThucHD { get; set; }

        //Đã đầy đủ hồ sơ chưa? 1 là có, 0 là không
        public int DaHoanThienHoSo { get; set; }

        //Mã khoa BHYT
        [MaxLength(3)]
        public string BH_Ma_Khoa { get; set; }

        //Chữ ký điện tử của BS
        public string ChuKy { get; set; }

        //Mã SAP
        [MaxLength(20)]
        public string MaSap { get; set; }

        //Số phụ lục
        [MaxLength(100)]
        public string SoPhuLuc { get; set; }

        //Ngày ký phụ lục
        public DateTime NgayKyPL { get; set; }

        //Ngày kết thúc phụ lục
        public DateTime NgayKetThucPL { get; set; }

        /// <summary>
        /// 0: Không trong quy trình nào, 1: Đang trong 1 quy trình nào đó
        /// </summary>
        public int CtvStatus { get; set; }
    }
}