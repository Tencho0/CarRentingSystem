﻿namespace CarRentingSystem.Services.Cars
{
    using Models;
    using CarRentingSystem.Models;

    public interface ICarService
    {
        CarQueryServiceModel All(
            string brand = null,
            string searchTerm = null,
            CarSorting sorting = CarSorting.DateCreated,
            int currentPage = 1,
            int carsPerPage = int.MaxValue,
            bool publicOnly = true);

        IEnumerable<LatestCarServiceModel> Latest();

        CarDetailsServiceModel Details(int carId);

        int Create(
            string brand,
            string model,
            string description,
            string imageUrl,
            int year,
            int categoryId,
            int dealerId);

        bool Edit(
            int carId,
            string brand,
            string model,
            string description,
            string imageUrl,
            int year,
            int categoryId,
            bool isPublic);

        IEnumerable<CarServiceModel> ByUser(string userId);

        bool IsByDealer(int carId, int dealerId);

        void ChangeVisibility(int carId);

        IEnumerable<string> AllBrands();

        IEnumerable<CarCategoryServiceModel> AllCategories();

        bool CategoryExists(int categoryId);
    }
}
