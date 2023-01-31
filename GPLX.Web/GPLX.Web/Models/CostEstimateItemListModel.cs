using System.Collections.Generic;
using GPLX.Core.DTO.Response.CostStatus;

namespace GPLX.Web.Models
{
    public class CostEstimateItemListModel
    {
        public IEnumerable<StatusesGranted> Stats { get; set; }
        public string AccessToken { set; get; }

        public int DefaultStats { get; set; }
        public string DefaultStatsName { get; set; }

        public string RequestType { get; set; }
        public bool StatsFilterVisible { get; set; }
    }
}
