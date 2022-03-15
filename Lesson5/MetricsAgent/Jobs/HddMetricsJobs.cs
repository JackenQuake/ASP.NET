using MetricsAgent.DAL;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MetricsAgent.Jobs
{
	public class HddMetricJob : IJob
	{
		private IHddMetricsRepository _repository;
		// ������� ��� ������� HDD
		private PerformanceCounter _hddCounter;

		public HddMetricJob(IHddMetricsRepository repository)
		{
			_repository = repository;
			_hddCounter = new PerformanceCounter("LogicalDisk ", "% Free Space", "C:");
		}

		public Task Execute(IJobExecutionContext context)
		{
			// �������� �������� ����������� ���������� ����� �� HDD
			var hddLeft = Convert.ToInt32(_hddCounter.NextValue());

			// ������, ����� �� ����� �������� �������
			var time = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

			// ������ ����� �������� ���-�� ����������� �����������

			_repository.Create(new Models.HddMetric { Time = time, Value = hddLeft });

			return Task.CompletedTask;
		}
	}
}
