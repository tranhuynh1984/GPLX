using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Request.DeXuatChiTiet;
using GPLX.Core.DTO.Response.DeXuatChiTiet;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.DeXuatChiTiet
{
    public interface IDeXuatChiTietRepository
    {
        Task<DeXuatChiTietSearchResponse> Search(DeXuatChiTietSearchRequest request);
        Task<DeXuatChiTietCreateResponse> Create(DeXuatChiTietCreateRequest request);
        Task<DeXuatChiTietCreateResponse> Remove(DeXuatChiTietCreateRequest request);
    }
}
