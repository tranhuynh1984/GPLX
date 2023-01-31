using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Request.DMHuyen;
using GPLX.Core.DTO.Response.DMHuyen;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.DMHuyen
{
    public interface IDMHuyenRepository
    {
        Task<DMHuyenSearchResponse> Search(int skip, int length, DMHuyenSearchRequest request);
        Task<DMHuyenSearchResponse> SearchAll(DMHuyenSearchRequest request);

    }
}
