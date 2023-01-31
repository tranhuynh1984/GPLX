using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Response;
using GPLX.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GPLX.Core.DTO.Response.CostEstimateItem;

namespace GPLX.Infrastructure.Contracts
{
    public interface ICostEstimateItemTypeConnect
    {
        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_5m)]
        Task<IList<CostEstimateItemTypeResponse>> GetDataAsync(string endPoint);
    }
}
