using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Request.DM;
using GPLX.Core.DTO.Response.DM;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.DM
{
    public interface IDMRepository
    {
        Task<DMSearchResponse> Search(int skip, int length, DMSearchRequest request);
        Task<DMSearchResponse> GetAllDM(int idTree);
        Task<DMSearchResponse> SearchAll(DMSearchRequest request);
        Task<DMSearchResponseData> GetById(int idTree, string maDM);
        Task<DMCreateResponse> Create(DMCreateRequest request);
        Task<DMCreateResponse> Remove(DMCreateRequest request);
        Task<int> GetMaxStt(int idTree);
    }
}
