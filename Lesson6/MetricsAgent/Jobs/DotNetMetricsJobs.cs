using MetricsAgent.DAL;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MetricsAgent.Jobs
{
	public class DotNetMetricJob : IJob
	{
		private IDotNetMetricsRepository _repository;
		// Счётчик для метрики DotNet
		private PerformanceCounter _dotNetCounter;

		public DotNetMetricJob(IDotNetMetricsRepository repository)
		{
			_repository = repository;
			_dotNetCounter = new PerformanceCounter("ASP.NET Applications", "Errors Total", "__Total__");
		}

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
