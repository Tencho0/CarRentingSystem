namespace CarRentingSystem.Services.Statistics
{
    using Data;
    using Microsoft.EntityFrameworkCore;

    public class StatisticsService : IStatisticsService
    {
        private readonly CarRentingDbContext data;

        public StatisticsService(CarRentingDbContext data)
            => this.data = data;

        public async Task<StatisticsServiceModel> TotalAsync()
        {
            var totalCars = await this.data.Cars.CountAsync(c => c.IsPublic);
            var totalUsers = await this.data.Users.CountAsync();

            return new StatisticsServiceModel
            {
                TotalCars = totalCars,
                TotalUsers = totalUsers
            };
        }
    }
}
