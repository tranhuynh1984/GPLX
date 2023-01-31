using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class ProcessRole : UpdateTime
    {
        //Id
        [Required]
        public int IDRole { get; set; }
        //Mã danh mục
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }
    }
}