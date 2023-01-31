using AspNetCoreRateLimit;
using GPLX.Infrastructure.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GPLX.Infrastructure.Installers
{
    public  class RegisterRequestRateLimiter : IServiceRegistration
	{
		public void RegisterAppServices(IServiceCollection services, IConfiguration config)
		{
			// needed to load configuration from appsettings.json
			services.AddOptions();
			// needed to store rate limit counters and ip rules
			services.AddMemoryCache();

			//load general configuration from appsettings.json
			services.Configure<IpRateLimitOptions>(config.GetSection("IpRateLimiting"));

			// inject counter and rules stores
			//services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
			//services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
			services.AddInMemoryRateLimiting();

			// https://github.com/aspnet/Hosting/issues/793
			// the IHttpContextAccessor service is not registered by default.
			// the clientId/clientIp resolvers use it.
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			// configuration (resolvers, counter key builders)
			services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
		}
	}
}
