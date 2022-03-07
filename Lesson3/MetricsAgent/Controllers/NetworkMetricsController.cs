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
	[Route("api/metrics/network")]
	[ApiController]
	public class NetworkMetricsController : ControllerBase
	{
		private INetworkMetricsRepository _repository;

		private readonly ILogger<NetworkMetricsController> _logger;

		public NetworkMetricsController(INetworkMetricsRepository repository, ILogger<NetworkMetricsController> logger)
		{
			_repository = repository;
			//_logger = (logger != null) ? logger : (ILogger<NetworkMetricsController>)NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger(this.GetType());
			_logger = logger;
			if (_logger != null) _logger.LogDebug(1, "NLog встроен в NetworkMetricsController");
		}

		[HttpGet("from/{fromTime}/to/{toTime}")]
		public IActionResult GetMetrics([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			if (_logger != null) _logger.LogInformation($"Agent network metrics request: {fromTime} - {toTime}.");

			var metrics = _repository.GetByTimePeriod(fromTime, toTime);

			if (metrics == null) return Ok();

			var response = new AllNetworkMetricsResponse()
			{
				Metrics = new List<NetworkMetricDto>()
			};

			foreach (var metric in metrics)
			{
				response.Metrics.Add(new NetworkMetricDto { Time = metric.Time, Value = metric.Value, Id = metric.Id });
			}

			return Ok(response);
		}
	}
}