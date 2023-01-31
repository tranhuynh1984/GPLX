using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Request.TBL_CTVGROUPSUB1_DETAIL;
using GPLX.Core.DTO.Response.HDCTV;
using GPLX.Core.DTO.Response.TBL_CTVGROUPSUB1_DETAIL;
using GPLX.Core.Enum;
using GPLX.Database.Models;

namespace GPLX.Core.Contracts.TBL_CTVGROUPSUB1_DETAIL
{
    public interface ITBL_CTVGROUPSUB1_DETAILRepository
    {
        Task<TBL_CTVGROUPSUB1_DETAILSearchResponse> Search(int skip, int length, TBL_CTVGROUPSUB1_DETAILSearchRequest request);
        Task<TBL_CTVGROUPSUB1_DETAILSearchResponse> SearchAll(TBL_CTVGROUPSUB1_DETAILSearchRequest request);
        Task<TBL_CTVGROUPSUB1_DETAILSearchResponseData> GetById(int subId, string maCP);
        Task<TBL_CTVGROUPSUB1_DETAILCreateResponse> Create(TBL_CTVGROUPSUB1_DETAILCreateRequest request);
        Task<TBL_CTVGROUPSUB1_DETAILCreateResponse> Remove(TBL_CTVGROUPSUB1_DETAILCreateRequest request);
        Task<ImportToDBResponse> CreateRange(HdctvImportExcel<HdctvType1UploadResponse> request);
        Task<List<TBL_CTVGROUPSUB1_DETAILSearchResponseData>> GetAllBySubId(int subId);
    }
}
