using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class DeXuatChiTiet : UpdateTime
    {
        //Phục vụ cho đề xuất sửa CTV
        //Mã đề xuất
        [MaxLength(20)]
        public string DeXuatCode { get; set; }
        //Tên trường
        [MaxLength(100)]
        public string FieldName { get; set; }
        [MaxLength(100)]
        //Giá trị cũ
        public string ValueOld { get; set; }
        //Giá trị mới
        [MaxLength(100)]
        public string ValueNew { get; set; }
        //Ghi cú
        [MaxLength(500)]
        public string Note { get; set; }
    }
}
