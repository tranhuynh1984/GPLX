using System;

namespace GPLX.Database.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public string UserPhone { get; set; }
        public string UserEmail { get; set; }
        public string UserImage { get; set; }
        public string Password { get; set; }
        public int Status { get; set; }
        public string Createby { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string AccountGuid { get; set; }
        public string DigitalSignature { get; set; }
        public string Salt { get; set; }

        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        // public int? GroupId { get; set; }
        // public string GroupName { get; set; }
        // public string GroupCode { get; set; }



        public int? UnitId { get; set; }
        public string UnitName { get; set; }
        /// <summary>
        /// Đường dẫn file chữ ký
        /// </summary>
        public string PathSignature { get; set; }
        /// <summary>
        /// Tài khoản ký số
        /// </summary>
        public string AccDigitalSignature { get; set; }
        /// <summary>
        /// Mật khẩu ký số
        /// Mã hóa
        /// </summary>
        public string PasswordDigitalSignature { get; set; }
    }
}
