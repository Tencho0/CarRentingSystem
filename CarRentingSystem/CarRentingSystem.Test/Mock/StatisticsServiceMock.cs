namespace CarRentingSystem.Test.Mock
{
    using Moq;
    using CarRentingSystem.Services.Statistics;

    public class StatisticsServiceMock
    {
        public static IStatisticsService Instance
        {
            get
            {
                var statisticsServiceMock = new Mock<IStatisticsService>();

                statisticsServiceMock
                    .Setup(s => s.TotalAsync())
                    .ReturnsAsync(new StatisticsServiceModel
                    {
                        TotalCars = 5,
                        TotalUsers = 10,
                        TotalRents = 15
                    });
                return statisticsServiceMock.Object;
            }
        }
    }
}
