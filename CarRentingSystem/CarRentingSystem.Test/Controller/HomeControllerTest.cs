namespace CarRentingSystem.Test.Controller
{
    using Xunit;
    using Microsoft.AspNetCore.Mvc;

    using CarRentingSystem.Controllers;

    public class HomeControllerTest
    {
        [Fact]
        public void ErrorShouldReturnView()
        {
            var controller = new HomeController(null, null, null);

            var result = controller.Error();

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }
    }
}
