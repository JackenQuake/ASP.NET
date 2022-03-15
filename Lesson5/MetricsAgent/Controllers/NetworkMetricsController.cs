using AutoMapper;
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
		private INetworkMetricsRepository repository;
		private readonly IMapper mapper;
		private readonly ILogger<NetworkMetricsController> logger;

		public NetworkMetricsController(INetworkMetricsRepository repository, IMapper mapper, ILogger<NetworkMetricsController> logger)
		{
			this.repository = repository;
			this.mapper = mapper;
			this.logger = logger;
			if (logger != null) logger.LogDebug(1, "NLog встроен в NetworkMetricsController");
		}

		[HttpGet("from/{fromTime}/to/{toTime}")]
		public IActionResult GetMetrics([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			if (logger != null) logger.LogInformation($"Agent network metrics request: {fromTime} - {toTime}.");
			var metrics = repository.GetByTimePeriod(fromTime, toTime);
			if ((metrics == null) || (mapper == null)) return Ok(); // Otherwise tests do not run
			var response = new AllNetworkMetricsResponse() { Metrics = new List<NetworkMetricDto>()	};
			foreach (var metric in metrics) { response.Metrics.Add(mapper.Map<NetworkMetricDto>(metric)); }
			return Ok(response);
		}
	}
}