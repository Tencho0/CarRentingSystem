namespace CarRentingSystem.Test.Controller.Api
{
    using Xunit;
    using CarRentingSystem.Test.Mock;
    using CarRentingSystem.Controllers.Api;

    public class StatisticsApiControllerTest
    {
        [Fact]
        public void GetStatisticsShouldReturnTotalStatistics()
        {
            var controller = new StatisticsApiController(StatisticsServiceMock.Instance);

            var result = controller.GetStatistics();

            Assert.NotNull(result);
            Assert.Equal(5, result.TotalCars);
            Assert.Equal(10, result.TotalUsers);
            Assert.Equal(15, result.TotalRents);
        }
    }
}
