using Microsoft.AspNetCore.Mvc;
using System;
using NLog.Web;
using Microsoft.Extensions.Logging;

namespace MetricsManager.Controllers
{
	[Route("api/metrics/network")]
	[ApiController]
	public class NetworkMetricsController : ControllerBase
	{
		private readonly ILogger<NetworkMetricsController> _logger;

		private static ILogger<NetworkMetricsController> GetDefaultLogger() => (ILogger<NetworkMetricsController>)NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger(typeof(NetworkMetricsController));

		public NetworkMetricsController() : this(GetDefaultLogger()) { }

		public NetworkMetricsController(ILogger<NetworkMetricsController> logger)
		{
			_logger = logger;
			_logger.LogDebug(1, "NLog встроен в NetworkMetricsController");
		}

		[HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}")]
		public IActionResult GetMetricsFromAgent([FromRoute] int agentId, [FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			_logger.LogInformation($"Network metrics request: {agentId}, {fromTime} - {toTime}.");
			return Ok();
		}

		[HttpGet("cluster/from/{fromTime}/to/{toTime}")]
		public IActionResult GetMetricsFromAllCluster([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			_logger.LogInformation($"Network metrics request: all cluster {fromTime} - {toTime}.");
			return Ok();
		}
	}
}
