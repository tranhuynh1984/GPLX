using GPLX.Core.DTO;
using Microsoft.Extensions.Configuration;

namespace GPLX.Core.Extensions
{
    public static class ConfigurationExtension
    {
        public static StorageConfig StorageConfig(this IConfiguration configuration)
        {
            return configuration?.GetSection("StorageConfig")?.Get<StorageConfig>();
        }
    }
}