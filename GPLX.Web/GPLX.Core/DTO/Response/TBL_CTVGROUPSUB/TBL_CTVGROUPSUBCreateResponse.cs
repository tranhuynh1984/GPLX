using System.Collections.Generic;
using GPLX.Core.DTO.Response.DM;

namespace GPLX.Core.DTO.Response.TBL_CTVGROUPSUB
{
    public class TBL_CTVGROUPSUBCreateResponse
    {
        public int SubId { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public List<ItemError> ListError { get; set; }
        public TBL_CTVGROUPSUBSearchResponseData Data { get; set; }
    }
}
