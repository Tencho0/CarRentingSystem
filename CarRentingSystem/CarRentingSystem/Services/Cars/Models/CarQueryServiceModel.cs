namespace CarRentingSystem.Services.Cars.Models
{
    using CarRentingSystem.Models.Api.Cars;

    public class CarQueryServiceModel
    {
        public int CurrentPage { get; init; }

        public int TotalCars { get; init; }

        public int CarsPerPage { get; init; }

        public IEnumerable<CarServiceModel> Cars { get; init; }
    }
}
