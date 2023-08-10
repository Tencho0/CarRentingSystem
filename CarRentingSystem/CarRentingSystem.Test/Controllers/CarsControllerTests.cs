namespace CarRentingSystem.Test.Controllers
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Mock;
    using Moq;
    using Xunit;

    using ViewModels;
    using ViewModels.Cars;
    using Services.Cars;
    using Services.Dealers;
    using Services.Models.Cars;

    using CarsController = CarRentingSystem.Controllers.CarsController;
    using CarRentingSystem.Controllers;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using CarRentingSystem.Infrastructure.Extensions;

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

        [Fact]
        public void Edit_Get_WithValidId_ShouldReturnViewWithCarFormModel()
        {
            var mapper = MapperMock.Instance;
            var carServiceMock = new Mock<ICarService>();
            var dealerServiceMock = new Mock<IDealerService>();
            var carsController = new CarsController(carServiceMock.Object, dealerServiceMock.Object, mapper);
            var userId = "testUserId";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            carsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
            var carId = 1;

            carServiceMock.Setup(c => c.Details(carId))
                .Returns(new CarDetailsServiceModel
                {
                    Id = carId,
                    Brand = "Toyota",
                    Model = "Camry",
                    Year = 2020,
                    UserId = userId,

                });
            dealerServiceMock.Setup(d => d.IsDealer(It.IsAny<string>())).Returns(true);

            var result = carsController.Edit(carId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CarFormModel>(viewResult.ViewData.Model);
            Assert.NotNull(model);
            Assert.Equal("Toyota", model.Brand);
            Assert.Equal("Camry", model.Model);
            Assert.Equal(2020, model.Year);
        }

        [Fact]
        public void Edit_Get_WithInvalidUserId_ShouldReturnViewWithCarFormModel()
        {
            var mapper = MapperMock.Instance;
            var carServiceMock = new Mock<ICarService>();
            var dealerServiceMock = new Mock<IDealerService>();
            var carsController = new CarsController(carServiceMock.Object, dealerServiceMock.Object, mapper);
            var userId = "testUserId";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            carsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
            var carId = 1;

            carServiceMock.Setup(c => c.Details(carId))
                .Returns(new CarDetailsServiceModel
                {
                    Id = carId,
                    Brand = "Toyota",
                    Model = "Camry",
                    Year = 2020,
                    UserId = userId,

                });
            dealerServiceMock.Setup(d => d.IsDealer(It.IsAny<string>())).Returns(false);

            var result = carsController.Edit(carId) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal(nameof(DealersController.Become), result.ActionName);
            Assert.Equal("Dealers", result.ControllerName);
        }

        [Fact]
        public void Edit_Get_WithInvalidCarId_ShouldReturnBadRequest()
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
            dealerServiceMock.Setup(d => d.IsDealer(It.IsAny<string>())).Returns(true);

            var carId = 1;

            var result = carsController.Edit(carId);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Edit_Get_WithInvalidUserId_ShouldReturnUnauthorized()
        {
            var mapper = MapperMock.Instance;
            var carServiceMock = new Mock<ICarService>();
            var dealerServiceMock = new Mock<IDealerService>();
            var carsController = new CarsController(carServiceMock.Object, dealerServiceMock.Object, mapper);

            var userId = "testUserId";
            var invalidUserId = "invalidUserId";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            carsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var carId = 1;

            carServiceMock.Setup(c => c.Details(carId))
                .Returns(new CarDetailsServiceModel
                {
                    Id = carId,
                    Brand = "Toyota",
                    Model = "Camry",
                    Year = 2020,
                    UserId = invalidUserId,

                });
            dealerServiceMock.Setup(d => d.IsDealer(It.IsAny<string>())).Returns(true);

            var result = carsController.Edit(carId);

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public void Edit_WithValidModel_ShouldUpdateCarAndRedirectToDetails()
        {
            var mapper = MapperMock.Instance;
            var carServiceMock = new Mock<ICarService>();
            var dealerServiceMock = new Mock<IDealerService>();
            var tempDataMock = new Mock<ITempDataDictionary>();  // Create a mock for ITempDataDictionary
            var carsController = new CarsController(carServiceMock.Object, dealerServiceMock.Object, mapper);
            carsController.TempData = tempDataMock.Object;
            var userId = "testUserId";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, "Administrator")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            carsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };


            var carId = 1;
            var dealerId = 1;

            var carFormModel = new CarFormModel
            {
                Brand = "Toyota",
                Model = "Camry",
                Description = "A popular sedan model",
                ImageUrl = "https://example.com/toyota.jpg",
                Year = 2022,
                CategoryId = 1
            };

            dealerServiceMock
                .Setup(c => c.IdByUser(It.IsAny<string>()))
                .Returns(dealerId);
            carServiceMock
                .Setup(c => c.CategoryExists(It.IsAny<int>()))
                .Returns(true);
            carServiceMock
                .Setup(c => c.IsByDealer(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(true);
            carServiceMock
                .Setup(c => c.Edit(carId,
                        carFormModel.Brand,
                        carFormModel.Model,
                        carFormModel.Description,
                        carFormModel.ImageUrl,
                        carFormModel.Year,
                        carFormModel.CategoryId,
                        It.IsAny<bool>()))
                .Returns(true);

            var result = carsController.Edit(carId, carFormModel);

            carServiceMock.Verify(c => c.Edit(
                carId,
                carFormModel.Brand,
                carFormModel.Model,
                carFormModel.Description,
                carFormModel.ImageUrl,
                carFormModel.Year,
                carFormModel.CategoryId,
                true
            ), Times.Once);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);

            object expectedRouteValues = new
            {
                id = carId,
                information = carFormModel.GetInformation()
            };

            // Convert actual RouteValueDictionary to an anonymous object for comparison
            var actualRouteValues = new
            {
                id = (int)redirectResult.RouteValues["id"],
                information = (string)redirectResult.RouteValues["information"]
            };

            Assert.Equal(expectedRouteValues, actualRouteValues);
        }

        [Fact]
        public void Edit_WithInvalidDealerId_ShouldRedirectToBecome()
        {
            var mapper = MapperMock.Instance;
            var carServiceMock = new Mock<ICarService>();
            var dealerServiceMock = new Mock<IDealerService>();
            var carsController = new CarsController(carServiceMock.Object, dealerServiceMock.Object, mapper);

            var userId = "testUserId";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            carsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var carId = 1;
            var invalidDealerId = 0;

            var carFormModel = new CarFormModel
            {
                Brand = "Toyota",
                Model = "Camry",
                Description = "A popular sedan model",
                ImageUrl = "https://example.com/toyota.jpg",
                Year = 2022,
                CategoryId = 1
            };

            dealerServiceMock
                .Setup(c => c.IdByUser(It.IsAny<string>()))
                .Returns(invalidDealerId);

            var result = carsController.Edit(carId, carFormModel);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Become", redirectResult.ActionName);
            Assert.Equal("Dealers", redirectResult.ControllerName);


        }

        [Fact]
        public void Edit_WithInvalidCarCategory_ShouldAddModelError()
        {
            var mapper = MapperMock.Instance;
            var carServiceMock = new Mock<ICarService>();
            var dealerServiceMock = new Mock<IDealerService>();
            var carsController = new CarsController(carServiceMock.Object, dealerServiceMock.Object, mapper);

            var userId = "testUserId";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            carsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var carId = 1;
            var dealerId = 1;

            var carFormModel = new CarFormModel
            {
                Brand = "Toyota",
                Model = "Camry",
                Description = "A popular sedan model",
                ImageUrl = "https://example.com/toyota.jpg",
                Year = 2022,
                CategoryId = 1
            };

            carServiceMock
                .Setup(c => c.CategoryExists(carFormModel.CategoryId))
                .Returns(false);

            dealerServiceMock
                .Setup(c => c.IdByUser(It.IsAny<string>()))
                .Returns(dealerId);

            var result = carsController.Edit(carId, carFormModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.True(viewResult.ViewData.ModelState.ContainsKey(nameof(carFormModel.CategoryId)));

            var modelStateEntry = viewResult.ViewData.ModelState[nameof(carFormModel.CategoryId)];
            Assert.Equal("Category does not exist!", modelStateEntry!.Errors[0].ErrorMessage);
        }

        [Fact]
        public void Edit_WithInvalidModel_ShouldReturnViewWithModel()
        {
            var mapper = MapperMock.Instance;
            var carServiceMock = new Mock<ICarService>();
            var dealerServiceMock = new Mock<IDealerService>();
            var carsController = new CarsController(carServiceMock.Object, dealerServiceMock.Object, mapper);

            var userId = "testUserId";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            carsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var carId = 1;
            var dealerId = 1;

            var carFormModel = new CarFormModel
            {
                Brand = "Toyota",
                Model = "Camry",
                Description = "A popular sedan model",
                ImageUrl = "https://example.com/toyota.jpg",
                Year = 2022,
                CategoryId = 1
            };

            carsController.ModelState.AddModelError("SomeKey", "Some error message");

            carServiceMock
                .Setup(c => c.CategoryExists(carFormModel.CategoryId))
                .Returns(true);

            dealerServiceMock
                .Setup(c => c.IdByUser(It.IsAny<string>()))
                .Returns(dealerId);

            var result = carsController.Edit(carId, carFormModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CarFormModel>(viewResult.ViewData.Model);
            Assert.Equal(carFormModel, model);
        }

        [Fact]
        public void Edit_WithInvalidCarId_ShouldReturnBadRequest()
        {
            var mapper = MapperMock.Instance;
            var carServiceMock = new Mock<ICarService>();
            var dealerServiceMock = new Mock<IDealerService>();
            var carsController = new CarsController(carServiceMock.Object, dealerServiceMock.Object, mapper);

            var carId = 1;
            var dealerId = 1;
            var userId = "testUserId";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            carsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
            carServiceMock
                .Setup(c => c.CategoryExists(It.IsAny<int>()))
                .Returns(true);

            dealerServiceMock
                .Setup(c => c.IdByUser(It.IsAny<string>()))
                .Returns(dealerId);

            var carFormModel = new CarFormModel
            {
                Brand = "Toyota",
                Model = "Camry",
                Description = "A popular sedan model",
                ImageUrl = "https://example.com/toyota.jpg",
                Year = 2022,
                CategoryId = 1
            };

            var result = carsController.Edit(carId, carFormModel);

            Assert.IsType<BadRequestResult>(result);
        }
    }
}
