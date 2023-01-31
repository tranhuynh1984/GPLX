using System;
using System.ComponentModel.DataAnnotations;

namespace GPLX.Database.Models
{
    public class Relationship : UpdateTime
    {
        [Required]
        public int Id { get; set; }
        //Group quan hệ
        public int GroupRelationshipId { get; set; }
        //Mã quan hệ
        [MaxLength(20)]
        public string RelationshipCode { get; set; }
        //Tên quan hệ
        [MaxLength(100)]
        public string RelationshipName { get; set; }
        //1: Có hiệu lực 0: Không có hiệu lực
        public int IsActive { get; set; }
        //STT
        public int Stt { get; set; }
    }
}
