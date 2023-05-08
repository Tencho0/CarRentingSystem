namespace CarRentingSystem.Services.Cars
{
    using CarRentingSystem.Models;

    public interface ICarService
    {
        CarQueryServiceModel All(string brand, string searchTerm, CarSorting sorting, int currentPage, int carsPerPage);

        IEnumerable<string> AllCarBrands();
    }
}
