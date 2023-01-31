using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.DTO.Response.Permission
{
    public class PermissionGrantResponse
    {
        public int FuncId { get; set; }
        public string FuncName { get; set; }
        public bool Checked { get; set; }
        public IList<PermissionGrantResponse> Children { get; set; }
        public string Unique { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }
    }
}
