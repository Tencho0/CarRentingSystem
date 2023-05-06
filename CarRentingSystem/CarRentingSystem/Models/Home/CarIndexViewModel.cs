namespace CarRentingSystem.Models.Home
{
    public class CarIndexViewModel
    {
        public int Id { get; set; }

        public string Brand { get; init; } = null!;

        public string Model { get; init; } = null!;

        public string ImageUrl { get; init; } = null!;

        public int Year { get; init; }
    }
}
