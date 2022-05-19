using MetricsAgent.DAL;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MetricsAgent.Jobs
{
	/// <summary>
	/// ������������ ����������� ������ ����� ������� RAM (����� ��������� ������)
	/// </summary>
	public class RamMetricJob : IJob
	{
		private IRamMetricsRepository _repository;
		// ������� ��� ������� RAM
		private PerformanceCounter _ramCounter;

		/// <summary>
		/// ����������� ������ � ���������� Dependency Injection
		/// </summary>
		public RamMetricJob(IRamMetricsRepository repository)
		{
			_repository = repository;
			_ramCounter = new PerformanceCounter("Memory", "Available MBytes");
		}

		/// <summary>
		/// ������������ ���������� ����� ��� ����� ���������� � ������ � �����������
		/// </summary>
		public Task Execute(IJobExecutionContext context)
		{
			// �������� ����� ��������� RAM
			var ramAvailable = Convert.ToInt32(_ramCounter.NextValue());

			// ������, ����� �� ����� �������� �������
			var time = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

			// ������ ����� �������� ���-�� ����������� �����������

			_repository.Create(new Models.RamMetric { Time = time, Value = ramAvailable });

			return Task.CompletedTask;
		}
	}
}
