using System.Collections.Generic;
using System.Threading.Tasks;
using GPLX.Core.DTO.Request.HDKCB;
using GPLX.Core.DTO.Response.HDKCB;

namespace GPLX.Core.Contracts.HDKCB
{
    public interface IHDKCBRepository
    {
        Task<HDKCBSearchResponse> Search(int skip, int length, HDKCBSearchRequest request);
        Task<HDKCBSearchResponse> SearchAll(HDKCBSearchRequest request);
        Task<HDKCBDetailResponse> GetByIdAsync(int id);
        Task<DetailDVSearchResponse> GetByHDKCKService(int skip, int length, string keyword, int? draw, int idhd);
    }
}
