using EasyCaching.Core.Interceptor;
using GPLX.Core.DTO.Response;
using GPLX.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GPLX.Infrastructure.Contracts
{
    public interface ISupplierConnect
    {
        [EasyCachingAble(Expiration = CacheContant.EXPIRATION_5m)]
        Task<SupplierResponse[]> GetDataAsync(string endPoint);
    }
}
