using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.HDKCB
{
    public class HDKCBSearchResponseData : UpdateTimeResponseData
    {
        public int Stt { get; set; }
        public int IDHD { get; set; }

        public string MaLoai { get; set; }
        public string MaHD { get; set; }
        public string TenHD { get; set; }
        public DateTime Ngay { get; set; }
        public int Del { get; set; }
        public int IsActive { get; set; }
        public string IsActiveName { get; set; }
        public DateTime ND { get; set; }
        public DateTime NS { get; set; }
        public string NDString { get; set; }
        public string NSString { get; set; }
    }

    public class HDKCBSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<HDKCBSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }
}
