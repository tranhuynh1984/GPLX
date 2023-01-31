using GPLX.Core.DTO.Response;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Contracts;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GPLX.Core.DTO.Response.CostEstimateItem;

namespace GPLX.Infrastructure.Services
{
    public class CostEstimateItemDepartmentConnect : ICostEstimateItemDepartmentConnect
	{
		private readonly HttpClient _httpClient;
		private readonly ILogger<CostEstimateItemDepartmentConnect> _logger;
		public CostEstimateItemDepartmentConnect(HttpClient httpClient, ILogger<CostEstimateItemDepartmentConnect> logger)
		{
			_httpClient = httpClient;
			_logger = logger;
		}

		public async Task<CostEstimateItemDepartmentResponse[]> GetDataAsync(string endPoint)
		{
			//var httpResponse = await _httpClient.GetAsync(endPoint);

			//if (!httpResponse.IsSuccessStatusCode)
			//{
			//	_logger.Log(LogLevel.Warning, $"[{httpResponse.StatusCode}] An error occured while requesting external api.");
			//	return default;
			//}

			//var jsonString = await httpResponse.Content.ReadAsStringAsync();
			//var data = JsonSerializer.Deserialize<CostEstimateItemTypeResponse[]>(jsonString);

			var data = new List<CostEstimateItemDepartmentResponse>
			{
				new CostEstimateItemDepartmentResponse{
					CostEstimateItemDepartmentName = "Dược phẩm TW 1",
					CostEstimateItemDepartmentId = 1
				},
				new CostEstimateItemDepartmentResponse{
					CostEstimateItemDepartmentName = "Dược phẩm TW 2",
					CostEstimateItemDepartmentId = 2
				},
				new CostEstimateItemDepartmentResponse{
					CostEstimateItemDepartmentName = "Dược phẩm TW 3",
					CostEstimateItemDepartmentId = 3
				},
				new CostEstimateItemDepartmentResponse{
					CostEstimateItemDepartmentName = "Dược phẩm TW 4",
					CostEstimateItemDepartmentId = 4
				},
			}.ToArray();


			return await Task.FromResult(data);
		}
	}
}
