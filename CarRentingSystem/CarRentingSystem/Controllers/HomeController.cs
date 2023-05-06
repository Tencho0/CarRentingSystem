namespace CarRentingSystem.Controllers
{
    using System.Diagnostics;
    using CarRentingSystem.Data;
    using CarRentingSystem.Models;
    using CarRentingSystem.Models.Home;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        private readonly CarRentingDbContext data;

        public HomeController(CarRentingDbContext data) => this.data = data;
        public IActionResult Index()
        {
            var totalCars= this.data.Cars.Count();

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

            return View(new IndexViewModel
            {
                TotalCars = totalCars,
                Cars = cars
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}