namespace GPLX.Core.DTO.Request.ProcessStep
{
    public class ProcessStepSearchRequest
    {
        public int Status { get; set; }

        public int Draw { get; set; }

        public int StepId { get; set; }
        public string StepName { get; set; }
        public int ProcessId { get; set; }
        public int OrderStep { get; set; }
        public bool IsLastStep { get; set; }
        public int GroupId { get; set; }
        public int ProcessRoleId { get; set; }
        public string RequestPage { get; set; }
    }
}
