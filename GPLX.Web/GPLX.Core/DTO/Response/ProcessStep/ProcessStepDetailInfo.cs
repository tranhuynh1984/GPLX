using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.ProcessStep
{
    public class ProcessStepDetailInfo : UpdateTimeResponseData
    {
        public int StepId { get; set; }
        public string StepName { get; set; }
        public int ProcessId { get; set; }
        public int OrderStep { get; set; }
        public bool IsLastStep { get; set; }
        public int GroupId { get; set; }
        public int ProcessRoleId { get; set; }
        public string ProcessRoleName { get; set; }
        public string Note { get; set; }
    }
}
