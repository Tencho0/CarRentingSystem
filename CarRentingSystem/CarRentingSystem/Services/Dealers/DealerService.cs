namespace CarRentingSystem.Services.Dealers
{
    using CarRentingSystem.Data;

    public class DealerService : IDealerService
    {
        private readonly CarRentingDbContext data;

        public DealerService(CarRentingDbContext data) 
            => this.data = data;

        public int GetIdByUser(string userId)
            => this.data.Dealers
                .Where(d => d.UserId == userId)
                .Select(d => d.Id)
                .FirstOrDefault();

        public bool IsDealer(string userId)
         => this.data.Dealers.Any(d => d.UserId == userId);
    }
}
