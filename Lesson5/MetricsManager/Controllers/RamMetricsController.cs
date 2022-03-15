using Microsoft.AspNetCore.Mvc;
using System;
using NLog.Web;
using Microsoft.Extensions.Logging;

namespace MetricsManager.Controllers
{
	[Route("api/metrics/ram")]
	[ApiController]
	public class RamMetricsController : ControllerBase
	{
		private readonly ILogger<RamMetricsController> _logger;

		private static ILogger<RamMetricsController> GetDefaultLogger() => (ILogger<RamMetricsController>)NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger(typeof(RamMetricsController));

		public RamMetricsController() : this(GetDefaultLogger()) { }

		public RamMetricsController(ILogger<RamMetricsController> logger)
		{
			_logger = logger;
			_logger.LogDebug(1, "NLog встроен в RamMetricsController");
		}

		[HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}")]
		public IActionResult GetMetricsFromAgent([FromRoute] int agentId, [FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			_logger.LogInformation($"RAM metrics request: {agentId}, {fromTime} - {toTime}.");
			return Ok();
		}

		[HttpGet("cluster/from/{fromTime}/to/{toTime}")]
		public IActionResult GetMetricsFromAllCluster([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			_logger.LogInformation($"RAM metrics request: all cluster {fromTime} - {toTime}.");
			return Ok();
		}
	}
}
