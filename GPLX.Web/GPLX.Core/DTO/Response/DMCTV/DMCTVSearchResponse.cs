using System;
using System.Collections.Generic;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.DMCTV
{
    public class DMCTVSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<DMCTVSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }

    public class ProvinceRespone
    {
        public string ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
    }

    public class DistrictRespone
    {
        public string DistrictCode { get; set; }
        public string DistrictName { get; set; }
    }

    public class JobTitleRespone
    {
        public string JobTitleCode { get; set; }
        public string JobTitleName { get; set; }
    }

    public class PartnerObjectRespone
    {
        public string PartnerObjectCode { get; set; }
        public string PartnerObjectName { get; set; }
    }
    public class DiscountRespone
    {
        public string DiscountCode { get; set; }
        public string DiscountName { get; set; }
    }
}