using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GPLX.Infrastructure.Contracts
{
    public interface IServiceRegistration
    {
        void RegisterAppServices(IServiceCollection services, IConfiguration configuration);
    }
}
