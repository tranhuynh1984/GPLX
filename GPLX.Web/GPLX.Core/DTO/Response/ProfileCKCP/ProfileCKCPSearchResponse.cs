using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.DTO.Response.DMCP;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.ProfileCKCP
{
    public class ProfileCKCPSearchResponseData : UpdateTimeResponseData
    {
        public int Index { get; set; }
        
        public int Id { get; set; }

        public string ProfileCKMa { get; set; }
        public string CPMa { get; set; }
        public string CPTen { get; set; }
        public int IsActive { get; set; }
        public string IsActiveName { get; set; }
    }

    public class ProfileCKCPSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<ProfileCKCPSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }
}
