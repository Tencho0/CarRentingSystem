namespace CarRentingSystem.Controllers
{
    using CarRentingSystem.Data;
    using CarRentingSystem.Data.Models;
    using CarRentingSystem.Infratructure;
    using CarRentingSystem.Models;
    using CarRentingSystem.Models.Cars;
    using CarRentingSystem.Services.Cars;
    using CarRentingSystem.Services.Dealers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;

    public class CarsController : Controller
    {
        private readonly ICarService cars;
        private readonly IDealerService dealers;
        private readonly CarRentingDbContext data;

        public CarsController(ICarService cars, IDealerService dealers, CarRentingDbContext data)
        {
            this.cars = cars;
            this.dealers = dealers;
            this.data = data;
        }

        public IActionResult All([FromQuery] AllCarsQueryModel query)
        {
            var queryResult = this.cars
                .All(query.Brand, query.SearchTerm, query.Sorting, query.CurrentPage, AllCarsQueryModel.CarsPerPage);

            var carBrands = this.cars.AllBrands();

            query.Brands = carBrands;
            query.TotalCars = queryResult.TotalCars;
            query.Cars = queryResult.Cars;

            return View(query);
        }

        [Authorize]
        public IActionResult Mine()
        {
            var myCars = this.cars.ByUser(this.User.GetId());

            return View(myCars);
        }

        [Authorize]
        public IActionResult Add()
        {
            if (!this.dealers.IsDealer(this.User.GetId()))
            {
                return RedirectToAction(nameof(DealersController.Become), "Dealers");
            }

            return View(new CarFormModel
            {
                Categories = this.cars.AllCategories()
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add(CarFormModel car)
        {
            var dealerId = this.dealers.GetIdByUser(this.User.GetId());

            if (dealerId == 0)
            {
                return RedirectToAction(nameof(DealersController.Become), "Dealers");
            }

            if (!this.data.Categories.Any(c => c.Id == car.CategoryId))
            {
                this.ModelState.AddModelError(nameof(car.CategoryId), "Category does not exist!");
            }

            if (!ModelState.IsValid)
            {
                car.Categories = this.cars.AllCategories();
                return View(car);
            }

            var carData = new Car()
            {
                Brand = car.Brand,
                Model = car.Model,
                Description = car.Description,
                ImageUrl = car.ImageUrl,
                Year = car.Year,
                CategoryId = car.CategoryId,
                DealerId = dealerId
            };

            this.data.Cars.Add(carData);
            this.data.SaveChanges();

            return RedirectToAction(nameof(All));
        }
    }
}
