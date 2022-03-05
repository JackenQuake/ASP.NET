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
            // Устанавливаем параметр заглушки
            // В заглушке прописываем, что в репозиторий прилетит CpuMetric-объект
            mock.Setup(repository => repository.Create(It.IsAny<CpuMetric>())).Verifiable();

            // Выполняем действие на контроллере
            var fromTime = TimeSpan.FromSeconds(0);
            var toTime = TimeSpan.FromSeconds(100);
            var result = controller.GetMetrics(fromTime, toTime);

            // Проверяем заглушку на то, что пока работал контроллер
            // Вызвался метод Create репозитория с нужным типом объекта в параметре
            mock.Verify(repository => repository.Create(It.IsAny<CpuMetric>()), Times.AtMostOnce());
        }

        [Fact]
        public void GetMetricsPercentile_ReturnsOk()
        {
            // Устанавливаем параметр заглушки
            // В заглушке прописываем, что в репозиторий прилетит CpuMetric-объект
            mock.Setup(repository => repository.Create(It.IsAny<CpuMetric>())).Verifiable();

            // Выполняем действие на контроллере
            var fromTime = TimeSpan.FromSeconds(0);
            var toTime = TimeSpan.FromSeconds(100);
            int Percentile = 50;
            var result = controller.GetMetricsPercentile(fromTime, toTime, Percentile);

            // Проверяем заглушку на то, что пока работал контроллер
            // Вызвался метод Create репозитория с нужным типом объекта в параметре
            mock.Verify(repository => repository.Create(It.IsAny<CpuMetric>()), Times.AtMostOnce());
        }
    }
}
