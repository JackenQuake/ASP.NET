using Microsoft.AspNetCore.Mvc;
using System;

namespace MetricsManager.Controllers
{
	[Route("api/metrics/network")]
	[ApiController]
	public class NetworkMetricsController : ControllerBase
	{
		[HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}")]
		public IActionResult GetMetricsFromAgent([FromRoute] int agentId, [FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			return Ok();
		}

		[HttpGet("cluster/from/{fromTime}/to/{toTime}")]
		public IActionResult GetMetricsFromAllCluster([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			return Ok();
		}
	}
}
