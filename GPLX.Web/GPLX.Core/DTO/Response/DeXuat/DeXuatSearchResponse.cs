using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.DeXuat
{
    public class DeXuatSearchResponseData : UpdateTimeResponseData
    {
        public int Index { get; set; }
        public string DeXuatCode { get; set; }
        public string DeXuatName { get; set; }
        public string MaBacsi { get; set; }
        public string TenBacsi { get; set; }
        public string LoaiDeXuatCode { get; set; }
        public string TenDeXuatCode { get; set; }
        public int ProcessId { get; set; }
        public int ProcessStepId { get; set; }
        public string Note { get; set; }
        public int IsDone { get; set; }
        public DateTime? ThoiGianKhoa { get; set; }
        public string LyDoKhoa { get; set; }
        public string MaDonViCu { get; set; }
        public string MaDonViMoi { get; set; }
        public string MaDonViDeXuat { get; set; }
        public string TrangThai { get; set; }
        public int IDRole { get; set; }
        public string TenCongTy { get; set; }
    }

    public class DeXuatSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<DeXuatSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }
}
