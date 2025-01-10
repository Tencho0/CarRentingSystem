namespace CarRentingSystem.Services.Dealers
{
    public interface IDealerService
    {
        Task<bool> IsDealerAsync(string userId);

        Task<int> IdByUserAsync(string userId);

        Task CreateDealerAsync(string userId, string name, string phoneNumber);
    }
}
