using GPLX.Core.DTO.Response.DM;
using System.Collections.Generic;

namespace GPLX.Core.DTO.Response.Relationship
{
    public class RelationshipCreateResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<ItemError> ListError { get; set; }
        public RelationshipSearchResponseData Data { get; set; }
    }
}
