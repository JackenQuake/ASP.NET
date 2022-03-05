using MetricsAgent.Controllers;
using MetricsAgent.DAL;
using MetricsAgent.Models;
using Moq;
using System;
using Xunit;

namespace MetricsAgentTests
{
    public class DotNetMetricsControllerUnitTests
    {
        private DotNetMetricsController controller;
        private Mock<IDotNetMetricsRepository> mock;

        public DotNetMetricsControllerUnitTests()
        {
            mock = new Mock<IDotNetMetricsRepository>();
            controller = new DotNetMetricsController(mock.Object, null);
        }

        [Fact]
        public void GetErrorCountMetrics_ReturnsOk()
        {
            // Устанавливаем параметр заглушки
            // В заглушке прописываем, что в репозиторий прилетит DotNetMetric-объект
            mock.Setup(repository => repository.Create(It.IsAny<DotNetMetric>())).Verifiable();

            // Выполняем действие на контроллере
            var fromTime = TimeSpan.FromSeconds(0);
            var toTime = TimeSpan.FromSeconds(100);
            var result = controller.GetErrorCountMetrics(fromTime, toTime);

            // Проверяем заглушку на то, что пока работал контроллер
            // Вызвался метод Create репозитория с нужным типом объекта в параметре
            mock.Verify(repository => repository.Create(It.IsAny<DotNetMetric>()), Times.AtMostOnce());
        }
    }
}
