using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Database.Models;

namespace GPLX.Core.DTO.Response.CostStatus
{
    public class CostStatusFollowResponse
    {
        public CostStatuses CostStatuses { get; set; }
        public IList<Database.Models.Groups> Groups { get; set; }
        public Database.Models.Groups Use { get; set; }
    }
}
