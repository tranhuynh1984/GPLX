using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class TBL_CTVGROUPSUB : UpdateTime
    {
        //ID hợp đồng CTV
        [Required]
        public int SubId { get; set; }
        //ID nhóm CTV
        public int CTVGroupID { get; set; }
        //Tên hợp đồng CTV
        [MaxLength(300)]
        public string SubName { get; set; }
        //Từ ngày
        public DateTime FromDate { get; set; }
        //Đến ngày
        public DateTime ToDate { get; set; }
        //Ghi chú
        [MaxLength(3000)]
        public string Note { get; set; }
        //1: Sử dụng 0:Không sử dụng
        public int IsUse { get; set; }
        //Lý do không sử dụng
        [MaxLength(2000)]
        public string WhyNotUse { get; set; }
        //Ngày thêm
        public DateTime DateI { get; set; }
        //Người thêm
        [MaxLength(30)]
        public string UserI { get; set; }
        //% Chiết khấu
        public float DisCount { get; set; }
        //Giá khách hàng
        public float CustomerPrice { get; set; }
    }
}
