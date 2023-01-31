using System;

namespace GPLX.Core.DTO.Request.HDKCB
{
    public class HDKCBSearchRequest
    {
        public int? Status { get; set; }
        public int Draw { get; set; }
        public int? IDHD { get; set; }
        //Mã loại HĐ
        public string MaHD { get; set; }
        //Tên hợp đồng
        public string TenHD { get; set; }
        //Ngày bắt đầu hợp đồng
        public DateTime? ND { get; set; }
        //Ngày kết thúc hợp đồng
        public DateTime? NS { get; set; }
        //Tên hợp đồng tìm nhanh
        public string TenHDTimNhanh { get; set; }
    }
}
