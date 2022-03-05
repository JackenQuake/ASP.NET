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
	[Route("api/metrics/hdd")]
	[ApiController]
	public class HddMetricsController : ControllerBase
	{
		private IHddMetricsRepository _repository;

		private readonly ILogger<HddMetricsController> _logger;

		public HddMetricsController(IHddMetricsRepository repository, ILogger<HddMetricsController> logger)
		{
			_repository = repository;
			//_logger = (logger != null) ? logger : (ILogger<HddMetricsController>)NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger(this.GetType());
			_logger = logger;
			if (_logger != null) _logger.LogDebug(1, "NLog встроен в HddMetricsController");
		}

		[HttpGet("left/from/{fromTime}/to/{toTime}")]
		public IActionResult GetLeftMetrics([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			if (_logger != null) _logger.LogInformation($"Agent HDD metrics request: {fromTime} - {toTime}.");

			var metrics = _repository.GetByTimePeriod(fromTime, toTime);

			if (metrics == null) return Ok();

			var response = new AllHddMetricsResponse()
			{
				Metrics = new List<HddMetricDto>()
			};

			foreach (var metric in metrics)
			{
				response.Metrics.Add(new HddMetricDto { Time = metric.Time, Value = metric.Value, Id = metric.Id });
			}

			return Ok(response);
		}
	}
}