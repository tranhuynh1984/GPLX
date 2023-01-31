using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class TBL_CTVGROUPSUB1_DETAIL : UpdateTime
    {
        //ID hợp đồng
        [Required]
        public int SubId { get; set; }
        //Mã dịch vụ
        [Required]
        [MaxLength(50)]
        public string MaCP { get; set; }
        //Tên dịch vụ
        [MaxLength(200)]
        public string TenCP { get; set; }
        //Biểu phí 1
        public float BP1 { get; set; }
        //Biểu phí 2
        public float BP2 { get; set; }
        //Biểu phí 3
        public float BP3 { get; set; }
        //Biểu phí 4
        public float BP4 { get; set; }
        //Biểu phí 5
        public float BP5 { get; set; }
        //Biểu phí 6
        public float BP6 { get; set; }
        //Biểu phí 7
        public float BP7 { get; set; }
        //Biểu phí 8
        public float BP8 { get; set; }
        //Biểu phí 9
        public float BP9 { get; set; }
        //Biểu phí 10
        public float BP10 { get; set; }
        //Biểu phí 11
        public float BP11 { get; set; }
        //Sử dụng
        public int IsActive { get; set; }
    }
}
