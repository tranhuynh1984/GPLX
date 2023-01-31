using System.Collections.Generic;
using GPLX.Core.DTO.Response.DMBS_ChuyenKhoa;
using GPLX.Core.DTO.Response.DMCP;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GPLX.Web.Models.Dashboard
{
    public class DMDVFilterModel
    {
        public IList<SelectListItem> DMPN { get; set; }
    }

    public class DMHuyenFilterModel
    {
        public IList<SelectListItem> DMTinh { get; set; }
    }

    public class DMProfileCKFilterModel
    {
        //public IList<SelectListItem> DMChuyenKhoa { get; set; }

        public IList<DMBS_ChuyenKhoaSearchResponseData> DMChuyenKhoa { get; set; }
    }
}
