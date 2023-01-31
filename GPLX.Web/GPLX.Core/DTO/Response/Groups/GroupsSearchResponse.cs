using System;
using System.Collections.Generic;
using GPLX.Core.DTO.Response.Department;

namespace GPLX.Core.DTO.Response.Groups
{
    public class GroupsSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IEnumerable<GroupsSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
    }

    public class GroupsSearchResponseData
    {
        public string Record { get; set; }
        public string Name { get; set; }
        public string GroupCode { get; set; }
        public int Order { get; set; }

        public string StatusName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatorName { get; set; }
    }
}
