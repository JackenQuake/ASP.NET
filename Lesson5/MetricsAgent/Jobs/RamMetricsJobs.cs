using MetricsAgent.DAL;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MetricsAgent.Jobs
{
	public class RamMetricJob : IJob
	{
		private IRamMetricsRepository _repository;
		// Счётчик для метрики RAM
		private PerformanceCounter _ramCounter;

		public RamMetricJob(IRamMetricsRepository repository)
		{
			_repository = repository;
			_ramCounter = new PerformanceCounter("Memory", "Available MBytes");
		}

		public Task Execute(IJobExecutionContext context)
		{
			// Получаем объем свободной RAM
			var ramAvailable = Convert.ToInt32(_ramCounter.NextValue());

			// Узнаем, когда мы сняли значение метрики
			var time = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

			// Теперь можно записать что-то посредством репозитория

			_repository.Create(new Models.RamMetric { Time = time, Value = ramAvailable });

			return Task.CompletedTask;
		}
	}
}
