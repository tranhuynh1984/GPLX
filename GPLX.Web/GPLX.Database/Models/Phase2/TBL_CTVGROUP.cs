using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class TBL_CTVGROUP : UpdateTime
    {
        //ID nhóm khóa chính
        [Required]
        public int CTVGroupID { get; set; }
        //Tên nhóm
        [MaxLength(300)]
        public string CTVGroupName { get; set; }
    }
}
