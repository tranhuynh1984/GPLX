using System;
using System.Collections.Generic;

namespace GPLX.Core.DTO.Response.Dashboard
{
    public class DashboardListResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<DashboardListResponseData> Data { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
        public int Draw { get; set; }
    }

    public class DashboardListResponseData
    {
        public string Record { get; set; }
        /// <summary>
        /// Tên loại kế hoạch
        /// </summary>
        public string PlanName { get; set; }

        public string PlanType { get; set; }

        public string CreatorName { get; set; }
        public string UnitName { get; set; }
        public int Year { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PathPdf { get; set; }
        public string StatusName { get; set; }
    }
}
