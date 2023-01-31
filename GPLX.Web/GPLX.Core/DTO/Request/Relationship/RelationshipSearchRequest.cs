namespace GPLX.Core.DTO.Request.Relationship
{
    public class RelationshipSearchRequest
    {
        public int Status { get; set; }

        public int Draw { get; set; }

        public string RelationshipCode { get; set; }
        public string RelationshipName { get; set; }
        public string Keywords { get; set; }
        public string RequestPage { get; set; }
    }
}
