using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.DTO.Response.Users
{
    public class UserSync
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public object UserPhone { get; set; }
        public object UserEmail { get; set; }
        public string UserImage { get; set; }
        public object Password { get; set; }
        public int Status { get; set; }
        public string Createby { get; set; }
        public DateTime Createdate { get; set; }
        public string Updateby { get; set; }
        public DateTime Updatedate { get; set; }
        public string AccountGuid { get; set; }
        public object Digitalsignature { get; set; }
        public object Salt { get; set; }
        public string OfficesCode { get; set; }
        public string OfficesType { get; set; }
        public string OfficesSub { get; set; }
    }
}
