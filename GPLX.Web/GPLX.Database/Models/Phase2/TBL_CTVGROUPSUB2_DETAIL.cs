using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class TBL_CTVGROUPSUB2_DETAIL : UpdateTime
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
        //Giá cố định
        public float FixedPrice { get; set; }
        //Sử dụng
        public int IsActive { get; set; }
    }
}
