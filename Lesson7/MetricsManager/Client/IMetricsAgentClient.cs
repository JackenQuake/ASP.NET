using MetricsManager.Client.Responses;

namespace MetricsManager.Client
{
	/// <summary>
	/// ��������� �������, �������������� � ������� ��������� �������
	/// </summary>
	public interface IMetricsAgentClient
	{
		/// <summary>
		/// ����������� � ������ ������� CPU (������������� ����������) �� �������� ��������� �������
		/// </summary>
		/// <param name="request">��������� ������� (��������� ��������)</param>
		/// <returns>������ ������, ���������� �� ������</returns>
		AllCpuMetricsApiResponse GetAllCpuMetrics(GetAllCpuMetricsApiRequest request);

		/// <summary>
		/// ����������� � ������ ������� DotNet (���������� ������ .NET) �� �������� ��������� �������
		/// </summary>
		/// <param name="request">��������� ������� (��������� ��������)</param>
		/// <returns>������ ������, ���������� �� ������</returns>
		AllDotNetMetricsApiResponse GetAllDotNetMetrics(GetAllDotNetMetricsApiRequest request);

		/// <summary>
		/// ����������� � ������ ������� HDD (��������� ������������ �� ����� C:) �� �������� ��������� �������
		/// </summary>
		/// <param name="request">��������� ������� (��������� ��������)</param>
		/// <returns>������ ������, ���������� �� ������</returns>
		AllHddMetricsApiResponse GetAllHddMetrics(GetAllHddMetricsApiRequest request);

		/// <summary>
		/// ����������� � ������ ������� Network (���������� ������������ ��������� �� IPv4 � �������) �� �������� ��������� �������
		/// </summary>
		/// <param name="request">��������� ������� (��������� ��������)</param>
		/// <returns>������ ������, ���������� �� ������</returns>
		AllNetworkMetricsApiResponse GetAllNetworkMetrics(GetAllNetworkMetricsApiRequest request);

		/// <summary>
		/// ����������� � ������ ������� RAM (����� ��������� ������) �� �������� ��������� �������
		/// </summary>
		/// <param name="request">��������� ������� (��������� ��������)</param>
		/// <returns>������ ������, ���������� �� ������</returns>
		AllRamMetricsApiResponse GetAllRamMetrics(GetAllRamMetricsApiRequest request);
	}
}
