using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Request.TBL_CTVGROUP;
using GPLX.Core.DTO.Response.TBL_CTVGROUP;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.TBL_CTVGROUP
{
    public interface ITBL_CTVGROUPRepository
    {
        Task<TBL_CTVGROUPSearchResponse> Search(int skip, int length, TBL_CTVGROUPSearchRequest request);
        Task<TBL_CTVGROUPSearchResponse> GetAllDM();
        Task<TBL_CTVGROUPSearchResponse> SearchAll(TBL_CTVGROUPSearchRequest request);
        Task<TBL_CTVGROUPSearchResponseData> GetById(int ctvGroupID);
    }
}
