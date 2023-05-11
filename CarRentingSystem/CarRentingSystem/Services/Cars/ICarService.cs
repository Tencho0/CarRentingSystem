namespace CarRentingSystem.Services.Cars
{
    using CarRentingSystem.Models;

    public interface ICarService
    {
        CarQueryServiceModel All(string brand, string searchTerm, CarSorting sorting, int currentPage, int carsPerPage);

        CarDetailsServiceModel Details(int id);

        IEnumerable<CarServiceModel> ByUser(string userId);

        IEnumerable<string> AllBrands();

        IEnumerable<CarCategoryServiceModel> AllCategories();
    }
}
