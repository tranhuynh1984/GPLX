using System;
using System.Collections.Generic;

namespace GPLX.Core.DTO.Response.Users
{
    public class UsersSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public int Draw { get; set; }
        public IList<UsersSearchResponseData> Data { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
    }

    public class UsersSearchResponseData
    {
        public string Record { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string UserCode { get; set; }
        public string UserPhone { get; set; }
        public string UserEmail { get; set; }
        public string UserImage { get; set; }
        public int Status { get; set; }
        public string Createby { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string AccountGuid { get; set; }

        public string DepartmentName { get; set; }

        public string GroupName { get; set; }
        public string GroupCode { get; set; }

        public string UnitName { get; set; }
    }
}
