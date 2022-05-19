using MetricsAgent.DAL;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MetricsAgent.Jobs
{
	/// <summary>
	/// Периодически выполняемая задача сбора метрики RAM (объем свободной памяти)
	/// </summary>
	public class RamMetricJob : IJob
	{
		private IRamMetricsRepository _repository;
		// Счётчик для метрики RAM
		private PerformanceCounter _ramCounter;

		/// <summary>
		/// Конструктор класса с поддержкой Dependency Injection
		/// </summary>
		public RamMetricJob(IRamMetricsRepository repository)
		{
			_repository = repository;
			_ramCounter = new PerformanceCounter("Memory", "Available MBytes");
		}

		/// <summary>
		/// Периодически вызываемый метод для сбора информации и записи в репозиторий
		/// </summary>
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
