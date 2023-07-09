namespace CarRentingSystem.Test.Routing
{
    using Xunit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;

    using Mock;
    using CarRentingSystem.Controllers;
    using CarRentingSystem.Data.Models;
    using CarRentingSystem.Services.Cars;

    using static Data.Cars;

    public class HomeControllerTest
    {
        [Fact]
        public void IndexRouteShouldBeMapped()
        {
            var homeController = GetHomeController();
            
            var result = homeController.Index();
            
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ErrorRouteShouldBeMapped()
        {
            var homeController = GetHomeController();

            var result = homeController.Error();

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        private static HomeController GetHomeController()
        {
            var data = DatabaseMock.Instance;
            var mapper = MapperMock.Instance;

            var cars = TenPublicCars();
            data.Cars.AddRange(cars);
            data.Users.Add(new User { FullName = "Petar" });
            data.SaveChanges();

            var carService = new CarService(data, mapper);
            var cache = new MemoryCache(new MemoryCacheOptions());
            var homeController = new HomeController(carService, cache);

            return homeController;
        }
    }
}