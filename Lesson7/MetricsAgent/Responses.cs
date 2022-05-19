using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Структуры данных на уровне контроллера
/// </summary>
namespace MetricsAgent.Responses
{
	public class AllCpuMetricsResponse
	{
		public List<CpuMetricDto> Metrics { get; set; }
	}

	public class CpuMetricDto
	{
		public TimeSpan Time { get; set; }
		public int Value { get; set; }
		public int Id { get; set; }
	}

	public class AllDotNetMetricsResponse
	{
		public List<DotNetMetricDto> Metrics { get; set; }
	}

	public class DotNetMetricDto
	{
		public TimeSpan Time { get; set; }
		public int Value { get; set; }
		public int Id { get; set; }
	}

	public class AllHddMetricsResponse
	{
		public List<HddMetricDto> Metrics { get; set; }
	}

	public class HddMetricDto
	{
		public TimeSpan Time { get; set; }
		public int Value { get; set; }
		public int Id { get; set; }
	}

	public class AllNetworkMetricsResponse
	{
		public List<NetworkMetricDto> Metrics { get; set; }
	}

	public class NetworkMetricDto
	{
		public TimeSpan Time { get; set; }
		public int Value { get; set; }
		public int Id { get; set; }
	}

	public class AllRamMetricsResponse
	{
		public List<RamMetricDto> Metrics { get; set; }
	}

	public class RamMetricDto
	{
		public TimeSpan Time { get; set; }
		public int Value { get; set; }
		public int Id { get; set; }
	}
}
