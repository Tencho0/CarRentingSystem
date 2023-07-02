namespace CarRentingSystem.Test.Controller
{
    using CarRentingSystem.Data.Models;
    using CarRentingSystem.Services.Cars;
    using CarRentingSystem.Services.Statistics;
    using Models.Home;
    using Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Mock;
    using Xunit;

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
            var statisticsService = new StatisticsService(data);
            var homeController = new HomeController(carService, statisticsService);

            var result = homeController.Index();

            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.Model;
            var indexViewModel = Assert.IsType<IndexViewModel>(model);

            Assert.Equal(3, indexViewModel.Cars.Count);
            Assert.Equal(10, indexViewModel.TotalCars);
            Assert.Equal(1, indexViewModel.TotalUsers);
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
