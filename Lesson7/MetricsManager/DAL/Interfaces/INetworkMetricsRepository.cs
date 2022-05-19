using MetricsManager.DAL.Interfaces;
using MetricsManager.Models;

namespace MetricsManager.DAL
{
	/// <summary>
	/// Интерфейс репозитория метрики Network (количество передаваемых дейтаграм по IPv4 в секунду)
	/// </summary>
	public interface INetworkMetricsRepository : IManagerRepository<NetworkMetric>
	{

	}
}
