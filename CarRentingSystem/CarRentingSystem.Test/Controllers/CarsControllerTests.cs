using CarRentingSystem.Controllers;
using CarRentingSystem.Services.Cars;
using CarRentingSystem.Models.Cars;
using CarRentingSystem.Services.Dealers;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using CarRentingSystem.Models;
using CarRentingSystem.Services.Cars.Models;
using CarRentingSystem.Test.Mock;
using MyTested.AspNetCore.Mvc;
using CarRentingSystem.Data.Models;
using AutoMapper;
using CarRentingSystem.Areas.Admin.Controller;
using CarsController = CarRentingSystem.Controllers.CarsController;

namespace CarRentingSystem.Test.Controllers
{
    public class CarsControllerTests
    {
        [Fact]
        public void AllShouldReturnViewWithQueryResult()
        {
            // Arrange
            var mapper = MapperMock.Instance;
            var carServiceMock = new Mock<ICarService>();
            var dealerServiceMock = new Mock<IDealerService>();
            var carsController = new CarsController(carServiceMock.Object, dealerServiceMock.Object, mapper);
            var queryModel = new AllCarsQueryModel
            {
                // Set properties for the queryModel as needed for the test scenario.
            };

            var queryResult = new CarQueryServiceModel
            {
                // Set properties for the queryResult as needed for the test scenario.
            };

            carServiceMock
                .Setup(s => s.All(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CarSorting>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(queryResult);

            // Act
            var result = carsController.All(queryModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AllCarsQueryModel>(viewResult.Model);
            Assert.Equal(queryResult.TotalCars, model.TotalCars);
            // Add more assertions based on the expected behavior of the All action.
        }

        [Fact]
        public void DetailsWithValidDataShouldReturnViewWithCarDetails()
        {
            var mapper = MapperMock.Instance;
            var carServiceMock = new Mock<ICarService>();
            var dealerServiceMock = new Mock<IDealerService>();
            var carsController = new CarsController(carServiceMock.Object, dealerServiceMock.Object, mapper);
            var carId = 1;

            carServiceMock.Setup(c => c.Details(carId))
                .Returns(new CarDetailsServiceModel
                {
                    Id = carId,
                    Brand = "Toyota",
                    Model = "Camry",
                    Year = 2020
                });

            var result = carsController.Details(carId, "Toyota-Camry-2020");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CarDetailsServiceModel>(viewResult.ViewData.Model);
            Assert.NotNull(model);
            Assert.Equal(carId, model.Id);
            Assert.Equal("Toyota", model.Brand);
            Assert.Equal("Camry", model.Model);
        }

        [Fact]
        public void DetailsWithInvalidIdShouldReturnBadRequest()
        {
            var mapper = MapperMock.Instance;
            var carServiceMock = new Mock<ICarService>();
            var dealerServiceMock = new Mock<IDealerService>();
            var carsController = new CarsController(carServiceMock.Object, dealerServiceMock.Object, mapper);
            var invalidCarId = 1;

            var result = carsController.Details(invalidCarId, "SomeInformation");

            var badRequestResult = Assert.IsType<BadRequestResult>(result);
            Assert.NotNull(badRequestResult);
        }

        [Fact]
        public void DetailsWithInvalidInformationShouldReturnBadRequest()
        {
            var mapper = MapperMock.Instance;
            var carServiceMock = new Mock<ICarService>();
            var dealerServiceMock = new Mock<IDealerService>();
            var carsController = new CarsController(carServiceMock.Object, dealerServiceMock.Object, mapper);
            var carId = 1;
            var information = "Invalid-Information";

            carServiceMock.Setup(c => c.Details(carId))
                .Returns(new CarDetailsServiceModel
                {
                    Id = carId,
                    Brand = "Toyota",
                    Model = "Camry",
                    Year = 2020
                });

            var result = carsController.Details(carId, information);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void AddGetShouldReturnViewWithCarFormModel()
        {
            var mapper = MapperMock.Instance;
            var carServiceMock = new Mock<ICarService>();
            var dealerServiceMock = new Mock<IDealerService>();
            var carsController = new CarsController(carServiceMock.Object, dealerServiceMock.Object, mapper);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "testUserId")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            carsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            dealerServiceMock.Setup(d => d.IsDealer("testUserId")).Returns(true);

            var result = carsController.Add();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CarFormModel>(viewResult.ViewData.Model);
            Assert.NotNull(model);
        }

        [Fact]
        public void AddGetWithoutDealerShouldRedirectToBecomeDealer()
        {
            var mapper = MapperMock.Instance;
            var carServiceMock = new Mock<ICarService>();
            var dealerServiceMock = new Mock<IDealerService>();
            var carsController = new CarsController(carServiceMock.Object, dealerServiceMock.Object, mapper);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "testUserId")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            carsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            dealerServiceMock.Setup(d => d.IsDealer(It.IsAny<string>())).Returns(false);

            var result = carsController.Add();

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Become", redirectResult.ActionName);
            Assert.Equal("Dealers", redirectResult.ControllerName);
        }

        //[Fact]
        //public void AddPostWithValidModelShouldCreateCarAndRedirectToDetails()
        //{
        //    var carServiceMock = new Mock<ICarService>();
        //    var dealerServiceMock = new Mock<IDealerService>();
        //    var mapperMock = new Mock<IMapper>();

        //    var carFormModel = new CarFormModel
        //    {
        //        Brand = "TestBrand",
        //        Model = "TestModel",
        //        Description = "TestDescription",
        //        ImageUrl = "test.jpg",
        //        Year = 2023,
        //        CategoryId = 1,
        //        Categories = new List<CarCategoryServiceModel>(),
        //    };

        //    var controller = new CarsController(carServiceMock.Object, dealerServiceMock.Object, mapperMock.Object);

        //    // Set up the ControllerContext with a mock HttpContext
        //    var httpContext = new DefaultHttpContext();
        //    httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, "testUserId")
        //    }));

        //    controller.ControllerContext = new ControllerContext
        //    {
        //        HttpContext = httpContext
        //    };

        //    dealerServiceMock.Setup(d => d.IdByUser(It.IsAny<string>())).Returns(1);
        //    carServiceMock.Setup(c => c.CategoryExists(It.IsAny<int>())).Returns(true);
        //    carServiceMock.Setup(c => c.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(1);


        //    var result = controller.Add(carFormModel);

        //    // Assert
        //    carServiceMock.Verify(c => c.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);

        //    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Details", redirectResult.ActionName);
        //    Assert.Equal("Cars", redirectResult.ControllerName);

        //    // Check TempData content
        //    //Assert.True(controller.TempData.ContainsKey(GlobalMessageKey));
        //    //Assert.Equal("Your car was added successfully and is awaiting for approval!", controller.TempData[GlobalMessageKey]);


        //    // Act
        //    //var result = controller.Add(carFormModel) as RedirectToActionResult;

        //    //carServiceMock.Verify(c => c.Create(
        //    //    carFormModel.Brand,
        //    //    carFormModel.Model,
        //    //    carFormModel.Description,
        //    //    carFormModel.ImageUrl,
        //    //    carFormModel.Year,
        //    //    carFormModel.CategoryId,
        //    //    1), Times.Once);

        //    //Assert.NotNull(result);
        //    //Assert.Equal("Details", result.ActionName);
        //    //Assert.Equal(1, result.RouteValues["id"]);
        //}





        //// Add more test methods for other actions in the CarsController.
        //// You can mock the dependencies (carServiceMock and dealerServiceMock) and set up their behavior
        //// based on the scenarios you want to test.

        //// Example test method for the Add action:
        //[Fact]
        //public void Add_Post_ShouldRedirectToDetails_WhenModelStateIsValid()
        //{
        //    // Arrange
        //    var carServiceMock = new Mock<ICarService>();
        //    var dealerServiceMock = new Mock<IDealerService>();
        //    var carsController = new CarsController(carServiceMock.Object, dealerServiceMock.Object);
        //    var carFormModel = new CarFormModel
        //    {
        //        // Set properties for the carFormModel as needed for the test scenario.
        //    };

        //    var userId = "user123"; // Set the user ID to be used in the test.

        //    // Set up HttpContext with the User containing the required claims.
        //    carsController.ControllerContext = new ControllerContext
        //    {
        //        HttpContext = new DefaultHttpContext
        //        {
        //            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        //            {
        //                new Claim(ClaimTypes.NameIdentifier, userId) // This sets the user ID claim.
        //                // Add other claims as needed.
        //            }, "mock"))
        //        }
        //    };

        //    // Act
        //    var result = carsController.Add(carFormModel);

        //    // Assert
        //    var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Details", redirectToActionResult.ActionName);
        //    // Add more assertions based on the expected behavior of the Add action.
        //}
    }
}
