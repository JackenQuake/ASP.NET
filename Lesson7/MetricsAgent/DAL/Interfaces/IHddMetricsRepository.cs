using Core;
using MetricsAgent.Models;

namespace MetricsAgent.DAL
{
	/// <summary>
	/// Интерфейс репозитория метрики HDD (свободное пространство на диске C:)
	/// </summary>
	public interface IHddMetricsRepository : IRepository<HddMetric>
	{

	}
}
