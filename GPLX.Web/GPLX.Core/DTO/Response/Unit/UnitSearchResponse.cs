using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.DTO.Response.Unit
{
    public class UnitSearchResponseData
    {
        public string OfficesName { get; set; }
        public string OfficesShortName { get; set; }
        public string OfficesCode { get; set; }
        public string OfficesDesc { get; set; }
        public int OfficesType { get; set; }
        public int OfficesOrder { get; set; }
        public string OfficesTypeName { get; set; }
        public string OfficesContact { get; set; }
        public string OfficesAddress { get; set; }
        public string ParrentID { get; set; }
        public string Createby { get; set; }
        public DateTime Createdate { get; set; }
        public string Updateby { get; set; }
        public DateTime? Updatedate { get; set; }
        public string OfficesGuid { get; set; }
        public string TenantName { get; set; }
        public int TenantID { get; set; }

        /// <summary>
        /// Loại đơn vị
        /// Sub hay đơn vị thành viên
        /// </summary>
        public string TypeName { get; set; }
    }

    public class UnitSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<UnitSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }
}
