using System;

namespace GPLX.Core.DTO.Request.DeXuatGhiChu
{
    public class DeXuatGhiChuSearchRequest
    {
        public int Status { get; set; }
        public int Draw { get; set; }
        public string DeXuatCode { get; set; }
        public int ProcessStepId { get; set; }
        public string Note { get; set; }
        public string Keywords { get; set; }
        public string RequestPage { get; set; }
    }
}
