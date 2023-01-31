using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Request.HDCTV;
using GPLX.Core.DTO.Response.HDCTV;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.HDCTV
{
    public interface IHDCTVRepository
    {
        //[EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixUnitCache)]

        //Task<Legal> GetByIdAsync(int id);
        //[EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixUnitCache)]

        //Task<Legal> GetByCodeAsync(string offCodes);

        //Task<IList<Legal>> GetAllAsync(string name, int offset, int limit);

        //[EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixUnitCache)]
        //Task<bool> AddRangeAsync(IList<Units> lst);

        //Task<LegalSearchResponse> Search(int skip, int length, LegalSearchRequest request);


        //[EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixUnitCache)]

        //Task<string> GetUnitType(string code);

    }
}
