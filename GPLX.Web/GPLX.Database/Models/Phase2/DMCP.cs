using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class DMCP : UpdateTime
    {
        //Mã dịch vụ
        [MaxLength(15)]
        [Required]
        public string MaCP { get; set; }
        //Mã nhập nhanh của dịch vụ
        [MaxLength(15)]
        [Required]
        public string MaNN { get; set; }
        //Tên dịch vụ
        [MaxLength(200)]
        public string TenCP { get; set; }
        //Tên tiếng anh của dịch vụ
        [MaxLength(200)]
        public string TenCPE { get; set; }
        //Đơn vị tính
        [MaxLength(20)]
        public string DVT { get; set; }
        //Đơn giá
        public float DG { get; set; }
        //Đơn giá bảo hiểm
        public float DGBH { get; set; }
        //1: Sử dụng 0: Không sử dụng
        public int IsActive { get; set; }
        //Mã nhóm dịch vụ
        [MaxLength(5)]
        public string MaNhCP { get; set; }
        //Mã loại kỹ thuật 1
        [MaxLength(10)]
        [Required]
        public string MaLoaiKT1 { get; set; }
        //Mã loại kỹ thuật 2
        [MaxLength(10)]
        public string MaLoaiKT2 { get; set; }
        //Mã loại kỹ thuật 3
        [MaxLength(10)]
        public string MaLoaiKT3 { get; set; }
        //Phân nhóm
        [MaxLength(20)]
        public string PhanNhom { get; set; }
        //Tên rút gọn của dịch vụ
        [MaxLength(200)]
        public string TenRutGon { get; set; }
        //Khóa giảm giá. 1: Không được phép giảm giá 0: Được phép giảm giá
        public int KhoaGGTrucTiep { get; set; }
    }
}
