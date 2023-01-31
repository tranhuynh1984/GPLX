using System;
using System.Collections.Generic;
using System.Globalization;
using GPLX.Core.DTO.Response.CostStatus;
using GPLX.Database.Models;

namespace GPLX.Core.DTO.Request.Actually
{
    public class SearchActuallySpentRequest
    {
        public string Keywords { get; set; }
        public string KeywordsNonUnicode { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int Status { get; set; }

        public int FilterWeek { get; set; }

        public DateTime DateFrom
            => DateTime.TryParseExact(FromDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out var result) ? result : DateTime.Now;


        public DateTime DateTo
            => DateTime.TryParseExact(ToDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out var result) ? result : DateTime.Now;
        public int Draw { get; set; }

        // Mã ID của đơn vị
        // Đơn vị nào chỉ nhìn thấy thực chi của đơn vị đó

        public int UserUnit { get; set; }

        // Key của page
        // Dùng để mã hóa ID
        public string PageRequest { get; set; }
        public IEnumerable<StatusesGranted> StatusAllowsSeen { get; set; }
        public bool IsSub { get; set; }

        public bool PermissionEdit { get; set; }
        public bool PermissionApprove { get; set; }
        public IList<UserUnitsManages> UserUnitsManages { get; set; }
    }
}
