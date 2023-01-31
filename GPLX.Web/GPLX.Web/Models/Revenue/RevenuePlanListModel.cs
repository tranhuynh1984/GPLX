using System.Collections.Generic;
using GPLX.Core.DTO.Response.CostStatus;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GPLX.Web.Models.Revenue
{
    public class RevenuePlanListModel
    {
        public IEnumerable<StatusesGranted> Stats { get; set; }

        public List<SelectListItem> Units { get; set; }

        public int DefaultStats { get; set; }
        public string DefaultStatsName { get; set; }

        public bool EnableSearchUnit { get; set; }
    }
}
