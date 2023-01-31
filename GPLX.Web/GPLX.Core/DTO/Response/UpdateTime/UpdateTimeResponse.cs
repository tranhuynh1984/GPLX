using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.DTO.Response.UpdateTime
{
    public class UpdateTimeResponseData
    {
        public string Createby { get; set; }
        public DateTime Createdate { get; set; }
        public string Updateby { get; set; }
        public DateTime? Updatedate { get; set; }
        public string CreatedateString { get; set; }
        public string UpdatedateString { get; set; }
    }
}
