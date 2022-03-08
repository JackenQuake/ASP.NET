using MetricsAgent.Controllers;
using MetricsAgent.DAL;
using MetricsAgent.Models;
using Moq;
using System;
using Xunit;

namespace MetricsAgentTests
{
    public class NetworkMetricsControllerUnitTests
    {
        private NetworkMetricsController controller;
        private Mock<INetworkMetricsRepository> mock;

        public NetworkMetricsControllerUnitTests()
        {
            mock = new Mock<INetworkMetricsRepository>();
            controller = new NetworkMetricsController(mock.Object, null, null);
        }

        [Fact]
        public void GetMetrics_ReturnsOk()
        {
            // ������������� �������� ��������
            // � �������� �����������, ��� � ����������� �������� NetworkMetric-������
            mock.Setup(repository => repository.Create(It.IsAny<NetworkMetric>())).Verifiable();

            // ��������� �������� �� �����������
            var fromTime = TimeSpan.FromSeconds(0);
            var toTime = TimeSpan.FromSeconds(100);
            var result = controller.GetMetrics(fromTime, toTime);

            // ��������� �������� �� ��, ��� ���� ������� ����������
            // �������� ����� Create ����������� � ������ ����� ������� � ���������
            mock.Verify(repository => repository.Create(It.IsAny<NetworkMetric>()), Times.AtMostOnce());
        }
    }
}
