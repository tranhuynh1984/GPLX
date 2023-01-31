using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.DeXuatLuanChuyenMa
{
    public class DeXuatLuanChuyenMaSearchResponseData : UpdateTimeResponseData
    {
        public int Index { get; set; }
        public string DeXuatCode { get; set; }
        public string MaCTV { get; set; }
        public string TenCTV { get; set; }
        public DateTime ThoiGianKhoa { get; set; }
        public string ThoiGianKhoaString { get; set; }
        public string Note { get; set; }
        public int ProcessId { get; set; }
        public int ProcessStepId { get; set; }
    }

    public class DeXuatLuanChuyenMaSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<DeXuatLuanChuyenMaSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }

        public int MaxSTT { get; set; }
    }
}
