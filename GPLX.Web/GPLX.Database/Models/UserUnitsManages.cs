using System;

namespace GPLX.Database.Models
{
    /// <summary>
    /// Đơn vị user đc gán quản lý
    /// </summary>
    public class UserUnitsManages
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string  OfficeCode { get; set; }
        public int OfficeId { get; set; }
    }
}
