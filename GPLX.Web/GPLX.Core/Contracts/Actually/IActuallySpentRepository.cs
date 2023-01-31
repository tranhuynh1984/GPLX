using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GPLX.Core.DTO.Request.Actually;
using GPLX.Core.DTO.Response.Actually;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.Actually
{
    public interface IActuallySpentRepository
    {
        public Task<SearchActuallySpentResponse> SearchAsync(SearchActuallySpentRequest request, int offset, int limit, string unitType);

        public Task<CreateActuallyResponse> CreateAsync(CreateActuallySpentRequest request);

        public Task<CreateActuallyResponse> EditAsync(CreateActuallySpentRequest request, string unitType);

        public Task<CreateActuallySpentRequest> GetByIdAsync(GetActuallySpentByIdRequest request);

        public Task<ActuallySpentApproveResponse> Approval(ActuallySpentApproveRequest request);

        public Task<ActuallySpent> GetActuallySpentApprovedByWeek(int week, int maxFollowStats);

        public Task<IList<ActuallySpent>> GetAllByRangeDate(int startWeek, int endWeek, int year, int unit, int maxFollowStats);

        public Task<IList<SctData>> GetAllSctDataByActuallyIds(IEnumerable<Guid> ids);
        public Task<IList<ActuallySpentItem>> GetAllSpentItemsByActuallyIds(IEnumerable<Guid> ids);

        public Task<IList<ActuallyLogResponse>> ViewHistories(ActuallyLogRequest record);
    }
}
