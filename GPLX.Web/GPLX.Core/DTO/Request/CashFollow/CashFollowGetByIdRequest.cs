using System;
using GPLX.Core.Extensions;

namespace GPLX.Core.DTO.Request.CashFollow
{
    public class CashFollowGetByIdRequest
    {
        public string Record { get; set; }

        public Guid RawId => !string.IsNullOrEmpty(Record) && !string.IsNullOrEmpty(RequestPage)
            ? Guid.TryParse(Record.StringAesDecryption(RequestPage, true), out var g) ? g : Guid.Empty
            : Guid.Empty;
        public string RequestPage { get; set; }
    }
}
