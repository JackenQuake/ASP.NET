using Core;
using MetricsAgent.Models;

namespace MetricsAgent.DAL
{
	/// <summary>
	/// Интерфейс репозитория метрики Network (количество передаваемых дейтаграм по IPv4 в секунду)
	/// </summary>
	public interface INetworkMetricsRepository : IRepository<NetworkMetric>
	{

	}
}
