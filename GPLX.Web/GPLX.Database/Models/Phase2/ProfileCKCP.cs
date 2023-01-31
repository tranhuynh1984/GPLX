using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class ProfileCKCP : UpdateTime
    {
        //Mã profile
        [MaxLength(20)]
        public string ProfileCKMa { get; set; }
        //Ma dịch vụ
        [MaxLength(15)]
        public string CPMa { get; set; }
        //Trạng thái
        public int IsActive { get; set; }
    }
}
