using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Request.ProcessStep;
using GPLX.Core.DTO.Response.ProcessStep;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.ProcessStep
{
    public interface IProcessStepRepository
    {
        Task<ProcessStepSearchResponse> Search();
        Task<ProcessStepSearchResponse> SearchStep(int processId, string dexuat ="");
        Task<List<ProcessStepDetailInfo>> GetProcessStepDetail(int processId);
    }
}
