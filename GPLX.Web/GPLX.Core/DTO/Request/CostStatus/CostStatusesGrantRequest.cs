using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.Extensions;

namespace GPLX.Core.DTO.Request.CostStatus
{
    public class CostStatusesGrantRequest
    {
        public string Record { get; set; }
        public string RequestPage { get; set; }

        public Guid RawId => !string.IsNullOrEmpty(Record) && !string.IsNullOrEmpty(RequestPage)
            ? Guid.TryParse(Record.StringAesDecryption(RequestPage, true), out var g) ? g : Guid.Empty : Guid.Empty;

        public IList<string> Grants { get; set; }
        public string Used { get; set; }
    }
}
