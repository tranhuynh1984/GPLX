using System;

namespace GPLX.Database.Models
{
    public class UserGroups
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
    }
}
