using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Request.Users;
using GPLX.Core.DTO.Response.Users;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.User
{
    public interface IUserRepository
    {
        Task<IList<Users>> GetAllAsync(string name, int offset, int limit);
        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixUsersCache)]

        Task<bool> AddRangeAsync(UserSync[] users);
        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixUsersCache)]

        Task<Users> GetUserByIdAsync(int id);
        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixUsersCache)]

        Task<Users> GetUserByUserIdAsync(string userId);

        
        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixUserManagesCache)]

        Task<IList<UserUnitsManages>> GetUserUnitManages(int userId);


        Task<UsersSearchResponse> Search(int skip, int length, UsersSearchRequest request);
        Task<UsersSearchResponse> SearchAll(UsersSearchRequest request);

        Task<UsersConfigResponse> Configs(UsersConfigRequest request);
        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.UserConcurrently)]
        Task<IList<UserConcurrently>> GetUserConcurrently(int userId);
        Task<SwitchUnitResponse> SwitchUnit(SwitchUnitRequest request);

        Task<UserSignatureConfigResponse> ConfigSignature(UserSignatureConfigRequest rq);

        Task<int> GetProcessRoleByUserIdAsync(int userid);
    }
}
