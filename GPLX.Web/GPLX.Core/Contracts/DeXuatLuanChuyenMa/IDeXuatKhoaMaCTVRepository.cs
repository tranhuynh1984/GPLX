using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Request.DeXuatLuanChuyenMa;
using GPLX.Core.DTO.Response.DeXuatLuanChuyenMa;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.DeXuatLuanChuyenMa
{
    public interface IDeXuatLuanChuyenMaRepository
    {
        Task<DeXuatLuanChuyenMaSearchResponse> Search(DeXuatLuanChuyenMaSearchRequest request);
        Task<DeXuatLuanChuyenMaCreateResponse> Create(DeXuatLuanChuyenMaCreateRequest request);
        Task<DeXuatLuanChuyenMaCreateResponse> Remove(DeXuatLuanChuyenMaCreateRequest request);
    }
}
