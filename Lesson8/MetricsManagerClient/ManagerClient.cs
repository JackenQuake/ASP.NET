using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace MetricsManagerClient
{
	class ManagerClient
	{
		private readonly string managerAddress;
		private readonly HttpClient httpClient;

		public ManagerClient(string managerAddress)
		{
			this.managerAddress = managerAddress;
			httpClient = new HttpClient();
		}

		public List<int> GetAgentList()
		{
			var result = new List<int>();
			var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{managerAddress}/api/controller/list");
			try
			{
				HttpResponseMessage response = httpClient.SendAsync(httpRequest).Result;
				using (var responseStream = response.Content.ReadAsStreamAsync().Result)
				{
					var agents = JsonSerializer.DeserializeAsync<AllAgentInfoResponse>(responseStream).Result;
					foreach (AgentInfoDto agent in agents.agentsList) result.Add(agent.agentId);
				}
			} catch (Exception) { }
			return result;
		}

		public async Task UpdateDataView(IDataView target, int agent, string controller, long from, long to)
		{
			var result = new List<double>();
			try
			{
				HttpResponseMessage response = await httpClient.GetAsync($"{managerAddress}/api/metrics/{controller}/agent/{agent}/from/{from}/to/{to}");
				using (var responseStream = response.Content.ReadAsStreamAsync().Result)
				{
					var metrics = JsonSerializer.DeserializeAsync<AllMetricsResponse>(responseStream).Result;
					if ((metrics != null) && (metrics.metrics != null))
						foreach (MetricDto metric in metrics.metrics) result.Add(metric.value);
				}
			} catch (Exception) { }
			target.UpdateData(result);
		}
	}
}
