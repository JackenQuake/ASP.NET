using Core;
using MetricsAgent.Models;

namespace MetricsAgent.DAL
{
	/// <summary>
	/// Интерфейс репозитория метрики RAM (объем свободной памяти)
	/// </summary>
	public interface IRamMetricsRepository : IRepository<RamMetric>
	{

	}
}
