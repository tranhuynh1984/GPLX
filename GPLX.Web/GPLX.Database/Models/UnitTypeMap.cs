using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Database.Models
{
    public class UnitTypeMap
    {
        public Guid Id { get; set; }

        public string UnitCode { get; set; }
        public string Type { get; set; }
        public string TypeName { get; set; }
    }
}
