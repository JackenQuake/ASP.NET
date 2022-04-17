using MetricsManager.DAL.Interfaces;
using MetricsManager.Models;

namespace MetricsManager.DAL
{
	/// <summary>
	/// Интерфейс репозитория метрики CPU (использования процессора)
	/// </summary>
	public interface ICpuMetricsRepository : IManagerRepository<CpuMetric>
	{

	}
}
