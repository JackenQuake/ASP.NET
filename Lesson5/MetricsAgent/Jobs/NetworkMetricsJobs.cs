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
		// ������� ��� ������� Network
		private PerformanceCounter _networkCounter;

		public NetworkMetricJob(INetworkMetricsRepository repository)
		{
			_repository = repository;
			_networkCounter = new PerformanceCounter("Web Service", "Current Connections");
		}

		public Task Execute(IJobExecutionContext context)
		{
			// �������� �������� ������������� Network
			var networkUsage = Convert.ToInt32(_networkCounter.NextValue());

			// ������, ����� �� ����� �������� �������
			var time = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

			// ������ ����� �������� ���-�� ����������� �����������

			_repository.Create(new Models.NetworkMetric { Time = time, Value = networkUsage });

			return Task.CompletedTask;
		}
	}
}
