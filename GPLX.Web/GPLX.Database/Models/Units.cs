using System;

namespace GPLX.Database.Models
{
    public class Units
    {
        public int Id { get; set; }
        public string OfficesName { get; set; }
        public string OfficesCode { get; set; }
        public string OfficesDesc { get; set; }
        public int OfficesType { get; set; }
        public int OfficesOrder { get; set; }
        public string OfficesTypeName { get; set; }
        public string OfficesContact { get; set; }
        public string OfficesAddress { get; set; }
        public int Status { get; set; }
        public string ParrentID { get; set; }
        public string Createby { get; set; }
        public DateTime Createdate { get; set; }
        public string Updateby { get; set; }
        public DateTime? Updatedate { get; set; }
        public string OfficesGuid { get; set; }
        public string TenantName { get; set; }
        public int TenantID { get; set; }
        /// <summary>
        /// Trường này bên API quy định là đơn vị thành viên hay ngoài đơn vị thành viên
        /// Không phải là 
        /// </summary>
        public string OfficesSub { get; set; }
        public string OfficesShortName { get; set; }
    }
}
