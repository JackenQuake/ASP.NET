using MetricsAgent.DAL;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MetricsAgent.Jobs
{
	/// <summary>
	/// Периодически выполняемая задача сбора метрики DotNet (количество ошибок .NET)
	/// </summary>
	public class DotNetMetricJob : IJob
	{
		private IDotNetMetricsRepository _repository;
		// Счётчик для метрики DotNet
		private PerformanceCounter _dotNetCounter;

		/// <summary>
		/// Конструктор класса с поддержкой Dependency Injection
		/// </summary>
		public DotNetMetricJob(IDotNetMetricsRepository repository)
		{
			_repository = repository;
			_dotNetCounter = new PerformanceCounter("ASP.NET Applications", "Errors Total", "__Total__");
		}

		/// <summary>
		/// Периодически вызываемый метод для сбора информации и записи в репозиторий
		/// </summary>
		public Task Execute(IJobExecutionContext context)
		{
			// Получаем значение счетчика ошибок DotNet
			var dotNetErrorCount = Convert.ToInt32(_dotNetCounter.NextValue());

			// Узнаем, когда мы сняли значение метрики
			var time = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

			// Теперь можно записать что-то посредством репозитория

			_repository.Create(new Models.DotNetMetric { Time = time, Value = dotNetErrorCount });

			return Task.CompletedTask;
		}
	}
}
