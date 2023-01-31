using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.Data;
using GPLX.Core.DTO.Request.Profit;
using GPLX.Core.DTO.Response;
using GPLX.Core.DTO.Response.ProfitPlan;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.Profit
{
    public interface IProfitPlanRepository
    {
        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_30m, CacheKeyPrefix = CacheContant.PrefixProfitPlanContents)]

        Task<IList<ProfitPlanGroups>> ListGroups(string subject);
        Task<ProfitPlanCreateResponse> Create(ProfitPlanCreateRequest request);
        Task<SearchProfitPlanResponse> SearchAsync(SearchProfitPlanRequest request, int offset, int limit, string unitType);
        //Task<ProfitPlanApproveResponse> Approval(ProfitPlanApproveRequest request);
        Task<ProfitPlanApproveResponse> Approval(ProfitPlanApproveRequest request, ProfitPlan record, SignatureCheckResponse sigCheck);

        Task<SignatureCheckResponse> CheckPermissionApprove(
            ProfitPlanApproveRequest request, ProfitPlan record);

        Task<ProfitPlan> GetByIdAsync(Guid id);
        Task<IList<ProfitPlanViewHistoryResponse>> ViewHistories(ProfitPlanViewHistoryRequest request);
        Task<ProfitPlanApproveResponse> Delete(ProfitPlanApproveRequest request);
        Task<bool> AutoDecline(int financeYear, ProfitPlanApproveRequest declineModel, string positionAct);
    }
}
