using MetricsAgent.DAL;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MetricsAgent.Jobs
{
	public class NetworkMetricJob : IJob
	{
		private INetworkMetricsRepository _repository;
		// Счётчик для метрики Network
		private PerformanceCounter _networkCounter;

		public NetworkMetricJob(INetworkMetricsRepository repository)
		{
			_repository = repository;
			_networkCounter = new PerformanceCounter("Web Service", "Current Connections");
		}

		public Task Execute(IJobExecutionContext context)
		{
			// Получаем значение использования Network
			var networkUsage = Convert.ToInt32(_networkCounter.NextValue());

			// Узнаем, когда мы сняли значение метрики
			var time = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

			// Теперь можно записать что-то посредством репозитория

			_repository.Create(new Models.NetworkMetric { Time = time, Value = networkUsage });

			return Task.CompletedTask;
		}
	}
}
