namespace CarRentingSystem.Test.Controller
{
    using Xunit;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Testing;

    public class HomeControllerSystemTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> factory;

        public HomeControllerSystemTest(WebApplicationFactory<Program> factory)
            => this.factory = factory;

        [Fact]
        public async Task IndexShouldReturnCorrectStatusCode()
        {
            var client = this.factory.CreateClient();

            var result = await client.GetAsync("/");

            Assert.True(result.IsSuccessStatusCode);
        }
    }
}