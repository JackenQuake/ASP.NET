using MetricsAgent.Controllers;
using MetricsAgent.DAL;
using MetricsAgent.Models;
using Moq;
using System;
using Xunit;

namespace MetricsAgentTests
{
    public class CpuMetricsControllerUnitTests
    {
        private CpuMetricsController controller;
        private Mock<ICpuMetricsRepository> mock;

        public CpuMetricsControllerUnitTests()
        {
            mock = new Mock<ICpuMetricsRepository>();
            controller = new CpuMetricsController(mock.Object, null);
        }

        [Fact]
        public void GetMetrics_ReturnsOk()
        {
            // ������������� �������� ��������
            // � �������� �����������, ��� � ����������� �������� CpuMetric-������
            mock.Setup(repository => repository.Create(It.IsAny<CpuMetric>())).Verifiable();

            // ��������� �������� �� �����������
            var fromTime = TimeSpan.FromSeconds(0);
            var toTime = TimeSpan.FromSeconds(100);
            var result = controller.GetMetrics(fromTime, toTime);

            // ��������� �������� �� ��, ��� ���� ������� ����������
            // �������� ����� Create ����������� � ������ ����� ������� � ���������
            mock.Verify(repository => repository.Create(It.IsAny<CpuMetric>()), Times.AtMostOnce());
        }

        [Fact]
        public void GetMetricsPercentile_ReturnsOk()
        {
            // ������������� �������� ��������
            // � �������� �����������, ��� � ����������� �������� CpuMetric-������
            mock.Setup(repository => repository.Create(It.IsAny<CpuMetric>())).Verifiable();

            // ��������� �������� �� �����������
            var fromTime = TimeSpan.FromSeconds(0);
            var toTime = TimeSpan.FromSeconds(100);
            int Percentile = 50;
            var result = controller.GetMetricsPercentile(fromTime, toTime, Percentile);

            // ��������� �������� �� ��, ��� ���� ������� ����������
            // �������� ����� Create ����������� � ������ ����� ������� � ���������
            mock.Verify(repository => repository.Create(It.IsAny<CpuMetric>()), Times.AtMostOnce());
        }
    }
}
