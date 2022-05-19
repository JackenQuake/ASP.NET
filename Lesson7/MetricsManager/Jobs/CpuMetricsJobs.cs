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
	/// <summary>
	/// Периодически выполняемая задача сбора метрики CPU (использования процессора)
	/// </summary>
	public class CpuMetricJob : IJob
	{
		private readonly IAgentInfoRepository agent_repository;
		private readonly ICpuMetricsRepository repository;
		private readonly IMetricsAgentClient client;
		private readonly IMapper mapper;

		/// <summary>
		/// Конструктор класса с поддержкой Dependency Injection
		/// </summary>
		public CpuMetricJob(IAgentInfoRepository agents, ICpuMetricsRepository repository, IMetricsAgentClient client, IMapper mapper)
		{
			this.agent_repository = agents;
			this.repository = repository;
			this.client = client;
			this.mapper = mapper;
		}

		/// <summary>
		/// Периодически вызываемый метод для сбора информации и записи в репозиторий
		/// </summary>
		public Task Execute(IJobExecutionContext context)
		{
			IList<AgentInfo> agents = agent_repository.GetAll();
			foreach (AgentInfo agent in agents)
				if (agent.AgentEnabled)
				{
					var request = new GetAllCpuMetricsApiRequest();
					request.FromTime = TimeSpan.FromSeconds(0);
					request.ToTime = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds()); //TimeSpan.FromSeconds(4000000);
					request.ClientBaseAddress = agent.AgentUrl;
					var response = client.GetAllCpuMetrics(request);
					if ((response != null) && (response.Metrics != null))
						foreach (var metric in response.Metrics)
						{
							metric.AgentId = agent.AgentId;
							repository.Create(mapper.Map<CpuMetric>(metric));
						}
				}
			return Task.CompletedTask;
		}
	}
}
