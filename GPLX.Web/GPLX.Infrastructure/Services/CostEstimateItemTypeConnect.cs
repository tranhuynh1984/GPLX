using GPLX.Core.DTO.Response;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Contracts;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GPLX.Core.DTO.Response.CostEstimateItem;

namespace GPLX.Infrastructure.Services
{
    public class CostEstimateItemTypeConnect : ICostEstimateItemTypeConnect
	{
		private readonly HttpClient _httpClient;
		private readonly ILogger<CostEstimateItemTypeConnect> _logger;
		public CostEstimateItemTypeConnect(HttpClient httpClient, ILogger<CostEstimateItemTypeConnect> logger)
		{
			_httpClient = httpClient;
			_logger = logger;
		}

		public async Task<IList<CostEstimateItemTypeResponse>> GetDataAsync(string endPoint)
		{
			//var httpResponse = await _httpClient.GetAsync(endPoint);

			//if (!httpResponse.IsSuccessStatusCode)
			//{
			//	_logger.Log(LogLevel.Warning, $"[{httpResponse.StatusCode}] An error occured while requesting external api.");
			//	return default;
			//}

			//var jsonString = await httpResponse.Content.ReadAsStringAsync();
			//var data = JsonSerializer.Deserialize<CostEstimateItemTypeResponse[]>(jsonString);

			var data = new List<CostEstimateItemTypeResponse>
			{
				new CostEstimateItemTypeResponse{
					CostEstimateItemTypeName = "Chi hoạt động thường quy",
					CostEstimateItemTypeId = 1
				},
				new CostEstimateItemTypeResponse{
					CostEstimateItemTypeName = "Chi hoạt động không thường quy",
					CostEstimateItemTypeId = 2
				},
				new CostEstimateItemTypeResponse{
					CostEstimateItemTypeName = "Chi đầu tư",
					CostEstimateItemTypeId = 3
				},
				new CostEstimateItemTypeResponse{
					CostEstimateItemTypeName = "Chi tài chính",
					CostEstimateItemTypeId = 4
				},
			}.ToList();


			return await Task.FromResult(data);
		}
	}
}
