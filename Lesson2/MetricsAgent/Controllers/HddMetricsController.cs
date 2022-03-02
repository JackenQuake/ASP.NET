using Microsoft.AspNetCore.Mvc;
using System;

namespace MetricsAgent.Controllers
{
	[Route("api/metrics/hdd")]
	[ApiController]
	public class HddMetricsController : ControllerBase
	{
		[HttpGet("left/from/{fromTime}/to/{toTime}")]
		public IActionResult GetLeftMetrics([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			return Ok();
		}
	}
}