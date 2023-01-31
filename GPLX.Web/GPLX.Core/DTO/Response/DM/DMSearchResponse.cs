using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.DM
{
    public class DMSearchResponseData : UpdateTimeResponseData
    {
        public int Index { get; set; }
        public int IDTree { get; set; }
        public string MaDM { get; set; }
        public string TenDM { get; set; }
        public int IsActive { get; set; }
        public string IsActiveName { get; set; }
        public int Stt { get; set; }
        public int MaxStt { get; set; }
    }

    public class DMSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<DMSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }

        public int MaxSTT { get; set; }
    }
}
