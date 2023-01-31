using GPLX.Core.DTO.Response.DM;
using System.Collections.Generic;

namespace GPLX.Core.DTO.Response.DMCTV
{
    public class DMCTVCreateResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<ItemError> ListError { get; set; }
        public DMCTVSearchResponseData Data { get; set; }
    }
}
