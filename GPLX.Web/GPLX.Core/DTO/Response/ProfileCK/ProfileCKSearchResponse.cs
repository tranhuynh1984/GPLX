using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.DTO.Response.DMBS_ChuyenKhoa;
using GPLX.Core.DTO.Response.DMCP;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.ProfileCK
{
    public class ProfileCKSearchResponseData : UpdateTimeResponseData
    {
        public int Index { get; set; }
        
        public int Id { get; set; }

        public string ProfileCKMa { get; set; }
        public string ProfileCKTen { get; set; }
        public string ChuyenKhoaMa { get; set; }
        public string ChuyenKhoaTen { get; set; }
        public string Note { get; set; }
        public int IsActive { get; set; }
        public string IsActiveName { get; set; }
    }

    public class ProfileCKSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<ProfileCKSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }

    public class ProfileCKDetailSearchResponse
    {   
        public string ProfileCKMa { get; set; }
        public string ChuyenKhoaMa { get; set; }

        public DMBS_ChuyenKhoaSearchResponse ListChuyenKhoa { get; set; }

        public ProfileCKSearchResponseData Data { get; set; }

    }
}
