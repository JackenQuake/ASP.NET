using Microsoft.AspNetCore.Mvc;
using System;
using NLog.Web;
using Microsoft.Extensions.Logging;

namespace MetricsManager.Controllers
{
	[Route("api/metrics/dotnet")]
	[ApiController]
	public class DotNetMetricsController : ControllerBase
	{
		private readonly ILogger<DotNetMetricsController> _logger;

		private static ILogger<DotNetMetricsController> GetDefaultLogger() => (ILogger<DotNetMetricsController>)NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger(typeof(DotNetMetricsController));

		public DotNetMetricsController() : this(GetDefaultLogger()) { }

		public DotNetMetricsController(ILogger<DotNetMetricsController> logger)
		{
			_logger = logger;
			_logger.LogDebug(1, "NLog встроен в DotNetMetricsController");
		}

		[HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}")]
		public IActionResult GetMetricsFromAgent([FromRoute] int agentId, [FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			_logger.LogInformation($"DotNet metrics request: {agentId}, {fromTime} - {toTime}.");
			return Ok();
		}

		[HttpGet("cluster/from/{fromTime}/to/{toTime}")]
		public IActionResult GetMetricsFromAllCluster([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			_logger.LogInformation($"DotNet metrics request: all cluster {fromTime} - {toTime}.");
			return Ok();
		}
	}
}