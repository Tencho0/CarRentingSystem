﻿using CarRentingSystem.Services.Models.Cars;

namespace CarRentingSystem.Test.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;

    using Mock;
    using Xunit;

    using Services.Cars;
    using CarRentingSystem.Controllers;
    using CarRentingSystem.Data.Models;
    using static Data.Cars;

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

        [Fact]
        public void ErrorShouldReturnView()
        {
            var controller = new HomeController(null, null);

            var result = controller.Error();

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void IndexReturnsViewWithLatestCarsFromCache()
        {
            var data = DatabaseMock.Instance;
            var mapper = MapperMock.Instance;

            var cars = TenPublicCars();
            data.Cars.AddRange(cars);
            data.Users.Add(new User { FullName = "Petar" });
            data.SaveChanges();

            var carService = new CarService(data, mapper);
            var cache = new MemoryCache(new MemoryCacheOptions());
            var controller = new HomeController(carService, cache);

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as List<LatestCarServiceModel>;
            Assert.NotNull(model);
            Assert.Equal(3, model.Count);
        }
    }
}
