using Core;
using MetricsAgent.Models;

namespace MetricsAgent.DAL
{
	/// <summary>
	/// Интерфейс репозитория метрики CPU (использования процессора)
	/// </summary>
	public interface ICpuMetricsRepository : IRepository<CpuMetric>
	{

	}
}
