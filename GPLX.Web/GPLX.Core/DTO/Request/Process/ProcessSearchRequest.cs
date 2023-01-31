namespace GPLX.Core.DTO.Request.Process
{
    public class ProcessSearchRequest
    {
        public int Status { get; set; }

        public int Draw { get; set; }

        public int ProcessId { get; set; }
        public string ProcessName { get; set; }
        public string RequestPage { get; set; }
    }
}
