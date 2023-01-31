using System;
using System.Collections.Generic;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.HDKCB
{
    public class HDKCBDetailResponse: UpdateTimeResponseData
    {
        public int Stt { get; set; }
        public int IDHD { get; set; }

        public string MaLoai { get; set; }
        public string MaHD { get; set; }
        public string TenHD { get; set; }
        public string Ngay { get; set; }
        public int Del { get; set; }
        public int IsActive { get; set; }
        public string IsActiveName { get; set; }
        public string ND { get; set; }
        public string NS { get; set; }
    }
    
    public class DetailDVSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<DetailDVResponse> Data { get; set; }
        public decimal TotalPrice { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }

    public class DetailDVResponse
    {
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public decimal Amount { get; set; }
        public string Unit { get; set; }
        public float ServicePrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public bool Active { get; set; }
    }
}