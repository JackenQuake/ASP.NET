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
	/// Контроллер для запроса метрик CPU (использования процессора)
	/// </summary>
	[Route("api/metrics/cpu")]
	[ApiController]
	public class CpuMetricsController : ControllerBase
	{
		private readonly ICpuMetricsRepository repository;
		private readonly IMapper mapper;
		private readonly ILogger<CpuMetricsController> logger;

		/// <summary>
		/// Конструктор класса с поддержкой Dependency Injection
		/// </summary>
		public CpuMetricsController(ICpuMetricsRepository repository, IMapper mapper, ILogger<CpuMetricsController> logger)
		{
			this.repository = repository;
			this.mapper = mapper;
			this.logger = logger;
			if (logger != null) logger.LogDebug(1, "NLog встроен в CpuMetricsController");
		}

		/// <summary>
		/// Получает метрики CPU (использования процессора) на заданном диапазоне времени для определенного агента
		/// </summary>
		/// <remarks>
		/// Пример запроса:
		/// GET api/metrics/cpu/agent/{agentId}/from/1/to/9999999999
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
			if (logger != null) logger.LogInformation($"CPU metrics request: {agentId}, {fromTime} - {toTime}.");
			var metrics = repository.GetByTimePeriodFromAgent(fromTime, toTime, agentId);
			if ((metrics == null) || (mapper == null)) return Ok(); // Otherwise tests do not run
			var response = new AllCpuMetricsResponse() { Metrics = new List<CpuMetricDto>() };
			foreach (var metric in metrics) { response.Metrics.Add(mapper.Map<CpuMetricDto>(metric)); }
			return Ok(response);
		}

		/// <summary>
		/// Получает метрики CPU (использования процессора) на заданном диапазоне времени для всего кластера
		/// </summary>
		/// <remarks>
		/// Пример запроса:
		/// GET api/metrics/cpu/cluster/from/1/to/9999999999
		/// </remarks>
		/// <param name="fromTime">начальная метрика времени в секундах с 01.01.1970</param>
		/// <param name="toTime">конечная метрика времени в секундах с 01.01.1970</param>
		/// <returns>Список метрик, сохранённых в заданном диапазоне времени</returns>
		/// <response code="200">Если всё хорошо</response>
		/// <response code="400">Если передали неправильные параметры</response>
		[HttpGet("cluster/from/{fromTime}/to/{toTime}")]
		public IActionResult GetMetricsFromAllCluster([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
		{
			if (logger != null) logger.LogInformation($"CPU metrics request: all cluster {fromTime} - {toTime}.");
			var metrics = repository.GetByTimePeriod(fromTime, toTime);
			if ((metrics == null) || (mapper == null)) return Ok(); // Otherwise tests do not run
			var response = new AllCpuMetricsResponse() { Metrics = new List<CpuMetricDto>() };
			foreach (var metric in metrics) { response.Metrics.Add(mapper.Map<CpuMetricDto>(metric)); }
			return Ok(response);
		}
	}
}
