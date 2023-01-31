using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class DMPN : UpdateTime
    {
        //ID PHÁP NHÂN
        [Required]
        public int PhapNhanId { get; set; }
        //TÊN PHÁP NHÂN
        [MaxLength(200)]
        public string PhapNhanName { get; set; }
        //Sử dụng không? 1: sử dụng, 0: không sử dụng
        public int IsActive { get; set; }
        //Tên công ty
        [MaxLength(500)]
        public string CompanyName { get; set; }
        //Mã số thuế
        [MaxLength(50)]
        public string TaxNumber { get; set; }
        //Đại chỉ công ty
        [MaxLength(500)]
        public string AddressCompany { get; set; }
    }
}
