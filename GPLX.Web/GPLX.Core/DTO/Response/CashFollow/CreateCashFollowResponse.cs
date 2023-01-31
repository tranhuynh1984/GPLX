using System.Collections.Generic;

namespace GPLX.Core.DTO.Response.CashFollow
{
    public class CreateCashFollowResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<string> Errors { get; set; }
        public string Record { get; set; }

    }
}
