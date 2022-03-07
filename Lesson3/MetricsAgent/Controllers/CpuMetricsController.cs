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
	[Route("api/metrics/cpu")]
	[ApiController]
	public class CpuMetricsController : ControllerBase
	{
		private ICpuMetricsRepository _repository;

		private readonly ILogger<CpuMetricsController> _logger;

		public CpuMetricsController(ICpuMetricsRepository repository, ILogger<CpuMetricsController> logger) 
		{
			_repository = repository;
			//_logger = (logger != null) ? logger : (ILogger<CpuMetricsController>)NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger(this.GetType());
			_logger = logger;
			if (_logger != null) _logger.LogDebug(1, "NLog встроен в CpuMetricsController");
		}

		[HttpGet("from/{fromTime}/to/{toTime}")]
		public IActionResult GetMetrics([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			if (_logger != null) _logger.LogInformation($"Agent CPU metrics request: {fromTime} - {toTime}.");

			var metrics = _repository.GetByTimePeriod(fromTime, toTime);

			if (metrics == null) return Ok();

			var response = new AllCpuMetricsResponse()
			{
				Metrics = new List<CpuMetricDto>()
			};
			foreach (var metric in metrics)
			{
				response.Metrics.Add(new CpuMetricDto { Time = metric.Time, Value = metric.Value, Id = metric.Id });
			}
			return Ok(response);
		}

		[HttpGet("from/{fromTime}/to/{toTime}/percentiles/{percentile}")]
		public IActionResult GetMetricsPercentile([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime, [FromRoute] int Percentile)
		{
			if (_logger != null) _logger.LogInformation($"Agent CPU metrics request: {fromTime} - {toTime}, percentile: {Percentile}.");
			return Ok();
		}
	}
}
