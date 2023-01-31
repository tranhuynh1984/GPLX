using System;

namespace GPLX.Database.Models
{
    public class UserConcurrently
    {
        public Guid Id { get; set; }
        public int UnitId { get; set; }
        public int UserId { get; set; }
        public string UnitName { get; set; }
        public string UnitCode { get; set; }
        public bool Selected { get; set; }
        public int GroupId { get; set; }
        public string GroupCode { get; set; }
    }
}
