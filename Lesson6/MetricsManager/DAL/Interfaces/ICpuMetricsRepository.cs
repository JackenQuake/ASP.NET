using MetricsManager.DAL.Interfaces;
using MetricsManager.Models;

namespace MetricsManager.DAL
{
	// Маркировочный интерфейс
	// используется, чтобы проверять работу репозитория на тесте-заглушке
	public interface ICpuMetricsRepository : IManagerRepository<CpuMetric>
	{

	}
}
