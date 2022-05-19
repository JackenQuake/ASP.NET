using MetricsAgent.DAL;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MetricsAgent.Jobs
{
	/// <summary>
	/// Периодически выполняемая задача сбора метрики HDD (свободное пространство на диске C:)
	/// </summary>
	public class HddMetricJob : IJob
	{
		private IHddMetricsRepository _repository;
		// Счётчик для метрики HDD
		private PerformanceCounter _hddCounter;

		/// <summary>
		/// Конструктор класса с поддержкой Dependency Injection
		/// </summary>
		public HddMetricJob(IHddMetricsRepository repository)
		{
			_repository = repository;
			_hddCounter = new PerformanceCounter("LogicalDisk", "% Free Space", "C:");
		}

		/// <summary>
		/// Периодически вызываемый метод для сбора информации и записи в репозиторий
		/// </summary>
		public Task Execute(IJobExecutionContext context)
		{
			// Получаем значение оставшегося свободного места на HDD
			var hddLeft = Convert.ToInt32(_hddCounter.NextValue());

			// Узнаем, когда мы сняли значение метрики
			var time = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

			// Теперь можно записать что-то посредством репозитория

			_repository.Create(new Models.HddMetric { Time = time, Value = hddLeft });

			return Task.CompletedTask;
		}
	}
}
