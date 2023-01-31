using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.DTO.Request.Users
{
    public class UsersSearchRequest
    {
        public string RequestPage { get; set; }
        public string Keywords { get; set; }

        public string UnitRecord { get; set; }
        public string GroupRecord { get; set; }
        public string DepartmentRecord { get; set; }
        public int Draw { get; set; }
    }
}
