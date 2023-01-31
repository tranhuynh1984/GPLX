using System;
using GPLX.Core.Contracts;
using GPLX.Core.DTO.Request;
using GPLX.Core.DTO.Response;
using GPLX.Database.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GPLX.Core.DTO.Response.Users;
using Serilog;

namespace GPLX.Core.Data
{
    public class MedAuthenticateConnect : IMedAuthenticateConnect
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MedAuthenticateConnect> _logger;
        public MedAuthenticateConnect(HttpClient httpClient, ILogger<MedAuthenticateConnect> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> GetAccessTokenAsync(string endPoint, MedAccesstokenRequest request)
        {
            try
            {
                var client = new RestClient(endPoint);

                var rq = new RestRequest(Method.POST);
                rq.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                rq.AddParameter("grant_type", request.grant_type);
                rq.AddParameter("client_id", request.client_id);
                rq.AddParameter("scope", request.scope);
                rq.AddParameter("client_secret", request.client_secret);

                IRestResponse response = await client.ExecuteAsync(rq);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Log.Information($"[{response.StatusCode}] An error occured while requesting external api.");
                    return default;
                }

                var data = JObject.Parse(response.Content);
                if (data?.SelectToken("access_token")?.ToObject<string>() != null)
                    return data.SelectToken("access_token")?.ToObject<string>();

                Log.Error("data: {0}", response.Content);
                return null;
            }
            catch (Exception e)
            {
               Log.Error(e, "Error");
               return null;
            }
        }

        public async Task<MedApiResponse<Units>> GetUnitsAsync(string endPoint, string accesstoken)
        {

            var client = new RestClient(endPoint);

            var rq = new RestRequest(Method.GET);
            rq.AddHeader("Content-Type", "application/json");
            rq.AddHeader("Authorization", $"Bearer {accesstoken}");

            IRestResponse response = await client.ExecuteAsync(rq);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.Log(LogLevel.Warning, $"[{response.StatusCode}] An error occured while requesting external api.");
                Log.Information($"[{response.StatusCode}] An error occured while requesting external api.");

                return default;
            }

            var data = JsonConvert.DeserializeObject<MedApiResponse<Units>>(response.Content);

            return data;
        }

        public async Task<MedApiResponse<Users>> GetUserByUnitCodeAsync(string code, string endPoint, string accesstoken)
        {
            var client = new RestClient(endPoint);

            var rq = new RestRequest(Method.GET);
            rq.AddHeader("Content-Type", "application/json");
            rq.AddHeader("Authorization", $"Bearer {accesstoken}");

            IRestResponse response = await client.ExecuteAsync(rq);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.Log(LogLevel.Warning, $"[{response.StatusCode}] An error occured while requesting external api.");
                Log.Information($"[{response.StatusCode}] An error occured while requesting external api.");

                return default;
            }
            var data = JsonConvert.DeserializeObject<MedApiResponse<Users>>(response.Content);

            return data;
        }

        public async Task<MedTelegramBotResponse> SendMessage(string body, string accesstoken, string endPoint)
        {
            var client = new RestClient(endPoint);

            var rq = new RestRequest(Method.POST);
            rq.AddParameter("application/json", body, ParameterType.RequestBody);
            rq.AddHeader("Content-Type", "application/json");
            rq.AddHeader("Authorization", $"Bearer {accesstoken}");

            IRestResponse response = await client.ExecuteAsync(rq);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Log.Information($"[{response.StatusCode}] An error occured while requesting external api.");

                return default;
            }
            var data = JsonConvert.DeserializeObject<MedTelegramBotResponse>(response.Content);
            return data;
        }

        public async Task<MedApiResponse<UserSync>> GetUsersAsync(string endPoint, string accesstoken, int page, int size)
        {
            var client = new RestClient(string.Format(endPoint, page, size));

            var rq = new RestRequest(Method.GET);
            rq.AddHeader("Content-Type", "application/json");
            rq.AddHeader("Authorization", $"Bearer {accesstoken}");

            IRestResponse response = await client.ExecuteAsync(rq);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.Log(LogLevel.Warning, $"[{response.StatusCode}] An error occured while requesting external api.");
                Log.Information($"[{response.StatusCode}] An error occured while requesting external api.");
                return default;
            }

            var data = JsonConvert.DeserializeObject<MedApiResponse<UserSync>>(response.Content);

            return data;
        }

        public async Task<MedApiResponse<UserSync>> LoginAsync(string accesstoken, string user, string pass, string endPoint)
        {

            var client = new RestClient(endPoint);

            var rq = new RestRequest(Method.POST);
            rq.AddHeader("Content-Type", "application/json");
            rq.AddHeader("Authorization", $"Bearer {accesstoken}");
            rq.AddParameter("application/json", JsonConvert.SerializeObject(new
            {
                Username = user,
                Password = pass
            }), ParameterType.RequestBody);

            IRestResponse response = await client.ExecuteAsync(rq);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.Log(LogLevel.Warning, $"[{response.StatusCode}] An error occured while requesting external api.");
                return default;
            }

            var data = JsonConvert.DeserializeObject<MedApiResponse<UserSync>>(response.Content);

            return data;
        }
    }
}
