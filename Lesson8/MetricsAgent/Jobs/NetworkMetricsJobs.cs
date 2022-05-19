using MetricsAgent.DAL;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MetricsAgent.Jobs
{
	/// <summary>
	/// ������������ ����������� ������ ����� ������� Network (���������� ������������ ��������� �� IPv4 � �������)
	/// </summary>
	public class NetworkMetricJob : IJob
	{
		private INetworkMetricsRepository _repository;
		// ������� ��� ������� Network
		private PerformanceCounter _networkCounter;

		/// <summary>
		/// ����������� ������ � ���������� Dependency Injection
		/// </summary>
		public NetworkMetricJob(INetworkMetricsRepository repository)
		{
			_repository = repository;
			_networkCounter = new PerformanceCounter("IPv4", "Datagrams/sec");
		}

		/// <summary>
		/// ������������ ���������� ����� ��� ����� ���������� � ������ � �����������
		/// </summary>
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
