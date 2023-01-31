using System;

namespace GPLX.Core.DTO.Request.Notify
{
    public class SendFormat
    {
        public Guid RecordId { get; set; }
        /// <summary>
        /// loại kế hoạch
        /// </summary>
        public string PlanType { get; set; }
        public int UnitId { get; set; }
        /// <summary>
        /// nhóm đơn vị
        /// sub....
        /// </summary>
        public string ForSubject { get; set; }
        public int Year { get; set; }
        /// <summary>
        /// người thực hiện thao tác
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Chức vụ người thực hiện
        /// </summary>
        public string PositionCode { get; set; }
        public string PositionName { get; set; }
        /// <summary>
        /// người lập kế hoạch
        /// </summary>
        public int Creator { get; set; }
        /// <summary>
        /// Phê duyệt
        /// </summary>
        public bool IsApproval { get; set; }
        /// <summary>
        /// Bước phê duyệt trong quy trình
        /// </summary>
        public int Level { get; set; }
    }
}
