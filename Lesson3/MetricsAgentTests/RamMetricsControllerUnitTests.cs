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
            // Устанавливаем параметр заглушки
            // В заглушке прописываем, что в репозиторий прилетит RamMetric-объект
            mock.Setup(repository => repository.Create(It.IsAny<RamMetric>())).Verifiable();

            // Выполняем действие на контроллере
            var fromTime = TimeSpan.FromSeconds(0);
            var toTime = TimeSpan.FromSeconds(100);
            var result = controller.GetAvailableMetrics(fromTime, toTime);

            // Проверяем заглушку на то, что пока работал контроллер
            // Вызвался метод Create репозитория с нужным типом объекта в параметре
            mock.Verify(repository => repository.Create(It.IsAny<RamMetric>()), Times.AtMostOnce());
        }
    }
}
