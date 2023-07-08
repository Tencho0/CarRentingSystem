namespace CarRentingSystem.Test.Controller
{
    using Xunit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using MyTested.AspNetCore.Mvc;

    using Mock;
    using Controllers;
    using CarRentingSystem.Data.Models;
    using CarRentingSystem.Services.Cars;
    using CarRentingSystem.Services.Cars.Models;

    using static Data.Cars;
    using AutoMapper;

    public class HomeControllerTest
    {
        [Fact]
        public void IndexShouldReturnViewWithCorrectModel()
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

            var result = homeController.Index();

            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.Model;
            var indexViewModel = Assert.IsType<List<LatestCarServiceModel>>(model);

            Assert.Equal(3, indexViewModel.Count);
        }

        //[Fact]
        //public void ErrorShouldReturnView2()
        //=> MyMvc
        //    .Pipeline()
        //    .ShouldMap("/Home/Error")
        //    .To<HomeController>(c => c.Error())
        //    .Which()
        //    .ShouldReturn()
        //    .View();

        [Fact]
        public void ErrorShouldReturnView()
        {
            var controller = new HomeController(null, null);

            var result = controller.Error();

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }
    }
}
