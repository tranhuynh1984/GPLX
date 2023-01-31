using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GPLX.Core.DTO.Response.NhCP
{
    /// <summary>
    /// Nhóm dịch vụ
    /// </summary>
    public class NhCPSearchResponseData
    {
        [JsonPropertyName("MaNhCP")]
        public string MaNhCP { get; set; }

        [JsonPropertyName("TenNhCP")]
        public string TenNhCP { get; set; }

        [JsonPropertyName("TenNhCPFull")]
        public string TenNhCPFull { get; set; }
    }

    public class NhCPSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<NhCPSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }
}