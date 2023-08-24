namespace CarRentingSystem.Services.Dealers
{
    using Microsoft.EntityFrameworkCore;

    using Data;
    using CarRentingSystem.Data.Models;

    public class DealerService : IDealerService
    {
        private readonly CarRentingDbContext data;

        public DealerService(CarRentingDbContext data)
            => this.data = data;

        public async Task<int> IdByUserAsync(string userId)
            => await this.data.Dealers
                .Where(d => d.UserId == userId)
                .Select(d => d.Id)
                .FirstOrDefaultAsync();

        public async Task<bool> IsDealerAsync(string userId)
         => await this.data.Dealers.AnyAsync(d => d.UserId == userId);

        public async Task CreateDealerAsync(string userId, string name, string phoneNumber)
        {
            var dealerData = new Dealer
            {
                Name = name,
                PhoneNumber = phoneNumber,
                UserId = userId,
            };

            await this.data.AddAsync(dealerData);
            await this.data.SaveChangesAsync();
        }
    }
}
