using GPLX.Core.DTO.Response.DeXuat;
using GPLX.Core.DTO.Response.DeXuatChiTiet;
using GPLX.Core.DTO.Response.DeXuatKhoaMaCTV;
using GPLX.Core.DTO.Response.DeXuatLuanChuyenMa;
using GPLX.Core.DTO.Response.DMCP;
using GPLX.Core.DTO.Response.DMCTV;
using GPLX.Core.DTO.Response.ProfileCK;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GPLX.Web.Models.Dashboard
{
    public class ProfileCKFilterModel
    {
        public DMCPSearchResponse DMCPSearchResponse { get; set; }
        public SelectList ListDMCP { get; set; }
        public ProfileCKDetailSearchResponse ProfileCKDetailSearchResponse { get; set; }
    }

    public class DeXuatTaoMaFilterModel
    {
        //public SelectList Branchs { get; set; }
        public SelectList ListCTV { get; set; }
        public SelectList ListDMCP { get; set; }
        public SelectList ListDMDV { get; set; }
        public DMCTVSearchResponse DMCTVSearchResponse { get; set; }
        public DeXuatSearchResponseData DeXuatSearchResponseData { get; set; }
        public int IDRole { get; set; }
        public string DonVi { get; set; }
        public string MaDeXuat { get; set; }
        public string TenDeXuat { get; set; }
        public int ProcessId { get; set; }

        //0: mặc định 1: đã done -1: đã từ chối 2: readonly 3: chờ duyệt
        public int Permission { get; set; }

        //0: Sửa mã CTV 1: Thêm mới CTV
        public int TypeCTV { get; set; }

        public DeXuatChiTietSearchResponse DeXuatChiTietSearchResponse { get; set; }
        public DeXuatKhoaMaCTVSearchResponse DeXuatKhoaMaCTVSearchResponse { get; set; }
        public DeXuatLuanChuyenMaSearchResponse DeXuatLuanChuyenMaSearchResponse { get; set; }
    }
}