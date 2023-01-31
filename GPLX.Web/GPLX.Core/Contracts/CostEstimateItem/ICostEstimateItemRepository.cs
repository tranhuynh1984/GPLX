using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GPLX.Core.DTO.Request.CostEstimateItem;
using GPLX.Core.DTO.Response.CostEstimateItem;

namespace GPLX.Core.Contracts.CostEstimateItem
{
    public interface ICostEstimateItemRepository
    {
        Task<Database.Models.CostEstimateItem[]> GetByStatusAsync(int status);
        Task<Database.Models.CostEstimateItem> GetByIdAsync(Guid id);
        Task<Database.Models.CostEstimateItem> GetByCodeAsync(string requestCode, int unitId);
        Task<Database.Models.CostEstimateItem> CreateAsync(Database.Models.CostEstimateItem item);
        Task<CostEstimateCreateResponse> CreateBulk(CostEstimateItemBulkCreateRequest request);

        Task<CostEstimateUpdateResponse> UpdateAsync(Guid id, Database.Models.CostEstimateItem request);
        Task<CostEstimateItemSearchResponse> SearchAsync(CostEstimateItemSearchRequest search, int offset, int limit, string unitType);
        Task<CostEstimateItemApprovalResponse> ApprovalOrDecline(CostEstimateItemApprovalRequest request);

        Task<int> CreateCostEstimateItemCodeForUnit(int unitId);
        Task<Database.Models.CostEstimateItem[]> GetByCostEstimateIdAsync(Guid id); 
        Task<IList<Database.Models.CostEstimateItem>> GetCostEstimateItemNotSpent(int week, int unit, bool isSub, string unitType);

        Task<IList<Database.Models.CostEstimateItem>> GetByListRequestCode(IEnumerable<string> codes);

        Task<CostEstimateItemApprovalResponse> Delete(CostEstimateItemApprovalRequest request);
        Task<bool> CheckBillCodeExists(string billCode);
    }
}
