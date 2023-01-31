using System.Collections.Generic;

namespace GPLX.Database.Models
{

    public class UsersSessionModel
    {
        public int UserId { get; set; }
        public string SyncUserId { get; set; }
        public string UserName { get; set; }
        public DepartmentModel Department { get; set; }
        public IList<PositionModel> Positions { get; set; }
        public UnitModel Unit { get; set; }
        public string PathSignature { get; set; }
        public string AccountSignature { get; set; }
        public string PasswordSignature { get; set; }
    }

    // Phòng ban
    public class DepartmentModel
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }

    // Chức vụ
    public class PositionModel
    {
        public int PositionId { get; set; }
        public string PositionName { get; set; }
        public string PositionCode { get; set; }

        // Vị trí của thành viên trong tổ chức
        // i.e nhân viên = 1, trưởng phòng  = 2, kế toán = 3, KTT = 4...
        public int PositionLevel { get; set; }
    }

    public class UnitModel
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string UnitCode { get; set; }
        /// <summary>
        /// Loại đơn vị
        /// Y tế - Ngoài y tế
        /// In - Out
        /// </summary>
        public string UnitType { get; set; }
        public bool IsSub { get; set; }
    }
}
