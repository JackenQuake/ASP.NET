using Microsoft.AspNetCore.Mvc;
using System;
using NLog.Web;
using Microsoft.Extensions.Logging;

namespace MetricsManager.Controllers
{
	[Route("api/metrics/Hdd")]
	[ApiController]
	public class HddMetricsController : ControllerBase
	{
		private readonly ILogger<HddMetricsController> _logger;

		private static ILogger<HddMetricsController> GetDefaultLogger() => (ILogger<HddMetricsController>)NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger(typeof(HddMetricsController));

		public HddMetricsController() : this(GetDefaultLogger()) { }

		public HddMetricsController(ILogger<HddMetricsController> logger)
		{
			_logger = logger;
			_logger.LogDebug(1, "NLog встроен в HddMetricsController");
		}

		[HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}")]
		public IActionResult GetMetricsFromAgent([FromRoute] int agentId, [FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			_logger.LogInformation($"HDD metrics request: {agentId}, {fromTime} - {toTime}.");
			return Ok();
		}

		[HttpGet("cluster/from/{fromTime}/to/{toTime}")]
		public IActionResult GetMetricsFromAllCluster([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			_logger.LogInformation($"HDD metrics request: all cluster {fromTime} - {toTime}.");
			return Ok();
		}
	}
}
