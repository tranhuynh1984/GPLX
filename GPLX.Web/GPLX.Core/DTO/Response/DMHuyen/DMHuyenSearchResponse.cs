using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.DMHuyen
{
    public class DMHuyenSearchResponseData : UpdateTimeResponseData
    {
        public int Stt { get; set; }
        public string MaHuyen { get; set; }
        public string TenHuyen { get; set; }
        public string MaTinh { get; set; }
        public string TenTinh { get; set; }
        public int IsActive { get; set; }
        public string IsActiveName { get; set; }
    }

    public class DMHuyenSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<DMHuyenSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }
}
