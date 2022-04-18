using MetricsManager.DAL.Interfaces;
using MetricsManager.Models;

namespace MetricsManager.DAL
{
	/// <summary>
	/// Интерфейс репозитория метрики RAM (объем свободной памяти)
	/// </summary>
	public interface IRamMetricsRepository : IManagerRepository<RamMetric>
	{

	}
}
