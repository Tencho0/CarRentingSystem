namespace CarRentingSystem.Test.Controllers
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    using Mock;
    using Moq;
    using Xunit;

    using Common;
    using ViewModels;
    using ViewModels.Cars;
    using Services.Cars;
    using Services.Dealers;
    using Services.Models.Cars;
    using Infrastructure.Extensions;
    using CarRentingSystem.Controllers;

    using CarsController = CarRentingSystem.Controllers.CarsController;
    using static Common.WebConstants.NotificationMessagesConstants;
    using AutoMapper;

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
                .Setup(s => s.AllAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CarSorting>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync(queryResult);

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

            carServiceMock.Setup(c => c.DetailsAsync(carId))
                .ReturnsAsync(new CarDetailsServiceModel
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

            carServiceMock.Setup(c => c.DetailsAsync(carId))
                .ReturnsAsync(new CarDetailsServiceModel
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

            dealerServiceMock.Setup(d => d.IsDealerAsync("testUserId")).ReturnsAsync(true);

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

            dealerServiceMock.Setup(d => d.IsDealerAsync(It.IsAny<string>())).ReturnsAsync(false);

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

            carServiceMock.Setup(c => c.DetailsAsync(carId))
                .ReturnsAsync(new CarDetailsServiceModel
                {
                    Id = carId,
                    Brand = "Toyota",
                    Model = "Camry",
                    Year = 2020,
                    UserId = userId,

                });
            dealerServiceMock.Setup(d => d.IsDealerAsync(It.IsAny<string>())).ReturnsAsync(true);

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
        public async void Edit_Get_WithInvalidUserId_ShouldReturnViewWithCarFormModel()
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

            carServiceMock.Setup(c => c.DetailsAsync(carId))
                .ReturnsAsync(new CarDetailsServiceModel
                {
                    Id = carId,
                    Brand = "Toyota",
                    Model = "Camry",
                    Year = 2020,
                    UserId = userId,

                });
            dealerServiceMock.Setup(d => d.IsDealerAsync(It.IsAny<string>())).ReturnsAsync(false);

            var result = await carsController.Edit(carId) as RedirectToActionResult;

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
            dealerServiceMock.Setup(d => d.IsDealerAsync(It.IsAny<string>())).ReturnsAsync(true);

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

            carServiceMock.Setup(c => c.DetailsAsync(carId))
                .ReturnsAsync(new CarDetailsServiceModel
                {
                    Id = carId,
                    Brand = "Toyota",
                    Model = "Camry",
                    Year = 2020,
                    UserId = invalidUserId,

                });
            dealerServiceMock.Setup(d => d.IsDealerAsync(It.IsAny<string>())).ReturnsAsync(true);

            var result = carsController.Edit(carId);

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public void Edit_WithValidModel_ShouldUpdateCarAndRedirectToDetails()
        {
            var mapper = MapperMock.Instance;
            var carServiceMock = new Mock<ICarService>();
            var dealerServiceMock = new Mock<IDealerService>();

            var carsController = new CarsController(carServiceMock.Object, dealerServiceMock.Object, mapper);
            carsController.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

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
                .Setup(c => c.IdByUserAsync(It.IsAny<string>()))
                .ReturnsAsync(dealerId);
            carServiceMock
                .Setup(c => c.CategoryExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(true);
            carServiceMock
                .Setup(c => c.IsByDealerAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);
            carServiceMock
                .Setup(c => c.EditAsync(carId,
                        carFormModel.Brand,
                        carFormModel.Model,
                        carFormModel.Description,
                        carFormModel.ImageUrl,
                        carFormModel.Year,
                        carFormModel.CategoryId,
                        It.IsAny<bool>()))
                .ReturnsAsync(true);

            var result = carsController.Edit(carId, carFormModel);

            carServiceMock.Verify(c => c.EditAsync(
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
            Assert.Equal("Your car was edited successfully!",
                carsController.TempData[WebConstants.GlobalMessageKey]);

            object expectedRouteValues = new
            {
                id = carId,
                information = carFormModel.GetInformation()
            };

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
                .Setup(c => c.IdByUserAsync(It.IsAny<string>()))
                .ReturnsAsync(invalidDealerId);

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
                .Setup(c => c.CategoryExistsAsync(carFormModel.CategoryId))
                .ReturnsAsync(false);

            dealerServiceMock
                .Setup(c => c.IdByUserAsync(It.IsAny<string>()))
                .ReturnsAsync(dealerId);

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
                .Setup(c => c.CategoryExistsAsync(carFormModel.CategoryId))
                .ReturnsAsync(true);

            dealerServiceMock
                .Setup(c => c.IdByUserAsync(It.IsAny<string>()))
                .ReturnsAsync(dealerId);

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
                .Setup(c => c.CategoryExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            dealerServiceMock
                .Setup(c => c.IdByUserAsync(It.IsAny<string>()))
                .ReturnsAsync(dealerId);

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

        [Fact]
        public void Delete_WithValidCarId_ShouldReturnViewWithModel()
        {
            var carServiceMock = new Mock<ICarService>();
            var dealerServiceMock = new Mock<IDealerService>();
            var mapper = MapperMock.Instance;
            var carsController = new CarsController(carServiceMock.Object, dealerServiceMock.Object, mapper);

            var carId = 1;
            var dealerId = 1;

            carServiceMock
                .Setup(c => c.ExistsByIdAsync(carId))
                .ReturnsAsync(true);
            dealerServiceMock
                .Setup(d => d.IsDealerAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            dealerServiceMock
                .Setup(d => d.IdByUserAsync(It.IsAny<string>()))
                .ReturnsAsync(dealerId);
            carServiceMock
                .Setup(c => c.IsByDealerAsync(carId, dealerId))
                .ReturnsAsync(true);
            carServiceMock
                .Setup(c => c.GetCarForDeleteByIdAsync(carId))
                .ReturnsAsync(new CarDetailsServiceModel());

            carsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "testUserId")
                    }, "TestAuthType"))
                }
            };

            var result = carsController.Delete(carId);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public void Delete_WithNonExistingCarId_ShouldRedirectToAllCars()
        {
            var carServiceMock = new Mock<ICarService>();
            var dealerServiceMock = new Mock<IDealerService>();
            var mapper = MapperMock.Instance;

            var carsController = new CarsController(carServiceMock.Object, dealerServiceMock.Object, mapper);
            carsController.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            carServiceMock.Setup(c => c.ExistsByIdAsync(It.IsAny<int>())).ReturnsAsync(false);

            var result = carsController.Delete(1);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("All", redirectResult.ActionName);
            Assert.Equal("Cars", redirectResult.ControllerName);
            Assert.Equal("Car with the provided Id does not exist!",
                carsController.TempData[ErrorMessage]);
        }

        [Fact]
        public void Delete_WithoutDealerOrAdminRole_ShouldRedirectToBecomeDealer()
        {
            var carServiceMock = new Mock<ICarService>();
            var dealerServiceMock = new Mock<IDealerService>();
            var mapper = MapperMock.Instance;

            var carsController = new CarsController(carServiceMock.Object, dealerServiceMock.Object, mapper);
            carsController.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            carServiceMock.Setup(c => c.ExistsByIdAsync(It.IsAny<int>())).ReturnsAsync(true);
            dealerServiceMock.Setup(d => d.IsDealerAsync(It.IsAny<string>())).ReturnsAsync(false);

            carsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "testUserId")
                    }, "TestAuthType"))
                }
            };

            var result = carsController.Delete(1);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Become", redirectResult.ActionName);
            Assert.Equal("Dealers", redirectResult.ControllerName);
            Assert.Equal("You must become a Dealer in order to edit or delete car info!",
                carsController.TempData[ErrorMessage]);
        }
        
        [Fact]
        public void Delete_WithAdminRole_ShouldReturnViewWithCarForDelete()
        {
            var carServiceMock = new Mock<ICarService>();
            var dealerServiceMock = new Mock<IDealerService>();
            var mapper = MapperMock.Instance;

            var carsController = new CarsController(carServiceMock.Object, dealerServiceMock.Object, mapper);
            carsController.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            carServiceMock.Setup(c => c.ExistsByIdAsync(It.IsAny<int>())).ReturnsAsync(true);
            carServiceMock.Setup(c => c.GetCarForDeleteByIdAsync(It.IsAny<int>())).ReturnsAsync(new CarDetailsServiceModel());

            carsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "testUserId"),
                        new Claim(ClaimTypes.Role, "Administrator")
                    }, "TestAuthType"))
                }
            };
            
            var result = carsController.Delete(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<CarDetailsServiceModel>(viewResult.Model);
        }

        [Fact]
        public void Delete_WithException_ShouldReturnGeneralErrorView()
        {
            var carServiceMock = new Mock<ICarService>();
            var dealerServiceMock = new Mock<IDealerService>();
            var mapper = MapperMock.Instance;

            var carsController = new CarsController(carServiceMock.Object, dealerServiceMock.Object, mapper);
            carsController.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            carServiceMock
                .Setup(c => c.ExistsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);
            carServiceMock
                .Setup(c => c.IsByDealerAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);
            dealerServiceMock
                .Setup(d => d.IsDealerAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            carServiceMock
                .Setup(c => c.GetCarForDeleteByIdAsync(It.IsAny<int>()))
                .Throws(new Exception("Test Exception"));
            carsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "testUserId")
                    }, "TestAuthType"))
                }
            };

            var result = carsController.Delete(1);
            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }
    }
}
