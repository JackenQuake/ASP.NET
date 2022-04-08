using MetricsManager.Models;
using MetricsManager.DAL;
using MetricsManager.Client.Responses;
using MetricsManager.Client;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using AutoMapper;

namespace MetricsManager.Jobs
{
	public class NetworkMetricJob : IJob
	{
		private readonly IAgentInfoRepository agent_repository;
		private readonly INetworkMetricsRepository repository;
		private readonly IMetricsAgentClient client;
		private readonly IMapper mapper;

		public NetworkMetricJob(IAgentInfoRepository agents, INetworkMetricsRepository repository, IMetricsAgentClient client, IMapper mapper)
		{
			this.agent_repository = agents;
			this.repository = repository;
			this.client = client;
			this.mapper = mapper;
		}

		public Task Execute(IJobExecutionContext context)
		{
			IList<AgentInfo> agents = agent_repository.GetAll();
			foreach (AgentInfo agent in agents)
				if (agent.AgentEnabled)
				{
					var request = new GetAllNetworkMetricsApiRequest();
					request.FromTime = TimeSpan.FromSeconds(0);
					request.ToTime = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds()); //TimeSpan.FromSeconds(4000000);
					request.ClientBaseAddress = agent.AgentUrl;
					var response = client.GetAllNetworkMetrics(request);
					foreach (var metric in response.Metrics)
					{
						metric.AgentId = agent.AgentId;
						repository.Create(mapper.Map<NetworkMetric>(metric));
					}
				}
			return Task.CompletedTask;
		}
	}
}
