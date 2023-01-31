using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.DeXuatChiTiet
{
    public class DeXuatChiTietSearchResponseData : UpdateTimeResponseData
    {
        public int Index { get; set; }
        public string DeXuatCode { get; set; }
        public string DeXuatName { get; set; }
        public string MaBacsi { get; set; }
        public string LoaiDeXuatCode { get; set; }
        public int ProcessId { get; set; }
        public int ProcessStepId { get; set; }
    }

    public class DeXuatChiTietSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<DeXuatChiTietSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }

        public int MaxSTT { get; set; }
    }
}
