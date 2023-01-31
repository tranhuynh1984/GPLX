using GPLX.Core.Extensions;

namespace GPLX.Core.DTO.Request.ProcessStep
{
    public class ProcessStepCreateRequest
    {
        public string Record { get; set; }
        public int Status { get; set; }
        public int StepId { get; set; }
        public string StepName { get; set; }
        public int ProcessId { get; set; }
        public int OrderStep { get; set; }
        public bool IsLastStep { get; set; }
        public int GroupId { get; set; }
        public int ProcessRoleId { get; set; }
        public string CreatorName { get; set; }
        public int Creator { get; set; }
    }
}
