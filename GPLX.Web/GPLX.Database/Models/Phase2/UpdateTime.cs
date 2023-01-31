using System;

namespace GPLX.Database.Models
{
    public class UpdateTime
    {
        public string Createby { get; set; }
        public DateTime Createdate { get; set; }
        public string Updateby { get; set; }
        public DateTime? Updatedate { get; set; }
    }
}
