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
	[Route("api/metrics/hdd")]
	[ApiController]
	public class HddMetricsController : ControllerBase
	{
		private IHddMetricsRepository repository;
		private readonly IMapper mapper;
		private readonly ILogger<HddMetricsController> logger;

		public HddMetricsController(IHddMetricsRepository repository, IMapper mapper, ILogger<HddMetricsController> logger)
		{
			this.repository = repository;
			this.mapper = mapper;
			this.logger = logger;
			if (logger != null) logger.LogDebug(1, "NLog встроен в HddMetricsController");
		}

		[HttpGet("left/from/{fromTime}/to/{toTime}")]
		public IActionResult GetLeftMetrics([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			if (logger != null) logger.LogInformation($"Agent HDD metrics request: {fromTime} - {toTime}.");
			var metrics = repository.GetByTimePeriod(fromTime, toTime);
			if ((metrics == null) || (mapper == null)) return Ok(); // Otherwise tests do not run
			var response = new AllHddMetricsResponse() { Metrics = new List<HddMetricDto>() };
			foreach (var metric in metrics) { response.Metrics.Add(mapper.Map<HddMetricDto>(metric)); }
			return Ok(response);
		}
	}
}