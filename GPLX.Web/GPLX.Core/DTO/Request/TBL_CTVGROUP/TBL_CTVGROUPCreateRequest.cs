using GPLX.Core.Extensions;

namespace GPLX.Core.DTO.Request.TBL_CTVGROUP
{
    public class TBL_CTVGROUPCreateRequest
    {
        public string Record { get; set; }
        public int Status { get; set; }
        public int CTVGroupID { get; set; }
        public string CTVGroupName { get; set; }

        public string CreatorName { get; set; }
        public int Creator { get; set; }
    }
}
