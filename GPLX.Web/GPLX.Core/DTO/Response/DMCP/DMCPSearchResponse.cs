using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.DMCP
{
    public class DMCPSearchResponseData : UpdateTimeResponseData
    {
        [JsonPropertyName("Index")]
        public int Index { get; set; }

        [JsonPropertyName("MaCP")]
        public string MaCP { get; set; }

        [JsonPropertyName("MaNN")]
        public string MaNN { get; set; }

        [JsonPropertyName("TenCP")]
        public string TenCP { get; set; }

        [JsonPropertyName("TenCPE")]
        public string TenCPE { get; set; }

        [JsonPropertyName("DVT")]
        public string DVT { get; set; }

        [JsonPropertyName("DG")]
        public float DG { get; set; }

        [JsonPropertyName("DGBH")]
        public float DGBH { get; set; }

        [JsonPropertyName("IsActive")]
        public int IsActive { get; set; }

        [JsonPropertyName("IsActiveName")]
        public string IsActiveName { get; set; }

        [JsonPropertyName("MaNhCP")]
        public string MaNhCP { get; set; }

        [JsonPropertyName("MaLoaiKT1")]
        public string MaLoaiKT1 { get; set; }

        [JsonPropertyName("MaLoaiKT2")]
        public string MaLoaiKT2 { get; set; }

        [JsonPropertyName("MaLoaiKT3")]
        public string MaLoaiKT3 { get; set; }

        [JsonPropertyName("TenRutGon")]
        public string TenRutGon { get; set; }

        [JsonPropertyName("KhoaGGTrucTiep")]
        public int KhoaGGTrucTiep { get; set; }

        [JsonPropertyName("BranchCode")]
        public string BranchCode { get; set; }
    }

    public class DMCPSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<DMCPSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }
}