using System;

namespace GPLX.Database.Models
{
    public class Groups
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public string GroupCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatorName { get; set; }
        public int Creator { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
    }
}
