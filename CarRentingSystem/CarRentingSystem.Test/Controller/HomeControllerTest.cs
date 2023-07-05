namespace CarRentingSystem.Test.Controller
{
    using Mock;
    using Xunit;
    using Microsoft.AspNetCore.Mvc;
    using CarRentingSystem.Data.Models;
    using CarRentingSystem.Services.Cars;
    using CarRentingSystem.Services.Cars.Models;
    using Controllers;
    using Microsoft.Extensions.Caching.Memory;

    public class HomeControllerTest
    {
        [Fact]
        public void IndexShouldReturnViewWithCorrectModel()
        {
            var data = DatabaseMock.Instance;
            var mapper = MapperMock.Instance;

            var cars = GetCars();
            data.Cars.AddRange(cars);
            data.Users.Add(new User { FullName = "Petar" });
            data.SaveChanges();

            var carService = new CarService(data, mapper);
            var cache = new MemoryCache(new MemoryCacheOptions());
            var homeController = new HomeController(carService, cache);

            var result = homeController.Index();

            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.Model;
            var indexViewModel = Assert.IsType<List<LatestCarServiceModel>>(model);

            Assert.Equal(3, indexViewModel.Count);
        }

        [Fact]
        public void ErrorShouldReturnView()
        {
            var controller = new HomeController(null, null);

            var result = controller.Error();

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        private static IEnumerable<Car> GetCars()
        => Enumerable.Range(0, 10).Select(x => new Car
        {
            Brand = "BMW",
            Model = "M4 coupe",
            Description = "Very fast Bmw m4 coupe",
            ImageUrl = "https://www.driving.co.uk/wp-content/uploads/sites/5/2014/08/BMWM4.jpg"
        });
    }
}
