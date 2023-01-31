using System;

namespace GPLX.Database.Models
{
    public class SpecialUnitFollowConfigs
    {
        public Guid Id { get; set; }
        public int UnitId { get; set; }
        public string UnitCode { get; set; }
        public string UnitShortName { get; set; }
    }
}
