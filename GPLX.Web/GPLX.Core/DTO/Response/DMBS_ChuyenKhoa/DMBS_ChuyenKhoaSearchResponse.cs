using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.DMBS_ChuyenKhoa
{
    public class DMBS_ChuyenKhoaSearchResponseData : UpdateTimeResponseData
    {
        public int Index { get; set; }
        public int Stt { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public int IsActive { get; set; }
        public string IsActiveName { get; set; }
        public int MaxStt { get; set; }
    }

    public class DMBS_ChuyenKhoaSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<DMBS_ChuyenKhoaSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }
}
