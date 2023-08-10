namespace CarRentingSystem.ViewModels.Cars
{
    using System.ComponentModel.DataAnnotations;

    using CarRentingSystem.Services.Models.Cars;

    public class AllCarsQueryModel
    {
        public const int CarsPerPage = 3;

        public string Brand { get; init; }

        [Display(Name = "Search by text")]
        public string SearchTerm { get; init; }

        public int CurrentPage { get; init; } = 1;

        public int TotalCars { get; set; }

        public CarSorting Sorting { get; init; }

        public IEnumerable<string> Brands { get; set; }

        public IEnumerable<CarServiceModel> Cars { get; set; }
    }
}
