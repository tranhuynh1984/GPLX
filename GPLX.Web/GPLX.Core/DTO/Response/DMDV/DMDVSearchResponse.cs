using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.DMDV
{
    public class DMDVSearchResponseData : UpdateTimeResponseData
    {
        public int Stt { get; set; }
        public string MaDV { get; set; }
        public string TenDV { get; set; }
        public int IsActive { get; set; }
        public string IsActiveName { get; set; }
        public int PhapNhanId { get; set; }
        public string PhapNhanName { get; set; }
        public string MaSAP { get; set; }
        public string MaDVThanhVien { get; set; }
        public string MaDVExSap { get; set; }
    }

    public class DMDVSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<DMDVSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }
}