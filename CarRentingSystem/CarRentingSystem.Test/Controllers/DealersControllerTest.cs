namespace CarRentingSystem.Test.Controllers
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    
    using Moq;
    using Mock;
    using Xunit;
    using Services.Dealers;
    using MyTested.AspNetCore.Mvc;

    using ViewModels.Dealers;
    using CarRentingSystem.Controllers;

    public class DealersControllerTest
    {
        [Fact]
        public void GetBecomeShouldBeForAuthorizedUsersAndReturnView()
        {
            // Arrange

            var data = DatabaseMock.Instance;
            var dealersService = new DealerService(data);
            var controller = new DealersController(dealersService);
            // Act
            var result = controller.Become();

            // Assert
            Assert.NotNull(result);
            Assert.True(controller.GetType().GetMethod(nameof(DealersController.Become), new Type[] { })?.GetCustomAttributes(typeof(AuthorizeAttribute), true).Length > 0);
            Assert.IsType<ViewResult>(result);
        }

        [Theory]
        [InlineData("Dealer", "+359888888888")]
        public async void PostBecomeShouldBeForAuthorizedUsersAndReturnRedirectWithValidModel(
          string dealerName,
          string phoneNumber)
        {
            // Arrange
            var data = DatabaseMock.Instance;
            var dealersService = new DealerService(data);
            var controller = new DealersController(dealersService);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, TestUser.Identifier) }));

            var tempDataMock = new Mock<ITempDataDictionary>();
            controller.TempData = tempDataMock.Object;

            var result = controller.Become(new BecomeDealerFromModel
            {
                Name = dealerName,
                PhoneNumber = phoneNumber
            });

            Assert.NotNull(result);
            Assert.True(controller.GetType().GetMethod(nameof(controller.Become), new Type[] { typeof(BecomeDealerFromModel) })?.GetCustomAttributes(typeof(HttpPostAttribute), true).Length > 0);
            Assert.True(controller.GetType().GetMethod(nameof(controller.Become), new[] { typeof(BecomeDealerFromModel) })?.GetCustomAttributes(typeof(AuthorizeAttribute), true).Length > 0);
            Assert.True(controller.ModelState.IsValid);
            var dealers = data.Dealers.ToList();
            Assert.True(dealers.Any(d =>
                d.Name == dealerName &&
                d.PhoneNumber == phoneNumber &&
                d.UserId == TestUser.Identifier));
            Assert.IsType<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult) await result;
            Assert.Equal("All", redirectResult.ActionName);
            Assert.Equal("Cars", redirectResult.ControllerName);
        }
    }
}