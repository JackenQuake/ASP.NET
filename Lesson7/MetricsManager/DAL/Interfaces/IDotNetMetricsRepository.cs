using MetricsManager.DAL.Interfaces;
using MetricsManager.Models;

namespace MetricsManager.DAL
{
	/// <summary>
	/// Интерфейс репозитория метрики DotNet (количество ошибок .NET)
	/// </summary>
	public interface IDotNetMetricsRepository : IManagerRepository<DotNetMetric>
	{

	}
}
