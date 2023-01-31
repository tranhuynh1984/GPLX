using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Request.DeXuatKhoaMaCTV;
using GPLX.Core.DTO.Response.DeXuatKhoaMaCTV;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.DeXuatKhoaMaCTV
{
    public interface IDeXuatKhoaMaCTVRepository
    {
        Task<DeXuatKhoaMaCTVSearchResponse> Search(DeXuatKhoaMaCTVSearchRequest request);
        Task<DeXuatKhoaMaCTVCreateResponse> Create(DeXuatKhoaMaCTVCreateRequest request);
        Task<DeXuatKhoaMaCTVCreateResponse> Remove(DeXuatKhoaMaCTVCreateRequest request);
    }
}
