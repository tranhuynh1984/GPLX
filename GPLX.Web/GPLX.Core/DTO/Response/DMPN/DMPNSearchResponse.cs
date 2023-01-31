using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.DMPN
{
    public class DMPNSearchResponseData : UpdateTimeResponseData
    {
        public int Stt { get; set; }
        public int PhapNhanId { get; set; }
        public string PhapNhanName { get; set; }
        public int IsActive { get; set; }
        public string IsActiveName { get; set; }
        public string CompanyName { get; set; }
        public string TaxNumber { get; set; }
        public string AddressCompany { get; set; }
    }

    public class DMPNSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<DMPNSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }
}
