using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.Data;
using GPLX.Core.DTO.Request.RevenuePlan;
using GPLX.Core.DTO.Response;
using GPLX.Core.DTO.Response.RevenuePlan;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.Revenue
{
    public interface IRevenuePlanRepository
    {
        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_30m, CacheKeyPrefix = CacheContant.PrefixRevenuePlanContents)]
        Task<IList<RevenuePlanContents>> ListRevenuePlanContents(string unitType);

        Task<RevenuePlanCreateResponse> Create(RevenuePlanCreateRequest request);
        Task<SearchRevenuePlanResponse> SearchAsync(SearchRevenuePlanRequest request, int offset, int limit, string unitType);
        //Task<RevenuePlanApproveResponse> Approval(RevenuePlanApproveRequest request);
        Task<RevenuePlanApproveResponse> Approval(RevenuePlanApproveRequest request, RevenuePlan record, SignatureCheckResponse sigCheck);

        Task<SignatureCheckResponse> CheckPermissionApprove(
            RevenuePlanApproveRequest request, RevenuePlan record);

        Task<RevenuePlan> GetByIdAsync(Guid id);
        Task<IList<RevenuePlanViewHistoryResponse>> ViewHistories(RevenuePlanViewHistoryRequest request);
        Task<RevenuePlanApproveResponse> Delete(RevenuePlanApproveRequest request);

        Task<IList<RevenuePlanCashDetails>> GetLatestUnitRevenuePlanCashDetails(int year, int unitId);
    }
}
