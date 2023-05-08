﻿namespace CarRentingSystem.Services.Statistics
{
    using CarRentingSystem.Data;

    public class StatisticsService : IStatisticsServices
    {
        private readonly CarRentingDbContext data;

        public StatisticsService(CarRentingDbContext data)
            => this.data = data;

        public StatisticsServiceModel Total()
        {
            var totalCars = this.data.Cars.Count();
            var totalUsers = this.data.Users.Count();

            return new StatisticsServiceModel
            {
                TotalCars = totalCars,
                TotalUsers = totalUsers
            };
        }
    }
}
