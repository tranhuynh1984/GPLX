using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Database.Models.Phase2
{
    /// <summary>
    /// Luồng duyệt
    /// </summary>
    public class ApprovedFlow : UpdateTime
    {
        public int FlowId { get; set; }
        public string FlowName { get; set; }
        public string DoctorId { get; set; }
        public int ProcessId { get; set; }
        public string UnitCode { get; set; }
        public int StatusFlow { get; set; }
        public DateTime SendDate { get; set; }
    }

    /// <summary>
    /// Các bước trong luồng duyệt
    /// </summary>
    public class ApprovedFlowDetail : UpdateTime
    {
        public long FlowDetailId { get; set; }
        public int FlowId { get; set; }
        public int StepId { get; set; }

        /// <summary>
        /// Danh sách User được duyệt ở bước này
        /// </summary>
        public string ListUserAccept { get; set; }

        /// <summary>
        /// User đã duyệt or không duyệt
        /// </summary>
        public string ApprovedUser { get; set; }

        /// <summary>
        /// Lý do từ chối
        /// </summary>
        public string Reason { get; set; }

        public bool IsLastStep { get; set; }
        public int Status { get; set; }
    }
}