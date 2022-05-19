using MetricsAgent.DAL;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MetricsAgent.Jobs
{
	/// <summary>
	/// Периодически выполняемая задача сбора метрики Network (количество передаваемых дейтаграм по IPv4 в секунду)
	/// </summary>
	public class NetworkMetricJob : IJob
	{
		private INetworkMetricsRepository _repository;
		// Счётчик для метрики Network
		private PerformanceCounter _networkCounter;

		/// <summary>
		/// Конструктор класса с поддержкой Dependency Injection
		/// </summary>
		public NetworkMetricJob(INetworkMetricsRepository repository)
		{
			_repository = repository;
			_networkCounter = new PerformanceCounter("IPv4", "Datagrams/sec");
		}

		/// <summary>
		/// Периодически вызываемый метод для сбора информации и записи в репозиторий
		/// </summary>
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
