using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using NLog.Web;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using MetricsManager.DAL;
using MetricsManager.Models;
using MetricsManager.Requests;
using MetricsManager.Responses;

namespace MetricsManager.Controllers
{
	/// <summary>
	/// Контроллер для запроса метрик DotNet (количество ошибок .NET)
	/// </summary>
	[Route("api/metrics/dotnet")]
	[ApiController]
	public class DotNetMetricsController : ControllerBase
	{
		private IDotNetMetricsRepository repository;
		private readonly IMapper mapper;
		private readonly ILogger<DotNetMetricsController> logger;

		/// <summary>
		/// Конструктор класса с поддержкой Dependency Injection
		/// </summary>
		public DotNetMetricsController(IDotNetMetricsRepository repository, IMapper mapper, ILogger<DotNetMetricsController> logger)
		{
			this.repository = repository;
			this.mapper = mapper;
			this.logger = logger;
			if (logger != null) logger.LogDebug(1, "NLog встроен в DotNetMetricsController");
		}

		/// <summary>
		/// Получает метрики DotNet (количество ошибок .NET) на заданном диапазоне времени для определенного агента
		/// </summary>
		/// <remarks>
		/// Пример запроса:
		/// GET api/metrics/dotnet/agent/{agentId}/from/1/to/9999999999
		/// </remarks>
		/// <param name="agentId">идентификатор агента</param>
		/// <param name="fromTime">начальная метрика времени в секундах с 01.01.1970</param>
		/// <param name="toTime">конечная метрика времени в секундах с 01.01.1970</param>
		/// <returns>Список метрик, сохранённых в заданном диапазоне времени</returns>
		/// <response code="200">Если всё хорошо</response>
		/// <response code="400">Если передали неправильные параметры</response>
		[HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}")]
		public IActionResult GetMetricsFromAgent([FromRoute] int agentId, [FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			if (logger != null) logger.LogInformation($"DotNet metrics request: {agentId}, {fromTime} - {toTime}.");
			var metrics = repository.GetByTimePeriodFromAgent(fromTime, toTime, agentId);
			if ((metrics == null) || (mapper == null)) return Ok(); // Otherwise tests do not run
			var response = new AllDotNetMetricsResponse() { Metrics = new List<DotNetMetricDto>() };
			foreach (var metric in metrics) { response.Metrics.Add(mapper.Map<DotNetMetricDto>(metric)); }
			return Ok(response);
		}

		/// <summary>
		/// Получает метрики DotNet (количество ошибок .NET) на заданном диапазоне времени для всего кластера
		/// </summary>
		/// <remarks>
		/// Пример запроса:
		/// GET api/metrics/dotnet/cluster/from/1/to/9999999999
		/// </remarks>
		/// <param name="fromTime">начальная метрика времени в секундах с 01.01.1970</param>
		/// <param name="toTime">конечная метрика времени в секундах с 01.01.1970</param>
		/// <returns>Список метрик, сохранённых в заданном диапазоне времени</returns>
		/// <response code="200">Если всё хорошо</response>
		/// <response code="400">Если передали неправильные параметры</response>
		[HttpGet("cluster/from/{fromTime}/to/{toTime}")]
		public IActionResult GetMetricsFromAllCluster([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			if (logger != null) logger.LogInformation($"DotNet metrics request: all cluster {fromTime} - {toTime}.");
			var metrics = repository.GetByTimePeriod(fromTime, toTime);
			if ((metrics == null) || (mapper == null)) return Ok(); // Otherwise tests do not run
			var response = new AllDotNetMetricsResponse() { Metrics = new List<DotNetMetricDto>() };
			foreach (var metric in metrics) { response.Metrics.Add(mapper.Map<DotNetMetricDto>(metric)); }
			return Ok(response);
		}
	}
}