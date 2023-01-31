using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.Extensions;

namespace GPLX.Core.DTO.Request.RevenuePlan
{
    public class RevenuePlanViewHistoryRequest
    {
        public string Record { get; set; }
        public string PageRequest { get; set; }

        //giải mã ID
        public Guid RawId => Guid.TryParse(Record.StringAesDecryption(PageRequest, true), out var g) ? g : Guid.Empty;
    }
}
