namespace CarRentingSystem.Controllers
{
    using System.Diagnostics;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using CarRentingSystem.Data;
    using CarRentingSystem.Models;
    using CarRentingSystem.Models.Home;
    using CarRentingSystem.Services.Statistics;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        private readonly IStatisticsServices statistics;
        private readonly CarRentingDbContext data;
        private readonly IMapper mapper;

        public HomeController(IStatisticsServices statistics, CarRentingDbContext data, IMapper mapper)
        {
            this.statistics = statistics;
            this.data = data;
            this.mapper = mapper;
        }

        public IActionResult Index()
        {
            var cars = this.data.Cars
                        .OrderByDescending(x => x.Id)
                        .ProjectTo<CarIndexViewModel>(this.mapper.ConfigurationProvider)
                        .Take(3)
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