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
	[Route("api/controller")]
	[ApiController]
	public class AgentsController : ControllerBase
	{
		private readonly IAgentInfoRepository repository;
		private readonly IMapper mapper;
		private readonly ILogger<AgentsController> logger;

		public AgentsController(IAgentInfoRepository repository, IMapper mapper, ILogger<AgentsController> logger)
		{
			this.repository = repository;
			this.mapper = mapper;
			this.logger = logger;
			if (logger != null) logger.LogDebug(1, "NLog встроен в AgentsController");
		}

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

		[HttpPost("register")]
		public IActionResult RegisterAgent([FromBody] AgentInfo agentInfo)
		{
			if (logger != null) logger.LogInformation($"AgentsController: new agent registered.");
			repository.Create(agentInfo);
			return Ok();
		}

		[HttpPut("enable/{agentId}")]
		public IActionResult EnableAgentById([FromRoute] int agentId)
		{
			if (logger != null) logger.LogInformation($"AgentsController: agent {agentId} enabled.");
			repository.ChangeEnabled(agentId, true);
			return Ok();
		}

		[HttpPut("disable/{agentId}")]
		public IActionResult DisableAgentById([FromRoute] int agentId)
		{
			if (logger != null) logger.LogInformation($"AgentsController: agent {agentId} disabled.");
			repository.ChangeEnabled(agentId, false);
			return Ok();
		}
	}
}
