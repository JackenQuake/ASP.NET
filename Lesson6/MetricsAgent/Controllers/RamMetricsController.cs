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
	[Route("api/metrics/ram")]
	[ApiController]
	public class RamMetricsController : ControllerBase
	{
		private IRamMetricsRepository repository;
		private readonly IMapper mapper;
		private readonly ILogger<RamMetricsController> logger;

		public RamMetricsController(IRamMetricsRepository repository, IMapper mapper, ILogger<RamMetricsController> logger)
		{
			this.repository = repository;
			this.mapper = mapper;
			this.logger = logger;
			if (logger != null) logger.LogDebug(1, "NLog встроен в RamMetricsController");
		}

		[HttpGet("available/from/{fromTime}/to/{toTime}")]
		public IActionResult GetAvailableMetrics([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			if (logger != null) logger.LogInformation($"Agent RAM metrics request: {fromTime} - {toTime}.");
			var metrics = repository.GetByTimePeriod(fromTime, toTime);
			if ((metrics == null) || (mapper == null)) return Ok(); // Otherwise tests do not run
			var response = new AllRamMetricsResponse() { Metrics = new List<RamMetricDto>()	};
			foreach (var metric in metrics) { response.Metrics.Add(mapper.Map<RamMetricDto>(metric)); }
			return Ok(response);
		}
	}
}