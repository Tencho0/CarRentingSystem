namespace CarRentingSystem.Services.Cars
{
    public class CarDetailsServiceModel : CarServiceModel
    {
        public string Description { get; init; } = null!;

        public int DealerId { get; init; }

        public string DealerName { get; set; }

        public string UserId { get; set; }
    }
}
