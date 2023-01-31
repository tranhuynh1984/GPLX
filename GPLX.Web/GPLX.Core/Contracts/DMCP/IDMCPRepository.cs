using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Request.DMCP;
using GPLX.Core.DTO.Response.DMCP;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.DMCP
{
    public interface IDMCPRepository
    {
        //[EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixUnitCache)]

        //Task<Legal> GetByIdAsync(int id);
        //[EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixUnitCache)]

        //Task<Legal> GetByCodeAsync(string offCodes);

        //Task<IList<Legal>> GetAllAsync(string name, int offset, int limit);

        //[EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixUnitCache)]
        //Task<bool> AddRangeAsync(IList<Units> lst);

        Task<DMCPSearchResponse> Search(int skip, int length, DMCPSearchRequest request);

        Task<DMCPSearchResponse> SearchAll(DMCPSearchRequest request);

        //[EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixUnitCache)]

        //Task<string> GetUnitType(string code);
    }
}