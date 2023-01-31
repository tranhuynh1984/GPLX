using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.DTO.Response.CostStatus;

namespace GPLX.Core.Enum
{
    public static class GlobalEnums
    {

        public const string ErrorMessage = "Lỗi hệ thống, vui lòng thử lại sau!";
        public const string NoContentMessage = "Không tìm thấy dữ liệu yêu cầu!";
        public const string UnAuthorMessage = "Bạn không có quyền thực hiện thao tác này!";

        public enum ResponseCodeEnum
        {
            Success = 200,
            Error = 500,
            NoContent = 204,
            UnAuthor = 401,

            //Trạng thái cho các bản ghi k được phép chỉnh sửa
            NotOpenCode = 402
        }
        public enum StatusDefaultEnum
        {
            Active = 1,
            InActive = 0,
            Decline = -1,
            Temporary = -2, // trạng thái bản ghi tạm
            All = -100,
            Deleted = -9999,

            Activator = -8888
        }

        public enum StatusDefaultType
        {
            Weekly = 0, //Tuần
            Year = 1,
        }

        public static string PayFormOwnCapital = "Vốn tự có";
        public static string PayFormLoan = "Vốn vay";
        public static string PayFormInvestMedGroup = "Đầu tư của MG";

        public enum CapitalPlanEnum
        {
            PayFormOwnCapital = 1,
            PayFormLoan = 2,
            PayFormInvestMedGroup = 3
        }

        public static string PrivateKey = "mED@2021CostEstimate09+-./#$@!#$";
        public static string Vector = "OFRna73m*aze01xY";
        public static readonly string CostStatusesElementItem = "request";
        public static readonly string CostStatusesElement = "estimate";
        public static readonly string ActuallySpent = "actually";
        public static readonly string Investment = "investment";
        public static readonly string CashFollow = "cashfollow";
        public static readonly string Revenue = "revenue";
        public static readonly string Profit = "profit";


        public static readonly string ObjectSub = "sub";
        public static readonly string ObjectUnit = "unit";

        public static readonly string Week = "week";
        public static readonly string Year = "year";


        public static readonly string UnitIn = "In";
        public static readonly string UnitOut = "Out";

        public enum PermissionEnum
        {
            View = 2,
            Approve = 4,
            Add = 8,
            Edit = 16,
            Delete = 32
        }

        public static readonly Dictionary<int, string> PermissionNames = new Dictionary<int, string> {
            {  (int)PermissionEnum.View, "Xem chi tiết" },
            {  (int)PermissionEnum.Approve, "Phê duyệt" },
            {  (int)PermissionEnum.Add, "Thêm mới" },
            {  (int)PermissionEnum.Edit, "Chỉnh sửa" },
            {  (int)PermissionEnum.Delete, "Xóa" },
        };

        public static readonly Dictionary<string, string> ObjectNames = new Dictionary<string, string> {
            {  ObjectSub, "SUB" },
            {  UnitIn, "Đơn vị y tế" },
            {  ObjectUnit, "Đơn vị thành viên" },
            { UnitOut, "Đơn vị ngoài y tế" }
        };

        public static readonly Dictionary<string, string> CostEstimateTypeNames = new Dictionary<string, string> {
            {  Week, "Tuần" },
            { Year, "Năm" }
        };

        public static readonly Dictionary<string, string> UnitTypeNames = new Dictionary<string, string> {
            {  UnitIn, "Đơn vị y tế" },  // officesub = YT
            { UnitOut, "Đơn vị ngoài y tế" } //
        };

        public static readonly Dictionary<string, string> TypeNames = new Dictionary<string, string> {
            {  CostStatusesElementItem, "Yêu cầu" },
            { CostStatusesElement, "Dự trù" },
            { ActuallySpent, "BC Thực chi" },
            { Investment, "Kế hoạch đầu tư" },
            { Revenue, "Kế hoạch doanh thu khách hàng" },
            { Profit, "Kế hoạch lợi nhuận" },
            { CashFollow, "BC tổng hợp dòng tiền" },
        };


        public enum StatusActuallyEnum
        {
            Done = 1, // đã chi hết
            NotDone = 2, //Đã chi nhưng chưa hết
            Wip = 0 // Chưa chi
        }

        public static readonly Dictionary<int, string> StatusActuallyNames = new Dictionary<int, string> {
            {  (int)StatusActuallyEnum.Done, "Đã chi" },
            { (int)StatusActuallyEnum.NotDone, "Chưa chi đủ" },
            { (int)StatusActuallyEnum.Wip, "Chưa chi" }
        };

        public static readonly Dictionary<int, string> DefaultStatusNames = new Dictionary<int, string> {
            { (int)StatusDefaultEnum.Active, "Đã duyệt" },
            { (int)StatusDefaultEnum.InActive, "Chờ duyệt" },
            { (int)StatusDefaultEnum.Decline, "Từ chối" },
            { (int)StatusDefaultEnum.Temporary, "Tạm" },
            { (int)StatusDefaultEnum.All, "Tất cả" },
        };

        public static readonly Dictionary<int, string> OtherStatusNames = new Dictionary<int, string> {
            { (int)StatusDefaultEnum.Active, "Kích hoạt" },
            { (int)StatusDefaultEnum.InActive, "Vô hiệu" },
            { (int)StatusDefaultEnum.Decline, "Từ chối" },
            { (int)StatusDefaultEnum.Temporary, "Tạm" },
            { (int)StatusDefaultEnum.All, "Tất cả" },
        };

        public static string GetStatusName(int status)
        {
            if (status == 0)
                return OtherStatusNames[(int) StatusDefaultEnum.InActive];
            
            if (status == 1)
                return OtherStatusNames[(int) StatusDefaultEnum.Active];
            
            return "";
        }

        public static bool ValidateTextSpeciallyFull(this string text)
        {
            text = text.ToLower();
            bool check = true;

            if (string.IsNullOrEmpty(text))
                return check;

            string[] unicodeChars = { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
                "đ",
                "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
                "í","ì","ỉ","ĩ","ị",
                "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
                "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
                "ý","ỳ","ỷ","ỹ","ỵ",
                "`","~","@", "!","#","$","%","^","&","*","(",")","-","+","=","_","\"","\\","/","{","}","[","]","|",":",";","'","<",">","?",".",",",};
            
            for (int i = 0; i < unicodeChars.Length; i++)
            {
                var item = unicodeChars[i];

                if(text.Contains(item))
                {
                    check = false;
                    break;
                }
            }
            return check;
        }
        public static bool ValidateTextSpecially(this string text)
        {
            text = text.ToLower();
            bool check = true;

            if (string.IsNullOrEmpty(text))
                return check;

            string[] unicodeChars = { ".", "'", "`", "~", "@", "!", "#", "$", "%", "^", "&", "*", "(", ")", "-", "+", "=", "_", "_", "\"", "\\", "/", "{", "}", "[", "]", "|", ":", ";", ",", "<", ">", "?" };

            for (int i = 0; i < unicodeChars.Length; i++)
            {
                var item = unicodeChars[i];

                if (text.Contains(item))
                {
                    check = false;
                    break;
                }
            }
            return check;
        }

        public static bool ValidateTextSpeciallyHDCTV(this string text)
        {
            text = text.ToLower();
            bool check = true;

            if (string.IsNullOrEmpty(text))
                return check;

            string[] unicodeChars = { ".", "`", "~", "@", "!", "#", "$", "%", "^", "&", "*", "(", ")", "+", "=", "_", "_", "\"", "\\", "/", "{", "}", "[", "]", "|", ":", ";", ",", "<", ">", "?" };

            for (int i = 0; i < unicodeChars.Length; i++)
            {
                var item = unicodeChars[i];

                if (text.Contains(item))
                {
                    check = false;
                    break;
                }
            }
            return check;
        }

        public static readonly Dictionary<int, string> DefaultTypeKeys = new Dictionary<int, string> {
            { (int)StatusDefaultType.Weekly, "week" },
            { (int)StatusDefaultType.Year, "year" }
        };
        public static readonly Dictionary<int, string> DefaultTypeNames = new Dictionary<int, string> {
            { (int)StatusDefaultType.Weekly, "Tuần" },
            { (int)StatusDefaultType.Year, "Năm" }
        };
        public static readonly Dictionary<string, int> DefaultKeyToTypes = new Dictionary<string, int> {
            { "week" ,(int)StatusDefaultType.Weekly },
            {  "year",(int)StatusDefaultType.Year }
        };

        public static readonly List<StatusesGranted> DefaultStatusSearchList = new List<StatusesGranted>
        {
            new StatusesGranted{Value = (int)StatusDefaultEnum.All,Name = DefaultStatusNames[(int)StatusDefaultEnum.All]},
            new StatusesGranted{Value = (int)StatusDefaultEnum.Active,Name = DefaultStatusNames[(int)StatusDefaultEnum.Active]},
            new StatusesGranted{Value = (int)StatusDefaultEnum.InActive,Name = DefaultStatusNames[(int)StatusDefaultEnum.InActive]},
            new StatusesGranted{Value = (int)StatusDefaultEnum.Decline,Name = DefaultStatusNames[(int)StatusDefaultEnum.Decline]}
        };


        /// <summary>
        /// Key mặc định của kế toán viên
        /// </summary>
        public const string DefaultCodeOfStaffAccountant = "StaffAccountant";
        /// <summary>
        /// Giám đốc đơn vị
        /// Khi user có order cao hơn giám đốc đơn vị
        /// thì bỏ qua điều kiện đơn vị
        /// </summary>
        public const string DefaultCodeOfUnitManager = "UnitManager";
        /// <summary>
        /// Cán bộ HC
        /// </summary>
        public const string DefaultCodeOfDepartmentStaff = "StaffDepartment";




        #region Phương án vốn
        public static readonly Dictionary<int, string> CapitalPlanValueToNamesDefaults = new Dictionary<int, string>
        {
            {(int)CapitalPlanEnum.PayFormOwnCapital,  PayFormOwnCapital},
            {(int)CapitalPlanEnum.PayFormLoan,  PayFormLoan},
            {(int)CapitalPlanEnum.PayFormInvestMedGroup,  PayFormInvestMedGroup},
        };

        public static readonly Dictionary<string, int> CapitalPlanNamesToValueDefaults = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            {  PayFormOwnCapital,(int)CapitalPlanEnum.PayFormOwnCapital},
            {  PayFormLoan,(int)CapitalPlanEnum.PayFormLoan},
            {  PayFormInvestMedGroup,(int)CapitalPlanEnum.PayFormInvestMedGroup},
        };

        #endregion
    }
}
