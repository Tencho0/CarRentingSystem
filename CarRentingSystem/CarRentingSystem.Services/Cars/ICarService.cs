namespace CarRentingSystem.Services.Cars
{
    using ViewModels;
    using CarRentingSystem.Services.Models.Cars;

    public interface ICarService
    {
        Task<CarQueryServiceModel> AllAsync(
            string brand = null,
            string searchTerm = null,
            CarSorting sorting = CarSorting.DateCreated,
            int currentPage = 1,
            int carsPerPage = int.MaxValue,
            bool publicOnly = true);

        Task<IEnumerable<LatestCarServiceModel>> LatestAsync();

        Task<CarDetailsServiceModel?> DetailsAsync(int carId);

        Task<int> CreateAsync(
            string brand,
            string model,
            string description,
            string imageUrl,
            int year,
            int categoryId,
            int dealerId);

        Task<bool> EditAsync(
            int carId,
            string brand,
            string model,
            string description,
            string imageUrl,
            int year,
            int categoryId,
            bool isPublic);

        Task<CarDetailsServiceModel> GetCarForDeleteByIdAsync(int id);

        Task DeleteCarByIdAsync(int id);

        Task<IEnumerable<CarServiceModel>> ByUserAsync(string userId);

        Task<bool> IsRentedAsync(int carId);

        Task<bool> IsByDealerAsync(int carId, int dealerId);

        Task<bool> IsRentedByUserWithIdAsync(int carId, string renterId);

        Task ChangeVisibilityAsync(int carId);

        Task<IEnumerable<string>> AllBrandsAsync();

        Task<IEnumerable<CarCategoryServiceModel>> AllCategoriesAsync();

        Task<bool> CategoryExistsAsync(int categoryId);

        Task<bool> ExistsByIdAsync(int carId);

        Task RentCarAsync(int carId, string userId);

        Task ReturnCarAsync(int carId);
    }
}
