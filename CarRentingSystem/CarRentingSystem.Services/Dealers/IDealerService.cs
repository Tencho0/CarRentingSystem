namespace CarRentingSystem.Services.Dealers
{
    public interface IDealerService
    {
        public bool IsDealer(string userId);

        public int IdByUser(string userId);

        public void CreateDealer(string userId, string name, string phoneNumber);
    }
}
