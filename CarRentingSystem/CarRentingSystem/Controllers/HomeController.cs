namespace CarRentingSystem.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;

    using Services.Cars;
    using Services.Models.Cars;

    using static Common.WebConstants.Cache;

    public class HomeController : Controller
    {
        private readonly ICarService cars;
        private readonly IMemoryCache cache;

        public HomeController(ICarService cars, IMemoryCache cache)
        {
            this.cars = cars;
            this.cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            var latestCars = await this.cache.GetOrCreateAsync(LatestCarsCacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

                var latestCarsData = await this.cars.LatestAsync();

                return latestCarsData.ToList();
            });

            return View(latestCars);
        }

        public IActionResult Error() => View();
    }
}