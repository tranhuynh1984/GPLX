using System;
using System.Collections.Generic;
using System.Globalization;
using GPLX.Core.DTO.Response.CostStatus;
using GPLX.Core.Extensions;
using GPLX.Database.Models;

namespace GPLX.Core.DTO.Request.CostEstimateItem
{
    public class CostEstimateItemSearchRequest
    {
        public string Keywords { get; set; }
        // từ khóa tìm kiếm đã được loại bỏ dấu
        public string KeywordsNonUnicode { get; set; }
        // format: vi-VN (dd/MM/yyyy)
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int Status { get; set; }

        public DateTime DateFrom
            => DateTime.TryParseExact(FromDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out var result) ? result : DateTime.Now;


        public DateTime DateTo
            => DateTime.TryParseExact(ToDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out var result) ? result.AddHours(24).AddSeconds(-1) : DateTime.Now;

        public int FilterWeek { get; set; }

        // danh sách các trạng thái của phiếu mà user hiện tại được xem
        public IEnumerable<StatusesGranted> StatusAllowsSeen { get; set; }

        public int Draw { get; set; }

        // Mã ID của đơn vị
        // Đơn vị nào chỉ nhìn được yêu cầu của đơn vị đó
        
        public int UserUnit { get; set; }
        // Mã ID của phòng ban trong đơn vị
        // Phòng ban nào chỉ được nhìn yêu cầu của phòng ban đó
        // Áp dụng từ cấp trưởng phòng xuống

        
        public int UserDepartmentId { get; set; }

        public int UserId { get; set; }

        // Key của page
        // Dùng để mã hóa ID
        public string PageRequest { get; set; }
        // Loại phiếu yêu cầu

        public string RequestType { get; set; }
        /// <summary>
        /// ID của dự trù đang được chỉnh sửa
        /// </summary>
        public Guid Current { get; set; }

        public string CurrentCostEstimateRecord { get; set; }

        
        /// <summary>
        /// Loại yêu cầu năm - tuần
        /// </summary>
        public int EstimateType { get; set; }

        public int ReportWeek { get; set; }
        public string FileHostView { get; set; }
        public bool IsSub { get; set; }

        public bool PermissionEdit { get; set; }
        public bool PermissionApprove { get; set; }
        public bool PermissionDelete { get; set; }
        /// <summary>
        /// Trường hợp nếu là CB Hành chính thì chỉ đc xem y/c của mình
        /// </summary>
        public int EqualUser { get; set; }
        /// <summary>
        /// Danh sách đơn vị user được quản lý
        /// </summary>
        public IList<UserUnitsManages> UserUnitsManages { get; set; }
    }

    public class CostEstimateItemApprovalRequest
    {
        // Duyệt hay từ chối
        public bool IsApproval { get; set; }

        public string Reason { get; set; }
        public string UnitType { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }

        public IList<PositionModel> Positions { get; set; }
        public string CostEstimateId { get; set; }
        public string PageRequest { get; set; }
        /// <summary>
        /// Loại dự trù năm - tuần
        /// </summary>
        public int Type { get; set; }

        //giải mã ID
        public Guid RawId => Guid.TryParse(CostEstimateId.StringAesDecryption(PageRequest), out var g) ? g : Guid.Empty;

        public List<string> Records { get; set; }

        //Trạng thái thay đổi
        public int StatusChange { get; set; }
        public string StatusChangeName { get; set; }

        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string UnitCode { get; set; }

        // danh sách các trạng thái của phiếu mà user hiện tại được xem
        public IEnumerable<StatusesGranted> StatusAllowsSeen { get; set; }
        public bool IsSub { get; set; }
        public bool PermissionEdit { get; set; }
    }
}
