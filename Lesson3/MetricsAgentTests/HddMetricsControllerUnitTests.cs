using MetricsAgent.Controllers;
using MetricsAgent.DAL;
using MetricsAgent.Models;
using Moq;
using System;
using Xunit;

namespace MetricsAgentTests
{
    public class HddMetricsControllerUnitTests
    {
        private HddMetricsController controller;
        private Mock<IHddMetricsRepository> mock;

        public HddMetricsControllerUnitTests()
        {
            mock = new Mock<IHddMetricsRepository>();
            controller = new HddMetricsController(mock.Object, null);
        }

        [Fact]
        public void GetLeftMetrics_ReturnsOk()
        {
            // Устанавливаем параметр заглушки
            // В заглушке прописываем, что в репозиторий прилетит HddMetric-объект
            mock.Setup(repository => repository.Create(It.IsAny<HddMetric>())).Verifiable();

            // Выполняем действие на контроллере
            var fromTime = TimeSpan.FromSeconds(0);
            var toTime = TimeSpan.FromSeconds(100);
            var result = controller.GetLeftMetrics(fromTime, toTime);

            // Проверяем заглушку на то, что пока работал контроллер
            // Вызвался метод Create репозитория с нужным типом объекта в параметре
            mock.Verify(repository => repository.Create(It.IsAny<HddMetric>()), Times.AtMostOnce());
        }
    }
}
