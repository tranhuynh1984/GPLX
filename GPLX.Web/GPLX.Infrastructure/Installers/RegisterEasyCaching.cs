using EasyCaching.Core.Interceptor;
using GPLX.Infrastructure.Cachings;
using GPLX.Infrastructure.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GPLX.Infrastructure.Installers
{
    public class RegisterEasyCaching : IServiceRegistration
	{
		public void RegisterAppServices(IServiceCollection services, IConfiguration config)
		{
			services.AddEasyCaching(options =>
			{
				options.UseInMemory(opt =>
				{
					opt.EnableLogging = true;
				}, "m1");
			});
			services.TryAddSingleton<IEasyCachingKeyGenerator, CustomEasyCachingKeyGenerator>();
		}
	}
}
