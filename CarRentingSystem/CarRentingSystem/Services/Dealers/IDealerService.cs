namespace CarRentingSystem.Services.Dealers
{
    public interface IDealerService
    {
        public bool IsDealer(string userId);

        public int GetIdByUser(string userId);
    }
}
