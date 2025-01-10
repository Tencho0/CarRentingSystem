using CarRentingSystem.Controllers.Api;
using CarRentingSystem.Test.Mock;
using Xunit;

namespace CarRentingSystem.Test.Controllers.Api
{
    public class StatisticsApiControllerTest
    {
        [Fact]
        public async void GetStatisticsShouldReturnTotalStatistics()
        {
            var controller = new StatisticsApiController(StatisticsServiceMock.Instance);

            var result = await controller.GetStatistics();

            Assert.NotNull(result);
            Assert.Equal(5, result.TotalCars);
            Assert.Equal(10, result.TotalUsers);
            Assert.Equal(15, result.TotalRents);
        }
    }
}
