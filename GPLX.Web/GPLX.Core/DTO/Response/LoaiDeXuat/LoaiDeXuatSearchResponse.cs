using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.LoaiDeXuat
{
    public class LoaiDeXuatSearchResponseData : UpdateTimeResponseData
    {
        public int Index { get; set; }
        public string LoaiDeXuatCode { get; set; }
        public string LoaiDeXuatName { get; set; }
        public int IsActive { get; set; }
        public string IsActiveName { get; set; }
        public int Stt { get; set; }
        public int MaxStt { get; set; }
    }

    public class LoaiDeXuatSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<LoaiDeXuatSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }
}
