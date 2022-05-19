using MetricsManager.Client.Responses;

namespace MetricsManager.Client
{
	/// <summary>
	/// »нтерфейс клиента, запрашивающего у агентов собранные метрики
	/// </summary>
	public interface IMetricsAgentClient
	{
		/// <summary>
		/// «апрашивает у агента метрики CPU (использовани€ процессора) на заданном диапазоне времени
		/// </summary>
		/// <param name="request">ѕараметры запроса (временной интервал)</param>
		/// <returns>—писок метрик, полученных от агента</returns>
		AllCpuMetricsApiResponse GetAllCpuMetrics(GetAllCpuMetricsApiRequest request);

		/// <summary>
		/// «апрашивает у агента метрики DotNet (количество ошибок .NET) на заданном диапазоне времени
		/// </summary>
		/// <param name="request">ѕараметры запроса (временной интервал)</param>
		/// <returns>—писок метрик, полученных от агента</returns>
		AllDotNetMetricsApiResponse GetAllDotNetMetrics(GetAllDotNetMetricsApiRequest request);

		/// <summary>
		/// «апрашивает у агента метрики HDD (свободное пространство на диске C:) на заданном диапазоне времени
		/// </summary>
		/// <param name="request">ѕараметры запроса (временной интервал)</param>
		/// <returns>—писок метрик, полученных от агента</returns>
		AllHddMetricsApiResponse GetAllHddMetrics(GetAllHddMetricsApiRequest request);

		/// <summary>
		/// «апрашивает у агента метрики Network (количество передаваемых дейтаграм по IPv4 в секунду) на заданном диапазоне времени
		/// </summary>
		/// <param name="request">ѕараметры запроса (временной интервал)</param>
		/// <returns>—писок метрик, полученных от агента</returns>
		AllNetworkMetricsApiResponse GetAllNetworkMetrics(GetAllNetworkMetricsApiRequest request);

		/// <summary>
		/// «апрашивает у агента метрики RAM (объем свободной пам€ти) на заданном диапазоне времени
		/// </summary>
		/// <param name="request">ѕараметры запроса (временной интервал)</param>
		/// <returns>—писок метрик, полученных от агента</returns>
		AllRamMetricsApiResponse GetAllRamMetrics(GetAllRamMetricsApiRequest request);
	}
}
