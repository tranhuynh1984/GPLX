using GPLX.Infrastructure.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace GPLX.Infrastructure.Extensions
{
    public static class ServiceRegistrationExtension
	{
		public static void AddServicesInAssembly(this IServiceCollection services, IConfiguration configuration, params Type[] types)
		{
			foreach (var item in types)
			{
				var appServices = item.Assembly.DefinedTypes
								.Where(x => typeof(IServiceRegistration)
								.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
								.Select(Activator.CreateInstance)
								.Cast<IServiceRegistration>().ToList();

				appServices.ForEach(svc => svc.RegisterAppServices(services, configuration));
			}
		}
	}
}
