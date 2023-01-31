using GPLX.Core.DTO.Response;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Contracts;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GPLX.Infrastructure.Services
{
    public class SupplierConnect : ISupplierConnect
	{
		private readonly HttpClient _httpClient;
		private readonly ILogger<CostEstimateItemDepartmentConnect> _logger;
		public SupplierConnect(HttpClient httpClient, ILogger<CostEstimateItemDepartmentConnect> logger)
		{
			_httpClient = httpClient;
			_logger = logger;
		}

		public async Task<SupplierResponse[]> GetDataAsync(string endPoint)
		{
			//var httpResponse = await _httpClient.GetAsync(endPoint);

			//if (!httpResponse.IsSuccessStatusCode)
			//{
			//	_logger.Log(LogLevel.Warning, $"[{httpResponse.StatusCode}] An error occured while requesting external api.");
			//	return default;
			//}

			//var jsonString = await httpResponse.Content.ReadAsStringAsync();
			//var data = JsonSerializer.Deserialize<CostEstimateItemTypeResponse[]>(jsonString);

			var data = new List<SupplierResponse>
			{
				new SupplierResponse{
					SupplierName = "Dược phẩm TW 1",
					SupplierId = 1
				},
				new SupplierResponse{
					SupplierName = "Dược phẩm TW 2",
					SupplierId = 2
				},
				new SupplierResponse{
					SupplierName = "Dược phẩm TW 3",
					SupplierId = 3
				},
				new SupplierResponse{
					SupplierName = "Dược phẩm TW 4",
					SupplierId = 4
				},
			}.ToArray();


			return await Task.FromResult(data);
		}
	}
}
