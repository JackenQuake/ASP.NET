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
	[Route("api/metrics/dotnet")]
	[ApiController]
	public class DotNetMetricsController : ControllerBase
	{
		private IDotNetMetricsRepository _repository;

		private readonly ILogger<DotNetMetricsController> _logger;

		public DotNetMetricsController(IDotNetMetricsRepository repository, ILogger<DotNetMetricsController> logger)
		{
			_repository = repository;
			//_logger = (logger != null) ? logger : (ILogger<DotNetMetricsController>)NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger(this.GetType());
			_logger = logger;
			if (_logger != null) _logger.LogDebug(1, "NLog встроен в DotNetMetricsController");
		}

		[HttpGet("errors-count/from/{fromTime}/to/{toTime}")]
		public IActionResult GetErrorCountMetrics([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			if (_logger != null) _logger.LogInformation($"Agent DotNet metrics request: {fromTime} - {toTime}.");
			
			var metrics = _repository.GetByTimePeriod(fromTime, toTime);

			if (metrics == null) return Ok();

			var response = new AllDotNetMetricsResponse()
			{
				Metrics = new List<DotNetMetricDto>()
			};

			foreach (var metric in metrics)
			{
				response.Metrics.Add(new DotNetMetricDto { Time = metric.Time, Value = metric.Value, Id = metric.Id });
			}

			return Ok(response);
		}
	}
}