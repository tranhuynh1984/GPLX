using GPLX.Core.Extensions;
using System;

namespace GPLX.Core.DTO.Request.DeXuatGhiChu
{
    public class DeXuatGhiChuCreateRequest
    {
        public string Record { get; set; }
        public int Status { get; set; }
        public string DeXuatCode { get; set; }
        public int ProcessStepId { get; set; }
        public string Note { get; set; }
        public string RequestPage { get; set; }
        public string CreatorName { get; set; }
        public int Creator { get; set; }

        public string CreateByCode { get; set; }
        public string CreateByName { get; set; }
        public DateTime? CreateDate { get; set; }

    }
}
