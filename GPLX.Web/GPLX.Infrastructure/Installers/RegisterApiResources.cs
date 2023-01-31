using GPLX.Core.Contracts;
using GPLX.Core.Data;
using GPLX.Infrastructure.Configs;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Contracts;
using GPLX.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GPLX.Infrastructure.Installers
{
    public class RegisterApiResources : IServiceRegistration
    {
        public void RegisterAppServices(IServiceCollection services, IConfiguration config)
        {
            var policyConfigs = new HttpClientPolicyConfiguration();
            config.Bind("HttpClientPolicies", policyConfigs);

            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(policyConfigs.RetryTimeoutInSeconds));

            var retryPolicy = HttpPolicyExtensions
               .HandleTransientHttpError()
               .OrResult(r => r.StatusCode == HttpStatusCode.NotFound)
               .WaitAndRetryAsync(policyConfigs.RetryCount, _ => TimeSpan.FromMilliseconds(policyConfigs.RetryDelayInMs));

            var circuitBreakerPolicy = HttpPolicyExtensions
               .HandleTransientHttpError()
               .CircuitBreakerAsync(policyConfigs.MaxAttemptBeforeBreak, TimeSpan.FromSeconds(policyConfigs.BreakDurationInSeconds));

            var noOpPolicy = Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>();

            services.AddHttpClient<ICostEstimateItemTypeConnect, CostEstimateItemTypeConnect>(client =>
            {
                //client.BaseAddress = new Uri(config["ApiResourceBaseUrls:SampleApi"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(HttpContentMediaTypes.JSON));
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(policyConfigs.HandlerTimeoutInMinutes))
            .AddPolicyHandler(request => request.Method == HttpMethod.Get ? retryPolicy : noOpPolicy)
            .AddPolicyHandler(timeoutPolicy)
            .AddPolicyHandler(circuitBreakerPolicy);

            services.AddHttpClient<ICostEstimateItemDepartmentConnect, CostEstimateItemDepartmentConnect>(client =>
            {
                //client.BaseAddress = new Uri(config["ApiResourceBaseUrls:SampleApi"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(HttpContentMediaTypes.JSON));
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(policyConfigs.HandlerTimeoutInMinutes))
            .AddPolicyHandler(request => request.Method == HttpMethod.Get ? retryPolicy : noOpPolicy)
            .AddPolicyHandler(timeoutPolicy)
            .AddPolicyHandler(circuitBreakerPolicy);

            services.AddHttpClient<ISupplierConnect, SupplierConnect>(client =>
            {
                //client.BaseAddress = new Uri(config["ApiResourceBaseUrls:SampleApi"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(HttpContentMediaTypes.JSON));
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(policyConfigs.HandlerTimeoutInMinutes))
            .AddPolicyHandler(request => request.Method == HttpMethod.Get ? retryPolicy : noOpPolicy)
            .AddPolicyHandler(timeoutPolicy)
            .AddPolicyHandler(circuitBreakerPolicy);

            services.AddHttpClient<IMedAuthenticateConnect, MedAuthenticateConnect>(client =>
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(HttpContentMediaTypes.JSON));
            })
           .SetHandlerLifetime(TimeSpan.FromMinutes(policyConfigs.HandlerTimeoutInMinutes))
           .AddPolicyHandler(request => request.Method == HttpMethod.Get ? retryPolicy : noOpPolicy)
           .AddPolicyHandler(timeoutPolicy)
           .AddPolicyHandler(circuitBreakerPolicy);
        }
    }
}
