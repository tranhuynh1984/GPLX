using System;

namespace GPLX.Database.Models
{
    public class CostStatusesGroups
    {
        public Guid Id { get; set; }
        /// <summary>
        /// ID của status bên bảng CostStatuses
        /// </summary>
        public Guid StatusesId { get; set; }
        // Mã của nhóm
        public string GroupCode { get; set; }


        /// <summary>
        /// Nếu type = Used ==> trạng thái được dùng bởi chức vụ nào
        /// Còn nếu trống thì là trạng thái đc map để view
        /// </summary>
        public string Type { get; set; }
       
    }
}
