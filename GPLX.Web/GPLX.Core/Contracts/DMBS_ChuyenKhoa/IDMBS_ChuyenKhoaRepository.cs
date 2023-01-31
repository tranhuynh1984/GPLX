using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Request.DMBS_ChuyenKhoa;
using GPLX.Core.DTO.Response.DMBS_ChuyenKhoa;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.DMBS_ChuyenKhoa
{
    public interface IDMBS_ChuyenKhoaRepository
    {
        Task<DMBS_ChuyenKhoaSearchResponse> Search(int skip, int length, DMBS_ChuyenKhoaSearchRequest request);
        Task<DMBS_ChuyenKhoaSearchResponse> SearchAll(DMBS_ChuyenKhoaSearchRequest request);

        Task<DMBS_ChuyenKhoaSearchResponseData> GetById(string ma);
        Task<DMBS_ChuyenKhoaCreateResponse> Create(DMBS_ChuyenKhoaCreateRequest request);
        Task<DMBS_ChuyenKhoaCreateResponse> Remove(DMBS_ChuyenKhoaCreateRequest request);
        Task<int> GetMaxStt();
    }
}
