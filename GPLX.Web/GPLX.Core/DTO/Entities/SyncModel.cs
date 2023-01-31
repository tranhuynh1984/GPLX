using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.DTO.Entities
{
    public class SyncModel
    {
        public string AccessToken { get; set; }
        public string UnitEndPoint { get; set; }
        public string UserEndPoint { get; set; }
        public string UserByUnitCodeEndPoint { get; set; }
    }
}
