using GPLX.Core.Extensions;

namespace GPLX.Core.DTO.Request.Process
{
    public class ProcessCreateRequest
    {
        public string Record { get; set; }
        public int Status { get; set; }
        public int ProcessId { get; set; }
        public string ProcessName { get; set; }

        public string CreatorName { get; set; }
        public int Creator { get; set; }
    }
}
