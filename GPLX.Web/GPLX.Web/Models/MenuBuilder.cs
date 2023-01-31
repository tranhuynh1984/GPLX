using System.Collections.Generic;
using GPLX.Database.Models;

namespace GPLX.Web.Models
{
    public class MenuBuilder
    {
        public Functions Parent { get; set; }
        public IList<MenuBuilder> ChildFunctions { get; set; }
    }
}
