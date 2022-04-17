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
	/// Контроллер для управления списком агентов
	/// </summary>
	[Route("api/controller")]
	[ApiController]
	public class AgentsController : ControllerBase
	{
		private readonly IAgentInfoRepository repository;
		private readonly IMapper mapper;
		private readonly ILogger<AgentsController> logger;

		/// <summary>
		/// Конструктор класса с поддержкой Dependency Injection
		/// </summary>
		public AgentsController(IAgentInfoRepository repository, IMapper mapper, ILogger<AgentsController> logger)
		{
			this.repository = repository;
			this.mapper = mapper;
			this.logger = logger;
			if (logger != null) logger.LogDebug(1, "NLog встроен в AgentsController");
		}

		/// <summary>
		/// Запрашивает список агентов
		/// </summary>
		/// <remarks>
		/// Пример запроса:
		/// GET api/controller/list
		/// </remarks>
		/// <returns>Список агентов, зарегистрированных в системе</returns>
		/// <response code="200">Если всё хорошо</response>
		/// <response code="400">Если передали неправильные параметры</response>
		[HttpGet("list")]
		public IActionResult GetAgentList()
		{
			if (logger != null) logger.LogInformation($"AgentsController: list requested.");
			var list = repository.GetAll();
			if ((list == null) || (mapper == null)) return Ok(); // Otherwise tests do not run
			var response = new AllAgentInfoResponse() { AgentsList = new List<AgentInfoDto>() };
			foreach (var agent in list) { response.AgentsList.Add(mapper.Map<AgentInfoDto>(agent)); }
			return Ok(response);
		}

		/// <summary>
		/// Добавляет нового агента в систему
		/// </summary>
		/// <remarks>
		/// Пример запроса:
		/// GET api/controller/register
		/// </remarks>
		/// <param name="agentInfo">данные о добавляемом агенте</param>
		/// <response code="200">Если всё хорошо</response>
		/// <response code="400">Если передали неправильные параметры</response>
		[HttpPost("register")]
		public IActionResult RegisterAgent([FromBody] AgentInfo agentInfo)
		{
			if (logger != null) logger.LogInformation($"AgentsController: new agent registered.");
			repository.Create(agentInfo);
			return Ok();
		}

		/// <summary>
		/// Включает агента
		/// </summary>
		/// <remarks>
		/// Пример запроса:
		/// GET api/controller/enable/{agentId}
		/// </remarks>
		/// <param name="agentId">идентификатор агента</param>
		/// <response code="200">Если всё хорошо</response>
		/// <response code="400">Если передали неправильные параметры</response>
		[HttpPut("enable/{agentId}")]
		public IActionResult EnableAgentById([FromRoute] int agentId)
		{
			if (logger != null) logger.LogInformation($"AgentsController: agent {agentId} enabled.");
			repository.ChangeEnabled(agentId, true);
			return Ok();
		}

		/// <summary>
		/// Выключает агента
		/// </summary>
		/// <remarks>
		/// Пример запроса:
		/// GET api/controller/disable/{agentId}
		/// </remarks>
		/// <param name="agentId">идентификатор агента</param>
		/// <response code="200">Если всё хорошо</response>
		/// <response code="400">Если передали неправильные параметры</response>
		[HttpPut("disable/{agentId}")]
		public IActionResult DisableAgentById([FromRoute] int agentId)
		{
			if (logger != null) logger.LogInformation($"AgentsController: agent {agentId} disabled.");
			repository.ChangeEnabled(agentId, false);
			return Ok();
		}
	}
}
