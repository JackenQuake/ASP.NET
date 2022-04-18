using MetricsManager.Responses;
using MetricsManager.Requests;
using MetricsManager.Client.Responses;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;

namespace MetricsManager.Client
{
	public class MetricsAgentClient : IMetricsAgentClient
	{
		private readonly HttpClient httpClient;
		private readonly ILogger<MetricsAgentClient> logger;

		/// <summary>
		/// Конструктор класса с поддержкой Dependency Injection
		/// </summary>
		public MetricsAgentClient(HttpClient httpClient, ILogger<MetricsAgentClient> logger)
		{
			this.httpClient = httpClient;
			this.logger = logger;
		}

		public AllCpuMetricsApiResponse GetAllCpuMetrics(GetAllCpuMetricsApiRequest request)
		{
			var fromParameter = request.FromTime.TotalSeconds;
			var toParameter = request.ToTime.TotalSeconds;
			var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{request.ClientBaseAddress}/api/metrics/cpu/from/{fromParameter}/to/{toParameter}");
			try
			{
				HttpResponseMessage response = httpClient.SendAsync(httpRequest).Result;
				using var responseStream = response.Content.ReadAsStreamAsync().Result;
				var options = new JsonSerializerOptions();
				options.PropertyNameCaseInsensitive = true;
				AllCpuMetricsApiResponse resp = JsonSerializer.DeserializeAsync<AllCpuMetricsApiResponse>(responseStream, options).Result;
				return resp;
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
			}
			return null;
		}

		public AllDotNetMetricsApiResponse GetAllDotNetMetrics(GetAllDotNetMetricsApiRequest request)
		{
			var fromParameter = request.FromTime.TotalSeconds;
			var toParameter = request.ToTime.TotalSeconds;
			var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{request.ClientBaseAddress}/api/metrics/dotnet/errors-count/from/{fromParameter}/to/{toParameter}");
			try
			{
				HttpResponseMessage response = httpClient.SendAsync(httpRequest).Result;
				using var responseStream = response.Content.ReadAsStreamAsync().Result;
				return JsonSerializer.DeserializeAsync<AllDotNetMetricsApiResponse>(responseStream).Result;
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
			}
			return null;
		}

		public AllHddMetricsApiResponse GetAllHddMetrics(GetAllHddMetricsApiRequest request)
		{
			var fromParameter = request.FromTime.TotalSeconds;
			var toParameter = request.ToTime.TotalSeconds;
			var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{request.ClientBaseAddress}/api/metrics/hdd/left/from/{fromParameter}/to/{toParameter}");
			try
			{
				HttpResponseMessage response = httpClient.SendAsync(httpRequest).Result;
				using var responseStream = response.Content.ReadAsStreamAsync().Result;
				return JsonSerializer.DeserializeAsync<AllHddMetricsApiResponse>(responseStream).Result;
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
			}
			return null;
		}

		public AllNetworkMetricsApiResponse GetAllNetworkMetrics(GetAllNetworkMetricsApiRequest request)
		{
			var fromParameter = request.FromTime.TotalSeconds;
			var toParameter = request.ToTime.TotalSeconds;
			var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{request.ClientBaseAddress}/api/metrics/network/from/{fromParameter}/to/{toParameter}");
			try
			{
				HttpResponseMessage response = httpClient.SendAsync(httpRequest).Result;
				using var responseStream = response.Content.ReadAsStreamAsync().Result;
				return JsonSerializer.DeserializeAsync<AllNetworkMetricsApiResponse>(responseStream).Result;
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
			}
			return null;
		}

		public AllRamMetricsApiResponse GetAllRamMetrics(GetAllRamMetricsApiRequest request)
		{
			var fromParameter = request.FromTime.TotalSeconds;
			var toParameter = request.ToTime.TotalSeconds;
			var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{request.ClientBaseAddress}/api/metrics/ram/available/from/{fromParameter}/to/{toParameter}");
			try
			{
				HttpResponseMessage response = httpClient.SendAsync(httpRequest).Result;
				using var responseStream = response.Content.ReadAsStreamAsync().Result;
				return JsonSerializer.DeserializeAsync<AllRamMetricsApiResponse>(responseStream).Result;
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
			}
			return null;
		}
	}
}
