namespace CarRentingSystem.Services.Cars
{
    using System.Linq;
    using System.Collections.Generic;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using Data;
    using ViewModels;
    using CarRentingSystem.Data.Models;
    using CarRentingSystem.Services.Models.Cars;
    using Microsoft.EntityFrameworkCore;

    public class CarService : ICarService
    {
        private readonly CarRentingDbContext data;
        private readonly IMapper mapper;

        public CarService(CarRentingDbContext data, IMapper mapper)
        {
            this.data = data;
            this.mapper = mapper;
        }

        public async Task<CarQueryServiceModel> AllAsync(
       string brand = null,
       string searchTerm = null,
       CarSorting sorting = CarSorting.DateCreated,
       int currentPage = 1,
       int carsPerPage = int.MaxValue,
       bool publicOnly = true)
        {
            IQueryable<Car> carsQuery = this.data.Cars
                .Where(c => !c.IsDeleted && (!publicOnly || c.IsPublic));

            if (!string.IsNullOrWhiteSpace(brand))
            {
                carsQuery = carsQuery.Where(c => c.Brand == brand);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                carsQuery = carsQuery.Where(c =>($"{c.Brand} {c.Model} {c.Description}").ToLower().Contains(searchTerm.ToLower()));
            }

            carsQuery = sorting switch
            {
                CarSorting.Year => carsQuery.OrderByDescending(c => c.Year),
                CarSorting.BrandAndModel => carsQuery.OrderByDescending(c => c.Brand).ThenBy(c => c.Model),
                CarSorting.DateCreated or _ => carsQuery.OrderByDescending(c => c.Id)
            };

            int totalCars = await carsQuery.CountAsync();
            List<CarServiceModel> cars = await GetCarsAsync(carsQuery
                .Skip((currentPage - 1) * carsPerPage)
                .Take(carsPerPage));

            return new CarQueryServiceModel
            {
                TotalCars = totalCars,
                CurrentPage = currentPage,
                CarsPerPage = carsPerPage,
                Cars = cars
            };
        }

        public async Task<IEnumerable<LatestCarServiceModel>> LatestAsync()
        {
            var latestCars = await this.data.Cars
                .Where(c => c.IsPublic && !c.IsDeleted)
                .OrderByDescending(x => x.Id)
                .ProjectTo<LatestCarServiceModel>(this.mapper.ConfigurationProvider)
                .Take(3)
                .ToListAsync();

            return latestCars;
        }

        public async Task<CarDetailsServiceModel?> DetailsAsync(int id)
        {
            var carDetails = await this.data.Cars
                .Where(c => c.Id == id)
                .ProjectTo<CarDetailsServiceModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return carDetails;
        }

        public async Task<int> CreateAsync(string brand, string model, string description, string imageUrl, int year, int categoryId, int dealerId)
        {
            var carData = new Car
            {
                Brand = brand,
                Model = model,
                Description = description,
                ImageUrl = imageUrl,
                Year = year,
                CategoryId = categoryId,
                DealerId = dealerId,
                IsPublic = false
            };

            this.data.Cars.Add(carData);
            await this.data.SaveChangesAsync();

            return carData.Id;
        }

        public async Task<bool> EditAsync(int id, string brand, string model, string description, string imageUrl, int year, int categoryId, bool isPublic)
        {
            var carData = await this.data.Cars.FindAsync(id);

            if (carData == null)
            {
                return false;
            }

            carData.Brand = brand;
            carData.Model = model;
            carData.Description = description;
            carData.ImageUrl = imageUrl;
            carData.Year = year;
            carData.CategoryId = categoryId;
            carData.IsPublic = isPublic;

            await this.data.SaveChangesAsync();
            return true;
        }

        public async Task<CarDetailsServiceModel> GetCarForDeleteByIdAsync(int id)
        {
            var carDetails = await this.data.Cars
                .Where(c => !c.IsDeleted)
                .ProjectTo<CarDetailsServiceModel>(this.mapper.ConfigurationProvider)
                .FirstAsync(c => c.Id == id);

            return carDetails;
        }

        public async Task DeleteCarByIdAsync(int id)
        {
            var carToDelete = await this.data.Cars
                .Where(c => !c.IsDeleted)
                .FirstAsync(c => c.Id == id);

            carToDelete.IsDeleted = true;
            carToDelete.IsPublic = false;

            await this.data.SaveChangesAsync();
        }

        public async Task<IEnumerable<CarServiceModel>> ByUserAsync(string userId)
        {
            var cars = await GetCarsAsync(this.data.Cars.Where(c => c.Dealer.UserId == userId && !c.IsDeleted));
            return cars;
        }

        public async Task<bool> IsRentedAsync(int carId)
        {
            var isRented = await this.data.Cars.AnyAsync(c => c.Id == carId && c.RenterId != null);
            return isRented;
        }

        public async Task<bool> IsByDealerAsync(int carId, int dealerId)
        {
            var isByDealer = await this.data.Cars.AnyAsync(c => c.Id == carId && c.DealerId == dealerId);
            return isByDealer;
        }

        public async Task<bool> IsRentedByUserWithIdAsync(int carId, string renterId)
        {
            var isRentedByUser = await this.data.Cars.AnyAsync(c => c.Id == carId && c.RenterId != null && c.RenterId == renterId);
            return isRentedByUser;
        }

        public async Task ChangeVisibilityAsync(int carId)
        {
            var car = await this.data.Cars.FindAsync(carId);
            car!.IsPublic = !car.IsPublic;

            await this.data.SaveChangesAsync();
        }

        public async Task<IEnumerable<string>> AllBrandsAsync()
        {
            var brands = await this.data.Cars
                .Where(c => c.IsPublic && !c.IsDeleted)
                .Select(c => c.Brand)
                .Distinct()
                .OrderBy(br => br)
                .ToListAsync();

            return brands;
        }

        public async Task<IEnumerable<CarCategoryServiceModel>> AllCategoriesAsync()
        {
            var categories = await this.data.Categories
                .ProjectTo<CarCategoryServiceModel>(this.mapper.ConfigurationProvider)
                .ToListAsync();

            return categories;
        }

        public async Task<bool> CategoryExistsAsync(int categoryId)
        {
            var categoryExists = await this.data.Categories.AnyAsync(c => c.Id == categoryId);
            return categoryExists;
        }

        public async Task<bool> ExistsByIdAsync(int carId)
        {
            var exists = await data.Cars
                .Where(c => c.IsPublic)
                .AnyAsync(c => c.Id == carId);

            return exists;
        }

        public async Task RentCarAsync(int carId, string userId)
        {
            var car = await data.Cars.FirstAsync(c => c.Id == carId);
            car.RenterId = userId;

            await data.SaveChangesAsync();
        }

        public async Task ReturnCarAsync(int carId)
        {
            var car = await data.Cars.FirstAsync(c => c.Id == carId);
            var renterId = car.RenterId!;

            car.RenterId = null;
            data.Users
                .First(u => u.Id == renterId)
                .RentedCars.Remove(car);

            await data.SaveChangesAsync();
        }

        private async Task<List<CarServiceModel>> GetCarsAsync(IQueryable<Car> carQuery)
        {
            var cars = await carQuery
                .ProjectTo<CarServiceModel>(this.mapper.ConfigurationProvider)
                .ToListAsync();

            return cars;
        }
    }
}
