using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Database.Models
{
    public class GroupsPermission
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int FunctionId { get; set; }
        public int Permission { get; set; }
    }
}
