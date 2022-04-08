using MetricsManager.Client.Responses;

namespace MetricsManager.Client
{
	public interface IMetricsAgentClient
	{
		AllCpuMetricsApiResponse GetAllCpuMetrics(GetAllCpuMetricsApiRequest request);
		AllDotNetMetricsApiResponse GetAllDotNetMetrics(GetAllDotNetMetricsApiRequest request);
		AllHddMetricsApiResponse GetAllHddMetrics(GetAllHddMetricsApiRequest request);
		AllNetworkMetricsApiResponse GetAllNetworkMetrics(GetAllNetworkMetricsApiRequest request);
		AllRamMetricsApiResponse GetAllRamMetrics(GetAllRamMetricsApiRequest request);
	}
}
