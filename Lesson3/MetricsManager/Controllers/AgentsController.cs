using Microsoft.AspNetCore.Mvc;
using NLog.Web;
using Microsoft.Extensions.Logging;

namespace MetricsManager.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AgentsController : ControllerBase
	{
		private readonly ILogger<AgentsController> _logger;

		private static ILogger<AgentsController> GetDefaultLogger() => (ILogger<AgentsController>)NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger(typeof(AgentsController));

		public AgentsController() : this(GetDefaultLogger()) { }

		public AgentsController(ILogger<AgentsController> logger)
		{
			_logger = logger;
			_logger.LogDebug(1, "NLog встроен в AgentsController");
		}

		[HttpGet("list")]
		public IActionResult GetAgentList()
		{
			_logger.LogInformation($"AgentsController: list requested.");
			return Ok();
		}

		[HttpPost("register")]
		public IActionResult RegisterAgent([FromBody] AgentInfo agentInfo)
		{
			_logger.LogInformation($"AgentsController: new agent registered.");
			return Ok();
		}

		[HttpPut("enable/{agentId}")]
		public IActionResult EnableAgentById([FromRoute] int agentId)

		{
			_logger.LogInformation($"AgentsController: agent {agentId} enabled.");
			return Ok();
		}

		[HttpPut("disable/{agentId}")]
		public IActionResult DisableAgentById([FromRoute] int agentId)
		{
			_logger.LogInformation($"AgentsController: agent {agentId} disabled.");
			return Ok();
		}
	}
}
