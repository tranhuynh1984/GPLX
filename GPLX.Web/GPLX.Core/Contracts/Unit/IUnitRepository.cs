using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Request.Unit;
using GPLX.Core.DTO.Response.Unit;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.Unit
{
    public interface IUnitRepository
    {
        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixUnitCache)]

        Task<Units> GetByIdAsync(int id);
        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixUnitCache)]

        Task<Units> GetByCodeAsync(string offCodes);

        Task<IList<Units>> GetAllAsync(string name, int offset, int limit);

        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixUnitCache)]
        Task<bool> AddRangeAsync(IList<Units> lst);

        Task<UnitSearchResponse> Search(int skip, int length, UnitSearchRequest request);

        Task<UnitSetTypeResponse> SetInOut(UnitSetTypeRequest request);

        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixUnitCache)]

        Task<string> GetUnitType(string code);

    }
}
