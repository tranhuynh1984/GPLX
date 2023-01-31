using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Request.DMDV;
using GPLX.Core.DTO.Response.DMDV;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.DMDV
{
    public interface IDMDVRepository
    {
        Task<DMDVSearchResponse> Search(int skip, int length, DMDVSearchRequest request);

        Task<DMDVSearchResponse> SearchAll(DMDVSearchRequest request);

        Task<List<DMDVSearchResponseData>> GetCategory();
    }
}