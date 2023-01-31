using GPLX.Core.DTO.Response;
using GPLX.Core.DTO.Response.CostEstimate;

namespace GPLX.Web.Models
{
    public class CostEstimateCreateModel
    {
        public string ViewMode { get; set; }
        public bool EnableCreate { get; set; }
        public bool EnableApprove { get; set; }

        public CostEstimateViewResponse DataView { get; set; }

        public string Record { get; set; }
        public int ReportForWeek { get; set; }
        public int RequestType { get; set; }
    }
}
