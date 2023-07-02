namespace CarRentingSystem.Test.Services
{
    using Xunit;

    using CarRentingSystem.Data.Models;
    using CarRentingSystem.Services.Dealers;
    using CarRentingSystem.Test.Mock;

    public class DealerServiceTest
    {
        private const string UserId = "TestUserId";

        [Fact]
        public void IsDealerShouldReturnTrueIfUserIsDealer()
        {
            var dealerService = GetDealerService();

            var result = dealerService.IsDealer(UserId);

            Assert.True(result);
        }

        [Fact]
        public void IsDealerShouldReturnFalseIfUserIsNotDealer()
        {
            var dealerService = GetDealerService();

            var result = dealerService.IsDealer("WrongUserId");

            Assert.False(result);
        }

        private static IDealerService GetDealerService()
        {
            var data = DatabaseMock.Instance;

            data.Dealers.Add(new Dealer
            {
                UserId = UserId,
                Name = "TestName",
                PhoneNumber = "+359 987654321"
            });
            data.SaveChanges();

            return new DealerService(data);
        }
    }
}
