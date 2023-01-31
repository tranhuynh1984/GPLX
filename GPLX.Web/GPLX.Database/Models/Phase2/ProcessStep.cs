using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Database.Models.Phase2
{
    /// <summary>
    /// Bảng các bước trong Quy trình
    /// </summary>
    public class ProcessStep : UpdateTime
    {
        /// <summary>
        /// ID step
        /// </summary>
        public int StepId { get; set; }

        /// <summary>
        /// Tên bước
        /// </summary>
        public string StepName { get; set; }

        /// <summary>
        /// Tên quy trình
        /// </summary>

        public int ProcessId { get; set; }

        /// <summary>
        /// Thứ tự các bước trong quy trình
        /// </summary>
        public int OrderStep { get; set; }

        /// <summary>
        /// Đánh dấu bước cuối cùng
        /// </summary>

        public bool IsLastStep { get; set; }

        /// <summary>
        /// Nhóm người dùng duyệt được duyệt Step này
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// Nhóm người dùng duyệt được duyệt Step này
        /// </summary>
        public int ProcessRoleId { get; set; }
    }
}