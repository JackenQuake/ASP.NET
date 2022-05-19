using System;
using System.Collections.Generic;

namespace MetricsManagerClient
{
	public class AllAgentInfoResponse
	{
		public List<AgentInfoDto> agentsList { get; set; }
	}

	public class AgentInfoDto
	{
		public int agentId { get; set; }
		public string agentUrl { get; set; }
		public bool agentEnabled { get; set; }
	}

	public class AllMetricsResponse
	{
		public List<MetricDto> metrics { get; set; }
	}

	public class MetricDto
	{
		public TimeSpan time { get; set; }
		public int agentId { get; set; }
		public int value { get; set; }
		public int id { get; set; }
	}
}
