using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPLX.Infrastructure.Extensions
{
    public static class HostBuilderLogExtensions
    {
		public static string DefaultSectionName => "Serilog";

		private static readonly string[] ProjectNameIgnores = new string[]
		{
			"Microsoft.Extensions.Logging",
			"System.Private.CoreLib"
		};

		private static IHostBuilder UseLog(this IHostBuilder builder
			, string projectName
			, Action<LoggerConfiguration> action)
		{
			builder.UseSerilog((hostContext, cfg) =>
			{
				cfg.ReadFrom.Configuration(hostContext.Configuration);

				cfg.Enrich.FromLogContext()
				.Enrich.WithMachineName()
				.Enrich.WithEnvironmentUserName()
				.Enrich.WithThreadId()
				.Enrich.WithThreadName()
				.Enrich.WithProcessId()
				.Enrich.WithProcessName();
				if (string.IsNullOrWhiteSpace(projectName))
					projectName = GetProjectName();
				if (!string.IsNullOrWhiteSpace(projectName))
					cfg.Enrich.WithProperty("ProjectName", projectName);
				if (action != null)
					action.Invoke(cfg);
			});
			return builder;
		}

		private static string GetProjectName()
		{
			string projectName = null;
			projectName = System.Reflection.Assembly.GetCallingAssembly().GetName().Name;
			if (!ProjectNameIgnores.Any(m => m.Equals(projectName)))
				return projectName;

			projectName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
			if (!ProjectNameIgnores.Any(m => m.Equals(projectName)))
				return projectName;
			projectName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
			if (!ProjectNameIgnores.Any(m => m.Equals(projectName)))
				return projectName;
			return null;
		}
		public static IHostBuilder UseLog(this IHostBuilder builder)
		{
			return builder.UseLog(projectName: null
				, action: null);
		}

		public static IHostBuilder UseLog(this IHostBuilder builder, Action<LoggerConfiguration> action)
		{
			return builder.UseLog(projectName: null
				, action: action);
		}
	}
}
