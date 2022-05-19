using MetricsManager.DAL.Interfaces;
using MetricsManager.Models;

namespace MetricsManager.DAL
{
	/// <summary>
	/// Интерфейс репозитория метрики HDD (свободное пространство на диске C:)
	/// </summary>
	public interface IHddMetricsRepository : IManagerRepository<HddMetric>
	{

	}
}
