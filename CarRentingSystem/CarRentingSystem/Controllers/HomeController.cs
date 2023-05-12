namespace CarRentingSystem.Controllers
{
    using System.Diagnostics;
    using CarRentingSystem.Data;
    using CarRentingSystem.Models;
    using CarRentingSystem.Models.Home;
    using CarRentingSystem.Services.Statistics;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        private readonly IStatisticsServices statistics;
        private readonly CarRentingDbContext data;

        public HomeController(IStatisticsServices statistics, CarRentingDbContext data)
        {
            this.statistics = statistics;
            this.data = data;
        }

        public IActionResult Index()
        {
            var cars = this.data.Cars
                        .OrderByDescending(x => x.Id)
                        .Take(3)
                        .Select(c => new CarIndexViewModel
                        {
                            Id = c.Id,
                            Brand = c.Brand,
                            Model = c.Model,
                            ImageUrl = c.ImageUrl,
                            Year = c.Year
                        })
                        .ToList();

            var totalStatistics = this.statistics.Total();

            return View(new IndexViewModel
            {
                TotalCars = totalStatistics.TotalCars,
                TotalUsers = totalStatistics.TotalUsers,
                Cars = cars
            });
        }
        public IActionResult Error() => View();
    }
}