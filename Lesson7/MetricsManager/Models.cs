using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsManager.Models
{
	/// <summary>
	/// Структуры данных, хранящиеся в репозитории
	/// </summary>
	public class AgentInfo
	{
		public int AgentId { get; set; }

		public string AgentUrl { get; set; }

		public bool AgentEnabled { get; set; }
	}

	public class CpuMetric
	{
		public int Id { get; set; }

		public int AgentId { get; set; }

		public int Value { get; set; }

		public TimeSpan Time { get; set; }
	}

	public class DotNetMetric
	{
		public int Id { get; set; }

		public int AgentId { get; set; }

		public int Value { get; set; }

		public TimeSpan Time { get; set; }
	}

	public class HddMetric
	{
		public int Id { get; set; }

		public int AgentId { get; set; }

		public int Value { get; set; }

		public TimeSpan Time { get; set; }
	}

	public class NetworkMetric
	{
		public int Id { get; set; }

		public int AgentId { get; set; }

		public int Value { get; set; }

		public TimeSpan Time { get; set; }
	}

	public class RamMetric
	{
		public int Id { get; set; }

		public int AgentId { get; set; }

		public int Value { get; set; }

		public TimeSpan Time { get; set; }
	}
}
