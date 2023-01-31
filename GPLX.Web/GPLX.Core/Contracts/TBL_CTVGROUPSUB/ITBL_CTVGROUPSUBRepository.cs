using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Request.TBL_CTVGROUPSUB;
using GPLX.Core.DTO.Response.TBL_CTVGROUPSUB;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.TBL_CTVGROUPSUB
{
    public interface ITBL_CTVGROUPSUBRepository
    {
        Task<TBL_CTVGROUPSUBSearchResponse> Search(int skip, int length, TBL_CTVGROUPSUBSearchRequest request);
        Task<TBL_CTVGROUPSUBSearchResponse> GetAllDM();
        Task<TBL_CTVGROUPSUBSearchResponse> SearchAll(TBL_CTVGROUPSUBSearchRequest request);
        Task<TBL_CTVGROUPSUBSearchResponseData> GetById(int subId);
        Task<TBL_CTVGROUPSUBCreateResponse> Create(TBL_CTVGROUPSUBCreateRequest request);
        Task<TBL_CTVGROUPSUBCreateResponse> Remove(TBL_CTVGROUPSUBCreateRequest request);
    }
}
