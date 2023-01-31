using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Request.LoaiDeXuat;
using GPLX.Core.DTO.Response.LoaiDeXuat;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.LoaiDeXuat
{
    public interface ILoaiDeXuatRepository
    {
        Task<LoaiDeXuatSearchResponse> Search(int skip, int length, LoaiDeXuatSearchRequest request);
        Task<LoaiDeXuatSearchResponse> GetAllLoaiDeXuat();
        Task<LoaiDeXuatSearchResponse> SearchAll(LoaiDeXuatSearchRequest request);
        Task<LoaiDeXuatSearchResponseData> GetById(string loaiDeXuatCode);
        Task<LoaiDeXuatCreateResponse> Create(LoaiDeXuatCreateRequest request);
        Task<LoaiDeXuatCreateResponse> Remove(LoaiDeXuatCreateRequest request);
        Task<int> GetMaxStt();
    }
}
