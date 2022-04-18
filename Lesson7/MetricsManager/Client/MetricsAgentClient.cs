using MetricsManager.Responses;
using MetricsManager.Requests;
using MetricsManager.Client.Responses;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;

namespace MetricsManager.Client
{
	/// <summary>
	///  лиент, запрашивающий у агентов собранные метрики
	/// </summary>
	public class MetricsAgentClient : IMetricsAgentClient
	{
		private readonly HttpClient httpClient;
		private readonly ILogger<MetricsAgentClient> logger;

		/// <summary>
		///  онструктор класса с поддержкой Dependency Injection
		/// </summary>
		public MetricsAgentClient(HttpClient httpClient, ILogger<MetricsAgentClient> logger)
		{
			this.httpClient = httpClient;
			this.logger = logger;
		}

		/// <summary>
		/// «апрашивает у агента метрики CPU (использовани€ процессора) на заданном диапазоне времени
		/// </summary>
		/// <param name="request">ѕараметры запроса (временной интервал)</param>
		/// <returns>—писок метрик, полученных от агента</returns>
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
			} catch (Exception ex)
			{
				logger.LogError(ex.Message);
			}
			return null;
		}

		/// <summary>
		/// «апрашивает у агента метрики DotNet (количество ошибок .NET) на заданном диапазоне времени
		/// </summary>
		/// <param name="request">ѕараметры запроса (временной интервал)</param>
		/// <returns>—писок метрик, полученных от агента</returns>
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
			} catch (Exception ex)
			{
				logger.LogError(ex.Message);
			}
			return null;
		}

		/// <summary>
		/// «апрашивает у агента метрики HDD (свободное пространство на диске C:) на заданном диапазоне времени
		/// </summary>
		/// <param name="request">ѕараметры запроса (временной интервал)</param>
		/// <returns>—писок метрик, полученных от агента</returns>
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
			} catch (Exception ex)
			{
				logger.LogError(ex.Message);
			}
			return null;
		}

		/// <summary>
		/// «апрашивает у агента метрики Network (количество передаваемых дейтаграм по IPv4 в секунду) на заданном диапазоне времени
		/// </summary>
		/// <param name="request">ѕараметры запроса (временной интервал)</param>
		/// <returns>—писок метрик, полученных от агента</returns>
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
			} catch (Exception ex)
			{
				logger.LogError(ex.Message);
			}
			return null;
		}

		/// <summary>
		/// «апрашивает у агента метрики RAM (объем свободной пам€ти) на заданном диапазоне времени
		/// </summary>
		/// <param name="request">ѕараметры запроса (временной интервал)</param>
		/// <returns>—писок метрик, полученных от агента</returns>
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
			} catch (Exception ex)
			{
				logger.LogError(ex.Message);
			}
			return null;
		}
	}
}
