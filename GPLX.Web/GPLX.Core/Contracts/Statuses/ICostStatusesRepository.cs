using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Request;
using GPLX.Core.DTO.Request.CostStatus;
using GPLX.Core.DTO.Response.CostStatus;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.Statuses
{
    public interface ICostStatusesRepository
    {
        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixStatusesForSubjectCache)]
        Task<IList<StatusesGranted>> ListStatusesForSubject(DataSeenRequest request);
        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixStatusesCache)]

        Task<CostStatusCreateResponse> AddAsync(CostStatusCreateRequest statuses);
        Task<CostStatusSearchResponse> Search(int skip, int length, CostStatusSearchRequest request);

        Task<CostStatusCreateResponse> ChangeStatus(CostStatusCreateRequest statuses);
        Task<CostStatuses> GetById(Guid id);

        Task<IList<Database.Models.Groups>> GetGrantStatuses(Guid id);
        //todo: cached
        
        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixStatusesUsedCache)]
        Task<Database.Models.Groups> GetUsedByGroup(Guid id);
        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixStatusesForSubjectCache)]

        Task<CostStatusesGrantResponse> SetGrant(CostStatusesGrantRequest request);
        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_10m, CacheKeyPrefix = CacheContant.PrefixStatusesCache)]
        Task<IList<CostStatuses>> GetAll();
        Task<IList<CostStatusFollowResponse>> GetFollow(GetFollowRequest request);

        Task<IList<SpecialUnitFollowConfigs>> GetSpecialUnitFollowConfigs();

        Task<string> ValidateSignerPositionInExcel(Aspose.Cells.Worksheet ws, string type,string time, string subject, int unitId);
    }
}
