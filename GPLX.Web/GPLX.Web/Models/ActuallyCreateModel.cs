
using System.Collections.Generic;
using GPLX.Core.DTO.Response;
using GPLX.Core.DTO.Response.Actually;
using GPLX.Database.Models;

namespace GPLX.Web.Models
{
    public class ActuallyCreateModel
    {
        public IEnumerable<ActuallySpentItemResponse> DataView { get; set; }
        public string Record { get; set; }
        public bool EnableCreate { get; set; }

        public bool IsError { get; set; }
        public string Message { get; set; }

        public int ReportForWeek { get; set; }
        public string ReportForWeekName { get; set; }
        public int Status { get; set; }

        public bool Partial { get; set; }
        public IList<CostEstimateItemType> Type { get; set; }

    }
}
