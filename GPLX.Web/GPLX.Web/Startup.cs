using AspNetCoreRateLimit;
using FluentValidation.AspNetCore;
using GPLX.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Hangfire;
using Hangfire.SqlServer;
using System;
using System.IO;
using Hangfire.Dashboard;
using GPLX.Core.Contracts.CostEstimate;
using GPLX.Core.Data.CostEstimate;
using GPLX.Infrastructure.Configs;
using GPLX.Database;
using Microsoft.EntityFrameworkCore;
using GPLX.Infrastructure.Installers;
using GPLX.Core.DTO.Request.CostEstimateItem;
using GPLX.Web.Middleware;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace GPLX.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddServicesInAssembly(Configuration, typeof(Startup), typeof(RegisterContractMappings));

            services.AddFluentValidation(fv =>
            {
                fv.DisableDataAnnotationsValidation = false;
                fv.ImplicitlyValidateChildProperties = true;
                fv.RegisterValidatorsFromAssemblies(new[] {
                    Assembly.GetAssembly(typeof(Startup)),
                    Assembly.GetAssembly(typeof(CostEstimateItemCreateRequest))
                });
            });

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            //dbcontext
            services.AddDbContext<Context>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DbConnection"),
                        b => b.MigrationsAssembly("GPLX.Database"))
                    .LogTo(Console.WriteLine, LogLevel.Information);
            });
            services.AddMvc(opts => { opts.MaxModelBindingCollectionSize = int.MaxValue; });
            //RecurringJob.AddOrUpdate(() => Console.WriteLine("Minutely Job executed"), Cron.Minutely); //Cron.Daily
            // Add the processing server as IHostedService
            services.AddHangfireServer();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = new PathString("/Account/Login");
                options.ExpireTimeSpan = TimeSpan.FromHours(6);
            });

            services.AddControllersWithViews();

            services.AddAutoMapper(typeof(MappingProfileConfiguration));

            //string licPath = Configuration.GetValue<string>("Aspose:License");
            //License lic = new License();
            //lic.SetLicense(licPath);
            services.Configure<FormOptions>(ops =>
            {
                ops.ValueCountLimit = int.MaxValue;
                ops.ValueLengthLimit = int.MaxValue;
            });
            services.AddTransient<ICostEstimateRepository, CostEstimateRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            app.UseExceptionHandler("/Error/500");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            // }

            var options = new RewriteOptions()
                .AddRedirect("^$", "index.html");
            app.UseRewriter(options);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseHangfireDashboard("/hf-server", new DashboardOptions
            {
                IsReadOnlyFunc = (DashboardContext context) => true
            });

            //Enable AspNetCoreRateLimit
            if (env.IsProduction())
                app.UseIpRateLimiting();

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict
            });

            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.WebRootPath, "metadata")),
                RequestPath = "/metadata",
                // EnableDirectoryBrowsing = true
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHangfireDashboard();
            });
            var manager = new RecurringJobManager();
            string jobId = "_Delete_MaxTime_EstimateItem";
            manager.AddOrUpdate<ICostEstimateRepository>(jobId, (ICostEstimateRepository mc) => mc.DeleteUnUsed(), Cron.Daily(0, 0));//Cron.Daily(0,0)
            // CheckUnitNotCreateYet
            // CheckUnitNotApproveYet
        }
    }
    public class ContainerJobActivator : JobActivator
    {
        private readonly IServiceProvider _container;

        public ContainerJobActivator(IServiceProvider serviceProvider)
        {
            _container = serviceProvider;
        }

        public override object ActivateJob(Type type)
        {
            return _container.GetService(type);
        }
    }
}
