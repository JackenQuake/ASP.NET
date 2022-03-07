using MetricsAgent.Controllers;
using MetricsAgent.DAL;
using MetricsAgent.Models;
using Moq;
using System;
using Xunit;

namespace MetricsAgentTests
{
    public class RamMetricsControllerUnitTests
    {
        private RamMetricsController controller;
        private Mock<IRamMetricsRepository> mock;

        public RamMetricsControllerUnitTests()
        {
            mock = new Mock<IRamMetricsRepository>();
            controller = new RamMetricsController(mock.Object, null);
        }

        [Fact]
        public void GetAvailableMetrics_ReturnsOk()
        {
            // ������������� �������� ��������
            // � �������� �����������, ��� � ����������� �������� RamMetric-������
            mock.Setup(repository => repository.Create(It.IsAny<RamMetric>())).Verifiable();

            // ��������� �������� �� �����������
            var fromTime = TimeSpan.FromSeconds(0);
            var toTime = TimeSpan.FromSeconds(100);
            var result = controller.GetAvailableMetrics(fromTime, toTime);

            // ��������� �������� �� ��, ��� ���� ������� ����������
            // �������� ����� Create ����������� � ������ ����� ������� � ���������
            mock.Verify(repository => repository.Create(It.IsAny<RamMetric>()), Times.AtMostOnce());
        }
    }
}
