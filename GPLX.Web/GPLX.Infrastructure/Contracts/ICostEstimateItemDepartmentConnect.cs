using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Response;
using GPLX.Core.Enum;
using System.Threading.Tasks;
using GPLX.Core.DTO.Response.CostEstimateItem;

namespace GPLX.Infrastructure.Contracts
{
    public interface ICostEstimateItemDepartmentConnect
    {
        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_5m)]
        Task<CostEstimateItemDepartmentResponse[]> GetDataAsync(string endPoint);
    }
}
