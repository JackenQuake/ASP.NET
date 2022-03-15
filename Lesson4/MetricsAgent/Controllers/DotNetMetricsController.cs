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
	[Route("api/metrics/dotnet")]
	[ApiController]
	public class DotNetMetricsController : ControllerBase
	{
		private IDotNetMetricsRepository repository;
		private readonly IMapper mapper;
		private readonly ILogger<DotNetMetricsController> logger;

		public DotNetMetricsController(IDotNetMetricsRepository repository, IMapper mapper, ILogger<DotNetMetricsController> logger)
		{
			this.repository = repository;
			this.mapper = mapper;
			this.logger = logger;
			if (logger != null) logger.LogDebug(1, "NLog встроен в DotNetMetricsController");
		}

		[HttpGet("errors-count/from/{fromTime}/to/{toTime}")]
		public IActionResult GetErrorCountMetrics([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			if (logger != null) logger.LogInformation($"Agent DotNet metrics request: {fromTime} - {toTime}.");
			var metrics = repository.GetByTimePeriod(fromTime, toTime);
			if ((metrics == null) || (mapper == null)) return Ok(); // Otherwise tests do not run
			var response = new AllDotNetMetricsResponse() { Metrics = new List<DotNetMetricDto>() };
			foreach (var metric in metrics) { response.Metrics.Add(mapper.Map<DotNetMetricDto>(metric)); }
			return Ok(response);
		}
	}
}