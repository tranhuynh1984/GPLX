using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Request.Process;
using GPLX.Core.DTO.Response.Process;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.Process
{
    public interface IProcessRepository
    {
        Task<ProcessSearchResponse> Search();
    }
}
