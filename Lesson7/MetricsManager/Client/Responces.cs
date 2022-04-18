using System;
using System.Collections.Generic;
using MetricsManager.Responses;

/// <summary>
/// «апросы менеджера к агенту и возвращаемые агентом структуры данных
/// </summary>
namespace MetricsManager.Client.Responses
{
	public class AllCpuMetricsApiResponse
	{
		public List<CpuMetricDto> Metrics { get; set; }
	}

	public class GetAllCpuMetricsApiRequest
	{
		public TimeSpan FromTime { get; set; }

		public TimeSpan ToTime { get; set; }

		public string ClientBaseAddress { get; set; }
	}

	public class AllDotNetMetricsApiResponse
	{
		public List<DotNetMetricDto> Metrics { get; set; }
	}

	public class GetAllDotNetMetricsApiRequest
	{
		public TimeSpan FromTime { get; set; }

		public TimeSpan ToTime { get; set; }

		public string ClientBaseAddress { get; set; }
	}

	public class AllHddMetricsApiResponse
	{
		public List<HddMetricDto> Metrics { get; set; }
	}

	public class GetAllHddMetricsApiRequest
	{
		public TimeSpan FromTime { get; set; }

		public TimeSpan ToTime { get; set; }

		public string ClientBaseAddress { get; set; }
	}

	public class AllNetworkMetricsApiResponse
	{
		public List<NetworkMetricDto> Metrics { get; set; }
	}

	public class GetAllNetworkMetricsApiRequest
	{
		public TimeSpan FromTime { get; set; }

		public TimeSpan ToTime { get; set; }

		public string ClientBaseAddress { get; set; }
	}

	public class AllRamMetricsApiResponse
	{
		public List<RamMetricDto> Metrics { get; set; }
	}

	public class GetAllRamMetricsApiRequest
	{
		public TimeSpan FromTime { get; set; }

		public TimeSpan ToTime { get; set; }

		public string ClientBaseAddress { get; set; }
	}
}
