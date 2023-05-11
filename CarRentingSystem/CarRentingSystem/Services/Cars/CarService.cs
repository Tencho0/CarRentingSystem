namespace CarRentingSystem.Services.Cars
{
    using CarRentingSystem.Models;
    using CarRentingSystem.Data;
    using System.Collections.Generic;
    using CarRentingSystem.Data.Models;

    public class CarService : ICarService
    {
        private readonly CarRentingDbContext data;

        public CarService(CarRentingDbContext data)
            => this.data = data;

        public CarQueryServiceModel All(string brand, string searchTerm, CarSorting sorting, int currentPage, int carsPerPage)
        {
            var carsQuery = this.data.Cars.AsQueryable();

            if (!string.IsNullOrWhiteSpace(brand))
            {
                carsQuery = carsQuery.Where(c => c.Brand == brand);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                carsQuery = carsQuery.Where(c =>
                    (c.Brand + " " + c.Model).ToLower().Contains(searchTerm.ToLower())
                    || c.Description.ToLower().Contains(searchTerm.ToLower()));
            }

            carsQuery = sorting switch
            {
                CarSorting.Year => carsQuery.OrderByDescending(c => c.Year),
                CarSorting.BrandAndModel => carsQuery.OrderByDescending(c => c.Brand).ThenBy(c => c.Model),
                CarSorting.DateCreated or _ => carsQuery.OrderByDescending(c => c.Id)
            };

            var totalCars = carsQuery.Count();
            var cars = GetCars(carsQuery
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


        public CarDetailsServiceModel Details(int id)
              => this.data.Cars
                        .Where(c => c.Id == id)
                        .Select(c => new CarDetailsServiceModel
                        {
                            Id = c.Id,
                            Brand = c.Brand,
                            Model = c.Model,
                            Description = c.Description,
                            ImageUrl = c.ImageUrl,
                            Year = c.Year,
                            Category = c.Category.Name,
                            DealerId = c.DealerId,
                            DealerName = c.Dealer.Name,
                            UserId = c.Dealer.UserId
                        })
                        .FirstOrDefault()!;

        public IEnumerable<CarServiceModel> ByUser(string userId)
              => GetCars(this.data.Cars.Where(c => c.Dealer.UserId == userId));

        public IEnumerable<string> AllBrands()
              => this.data.Cars
                        .Select(c => c.Brand)
                        .Distinct()
                        .OrderBy(br => br)
                        .ToList();

        public IEnumerable<CarCategoryServiceModel> AllCategories()
              => this.data.Categories
                    .Select(c => new CarCategoryServiceModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                    })
                    .ToList();

        private static IEnumerable<CarServiceModel> GetCars(IQueryable<Car> carQuery)
              => carQuery
                 .Select(c => new CarServiceModel
                 {
                     Id = c.Id,
                     Brand = c.Brand,
                     Model = c.Model,
                     ImageUrl = c.ImageUrl,
                     Year = c.Year,
                     Category = c.Category.Name
                 })
                .ToList();
    }
}
