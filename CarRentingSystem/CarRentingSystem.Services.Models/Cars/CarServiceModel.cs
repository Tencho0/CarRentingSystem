namespace CarRentingSystem.Services.Models.Cars
{
    using CarRentingSystem.Data.Models;

    public class CarServiceModel : ICarModel
    {
        public int Id { get; set; }

        public string Brand { get; init; } = null!;

        public string Model { get; init; } = null!;

        public string ImageUrl { get; init; } = null!;

        public int Year { get; init; }

        public string CategoryName { get; set; } = null!;

        public bool IsPublic { get; init; }

        public string? RenterId { get; set; }

        public virtual User? Renter { get; set; }
    }
}
