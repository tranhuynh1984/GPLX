using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GPLX.Core.DTO.Request.Dashboard;
using GPLX.Core.DTO.Request.Notify;
using GPLX.Core.DTO.Response.Dashboard;

namespace GPLX.Core.Contracts.Dashboard
{
    public interface IDashboardRepository
    {
        Task<DashboardListResponse> SearchAsync(int offset, int limit, DashboardListRequest request);
        Task<DashboardExportResponse> Export(List<DashboardExportRequest> rq, List<FileNPlanType> excelPaths, List<FileNPlanType> pdfPaths, List<int> listUnitIds, string localPath);

        Task<bool> SendCreateNotify(string planType, int unitId, string forSubject, int year);
        Task<bool> SendOnApproval(SendFormat sendOnApproval);
    }
}
