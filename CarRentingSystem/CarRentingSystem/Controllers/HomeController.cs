namespace CarRentingSystem.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using Models.Home;
    using Services.Cars;
    using Services.Statistics;

    public class HomeController : Controller
    {
        private readonly ICarService cars;
        private readonly IStatisticsService statistics;

        public HomeController(ICarService cars, IStatisticsService statistics)
        {
            this.cars = cars;
            this.statistics = statistics;
        }

        public IActionResult Index()
        {
            var latestCars = this.cars.Latest().ToList();

            var totalStatistics = this.statistics.Total();

            return View(new IndexViewModel
            {
                TotalCars = totalStatistics.TotalCars,
                TotalUsers = totalStatistics.TotalUsers,
                Cars = latestCars
            });
        }
        public IActionResult Error() => View();
    }
}