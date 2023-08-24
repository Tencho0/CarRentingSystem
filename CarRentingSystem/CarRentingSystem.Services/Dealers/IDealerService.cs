namespace CarRentingSystem.Services.Dealers
{
    public interface IDealerService
    {
        public Task<bool> IsDealerAsync(string userId);

        public Task<int> IdByUserAsync(string userId);

        public Task CreateDealerAsync(string userId, string name, string phoneNumber);
    }
}
