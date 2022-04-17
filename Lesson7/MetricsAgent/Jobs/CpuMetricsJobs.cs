using MetricsAgent.DAL;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MetricsAgent.Jobs
{
	/// <summary>
	/// Периодически выполняемая задача сбора метрики CPU (использования процессора)
	/// </summary>
	public class CpuMetricJob : IJob
	{
		private ICpuMetricsRepository _repository;
		// Счётчик для метрики CPU
		private PerformanceCounter _cpuCounter;

		/// <summary>
		/// Конструктор класса с поддержкой Dependency Injection
		/// </summary>
		public CpuMetricJob(ICpuMetricsRepository repository)
		{
			_repository = repository;
			_cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
		}

		/// <summary>
		/// Периодически вызываемый метод для сбора информации и записи в репозиторий
		/// </summary>
		public Task Execute(IJobExecutionContext context)
		{
			// Получаем значение занятости CPU
			var cpuUsageInPercents = Convert.ToInt32(_cpuCounter.NextValue());

			// Узнаем, когда мы сняли значение метрики
			var time = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

			// Теперь можно записать что-то посредством репозитория

			_repository.Create(new Models.CpuMetric { Time = time, Value = cpuUsageInPercents });

			return Task.CompletedTask;
		}
	}
}
