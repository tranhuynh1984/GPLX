using System;
using System.Collections.Generic;

namespace GPLX.Core.DTO.Response.Department
{
    public class DepartmentSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IEnumerable<DepartmentSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
    }

    public class DepartmentSearchResponseData
    {
        public string Record { get; set; }
        public string Name { get; set; }
        public string StatusName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatorName { get; set; }
    }
}
