using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GPLX.Web.Models.Dashboard
{
    public class SearchModel
    {
        public IList<SelectListItem> Plans { get; set; }
        public IList<SelectListItem> Units { get; set; }
        public IList<SelectListItem> Stats { get; set; }
        public string DefaultStatsName { get; set; }
        public int DefaultStats { get; set; }
    }
}
