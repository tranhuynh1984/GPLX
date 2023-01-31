using GPLX.Core.Extensions;

namespace GPLX.Core.DTO.Request.Relationship
{
    public class RelationshipCreateRequest
    {
        public string Record { get; set; }
        public int Status { get; set; }
        public string RelationshipCode { get; set; }
        public string RelationshipName { get; set; }
        public int IsActive { get; set; }
        public int Stt { get; set; }
        public string CreatorName { get; set; }
        public int Creator { get; set; }
    }
}
