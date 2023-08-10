namespace CarRentingSystem.Services.Dealers
{
    using Data;
    using CarRentingSystem.Data.Models;

    public class DealerService : IDealerService
    {
        private readonly CarRentingDbContext data;

        public DealerService(CarRentingDbContext data) 
            => this.data = data;

        public int IdByUser(string userId)
            => this.data.Dealers
                .Where(d => d.UserId == userId)
                .Select(d => d.Id)
                .FirstOrDefault();

        public bool IsDealer(string userId)
         => this.data.Dealers.Any(d => d.UserId == userId);

        public void CreateDealer(string userId, string name, string phoneNumber)
        {
            var dealerData = new Dealer
            {
                Name = name,
                PhoneNumber = phoneNumber,
                UserId = userId,
            };

            this.data.Add(dealerData);
            this.data.SaveChanges();
        }
    }
}
