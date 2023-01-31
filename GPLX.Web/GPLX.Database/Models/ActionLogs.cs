using System;

namespace GPLX.Database.Models
{
    public class ActionLogs
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FunctionUnique { get; set; }
        public string Action { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
