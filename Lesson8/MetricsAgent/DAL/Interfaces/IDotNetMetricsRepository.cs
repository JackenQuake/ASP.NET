using Core;
using MetricsAgent.Models;

namespace MetricsAgent.DAL
{
	/// <summary>
	/// Интерфейс репозитория метрики DotNet (количество ошибок .NET)
	/// </summary>
	public interface IDotNetMetricsRepository : IRepository<DotNetMetric>
	{

	}
}
