using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class DeXuatGhiChu
    {
        //Note ghi chú các step đề xuất
        //Mã đề xuất
        [MaxLength(20)]
        public string DeXuatCode { get; set; }
        public int ProcessStepId { get; set; }
        public string Note { get; set; }
        public string CreateByCode { get; set; }
        public string CreateByName { get; set; }
        public DateTime? CreateDate { get; set; }
        //Chữ ký
        public string ChuKy { get; set; }
        //Role
        public string CreateByRole { get; set; }
    }
}
