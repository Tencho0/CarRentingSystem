namespace CarRentingSystem.Services.Statistics
{
    public interface IStatisticsService
    {
        Task<StatisticsServiceModel> TotalAsync();
    }
}
