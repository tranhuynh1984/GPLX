using System.Collections.Generic;

using GPLX.Core.DTO.Response.DMDV;
using GPLX.Core.DTO.Response.NhCP;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GPLX.Web.Models.Dashboard
{
    public class DMCPFilterModel
    {
        // Danh sách đơn vị có bảng giá riêng
        public SelectList Branchs { get; set; }

        // Danh sách nhóm dịch vụ
        public SelectList Categories { get; set; }
    }
}