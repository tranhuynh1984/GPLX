using System;
using GPLX.Core.Extensions;

namespace GPLX.Core.DTO.Request.Profit
{
    public class ProfitPlanViewHistoryRequest
    {
        public string Record { get; set; }
        public string PageRequest { get; set; }

        //giải mã ID
        public Guid RawId => Guid.TryParse(Record.StringAesDecryption(PageRequest, true), out var g) ? g : Guid.Empty;
    }
}
