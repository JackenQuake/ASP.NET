using Microsoft.AspNetCore.Mvc;
using System;
using NLog.Web;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using MetricsAgent.DAL;
using MetricsAgent.Models;
using MetricsAgent.Requests;
using MetricsAgent.Responses;

namespace MetricsAgent.Controllers
{
	[Route("api/metrics/ram")]
	[ApiController]
	public class RamMetricsController : ControllerBase
	{
		private IRamMetricsRepository _repository;

		private readonly ILogger<RamMetricsController> _logger;

		public RamMetricsController(IRamMetricsRepository repository, ILogger<RamMetricsController> logger)
		{
			_repository = repository;
			//_logger = (logger != null) ? logger : (ILogger<RamMetricsController>)NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger(this.GetType());
			_logger = logger;
			if (_logger != null) _logger.LogDebug(1, "NLog встроен в RamMetricsController");
		}

		[HttpGet("available/from/{fromTime}/to/{toTime}")]
		public IActionResult GetAvailableMetrics([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			if (_logger != null) _logger.LogInformation($"Agent RAM metrics request: {fromTime} - {toTime}.");

			var metrics = _repository.GetByTimePeriod(fromTime, toTime);

			if (metrics == null) return Ok();

			var response = new AllRamMetricsResponse()
			{
				Metrics = new List<RamMetricDto>()
			};

			foreach (var metric in metrics)
			{
				response.Metrics.Add(new RamMetricDto { Time = metric.Time, Value = metric.Value, Id = metric.Id });
			}

			return Ok(response);
		}
	}
}