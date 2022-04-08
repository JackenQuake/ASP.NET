using MetricsAgent.DAL;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MetricsAgent.Jobs
{
	public class CpuMetricJob : IJob
	{
		private ICpuMetricsRepository _repository;
		// ������� ��� ������� CPU
		private PerformanceCounter _cpuCounter;

		public CpuMetricJob(ICpuMetricsRepository repository)
		{
			_repository = repository;
			_cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
		}

		public Task Execute(IJobExecutionContext context)
		{
			// �������� �������� ��������� CPU
			var cpuUsageInPercents = Convert.ToInt32(_cpuCounter.NextValue());

			// ������, ����� �� ����� �������� �������
			var time = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

			// ������ ����� �������� ���-�� ����������� �����������

			_repository.Create(new Models.CpuMetric { Time = time, Value = cpuUsageInPercents });

			return Task.CompletedTask;
		}
	}
}
