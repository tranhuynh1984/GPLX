using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class DMDV : UpdateTime
    {
        //Mã đơn vị
        [MaxLength(20)]
        [Required]
        public string MaDV { get; set; }
        //Tên đơn vị
        [MaxLength(100)]
        public string TenDV { get; set; }
        //Sử dụng không? 1: sử dụng, 0: không sử dụng
        [Required]
        public int IsActive { get; set; }
        //ID pháp nhân
        public int PhapNhanId { get; set; }
        //Mã SAP đơn vị pháp nhân
        [MaxLength(20)]
        public string MaSAP { get; set; }
        //Mã SAP công ty thành viên trên SAP
        [MaxLength(50)]
        public string MaDVThanhVien { get; set; }
        //Mã đánh dấu đơn vị để đẩy lên SAP
        [MaxLength(50)]
        public string MaDVExSap { get; set; }
    }
}
