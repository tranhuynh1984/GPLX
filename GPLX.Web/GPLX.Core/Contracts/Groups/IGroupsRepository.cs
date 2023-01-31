using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Entities;
using GPLX.Core.DTO.Request.Groups;
using GPLX.Core.DTO.Response.Groups;
using GPLX.Core.DTO.Response.Permission;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.Groups
{
    public interface IGroupsRepository
    {
        Task<GroupsCreateResponse> DeleteAsync(GroupsCreateRequest request);
        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_5m, CacheKeyPrefix = CacheContant.PrefixAuthorizeCache)]

        Task<bool> IsAuthorize(int groups, string module, int permission);
        Task<bool> IsAuthorize(List<int> groups, string module, int permission);
        Task<Database.Models.Groups> GetByIdAsync(int groups);
        Task<GroupsSearchResponseData> GetByIdAsyncView(int groups);
        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixPermissionCache)]

        Task<bool> EditPermission(int groupId, int permission, int functionId);

        Task<GroupsSearchResponse> SearchAsync(int length, int size, GroupsSearchRequest request);

        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_5m, CacheKeyPrefix = CacheContant.PrefixFunctionsCache)]

        Task<IList<Functions>> GetFunctionsAsync();
        Task<IList<GroupsPermission>> GetPermissionAsync(int groupId);

        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_5m, CacheKeyPrefix = CacheContant.PrefixGroupCache)]

        Task<GroupsCreateResponse> Create(GroupsCreateRequest request);
        Task<GroupsCreateResponse> ChangeStatus(GroupsCreateRequest request);

        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixGroupCache)]
        Task<IList<Database.Models.Groups>> GetAllActiveGroups();

        bool IsAuthorize(int groups, string module, int permission, IList<GroupsPermission> allGroups,
            IList<Functions> allFunctions);
        bool IsAuthorize(List<int> groups, string module, int permission, IList<GroupsPermission> allGroups,
            IList<Functions> allFunctions);

        bool IsAuthorizePath(int groups, int fcId, int permission, IList<GroupsPermission> allGroups,
            IList<Functions> allFunctions);
        bool IsAuthorizePath(List<int> groups, int fcId, int permission, IList<GroupsPermission> allGroups,
            IList<Functions> allFunctions);

        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixPermissionCache)]
        Task<IList<GroupsPermission>> GetAllPermission();

        Task<IList<PermissionGrantResponse>> GetGrants(int groupId);

        Task<bool> DeleteFuncInPermission(List<PermissionUpdate> updates,int g);
        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixGroupByCodeCache)]

        Task<Database.Models.Groups> GetByCode(string code);

        Task<IList<Database.Models.Groups>> GetGroupsOurUser(int user);
    }
}
