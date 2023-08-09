namespace CarRentingSystem.Test.Data
{
    using CarRentingSystem.Data.Models;

    using static TestingConstants;

    public static class Dealers
    {
        public static Dealer OneDealer()
            => new Dealer
            {
                Id = DealerData.Id,
                UserId = UserData.Id,
                Name = DealerData.Name,
                PhoneNumber = DealerData.PhoneNumber
            };
    }
}
