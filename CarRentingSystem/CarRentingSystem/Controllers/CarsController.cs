namespace CarRentingSystem.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Infrastructure.Extensions;
    using ViewModels.Cars;
    using Services.Cars;
    using Services.Dealers;
    using Services.Models.Cars;

    using static Common.WebConstants;
    using static Common.WebConstants.NotificationMessagesConstants;

    public class CarsController : Controller
    {
        private readonly ICarService cars;
        private readonly IDealerService dealers;
        private readonly IMapper mapper;

        public CarsController(ICarService cars, IDealerService dealers, IMapper mapper = null)
        {
            this.cars = cars;
            this.dealers = dealers;
            this.mapper = mapper;
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
            var myCars = this.cars.ByUser(this.User.Id()!);

            return View(myCars);
        }

        public IActionResult Details(int id, string information)
        {
            var car = this.cars.Details(id);

            if (car == null || information != car.GetInformation())
            {
                return BadRequest();
            }

            return View(car);
        }

        [Authorize]
        public IActionResult Add()
        {
            if (!this.dealers.IsDealer(this.User.Id()!))
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
            var dealerId = this.dealers.IdByUser(this.User.Id()!);

            if (dealerId == 0)
            {
                return RedirectToAction(nameof(DealersController.Become), "Dealers");
            }

            if (!this.cars.CategoryExists(car.CategoryId))
            {
                this.ModelState.AddModelError(nameof(car.CategoryId), "Category does not exist!");
            }

            if (!ModelState.IsValid)
            {
                car.Categories = this.cars.AllCategories();
                return View(car);
            }

            var carId = this.cars.Create(
                car.Brand,
                car.Model,
                car.Description,
                car.ImageUrl,
                car.Year,
                car.CategoryId,
                dealerId);

            TempData[GlobalMessageKey] = "Your car was added successfully and is awaiting for approval!";

            return RedirectToAction(nameof(Details), new
            {
                id = carId,
                information = car.GetInformation()
            });
        }

        [Authorize]
        public IActionResult Edit(int id)
        {
            var userId = this.User.Id()!;

            if (!this.dealers.IsDealer(userId) && !this.User.IsAdmin())
            {
                return RedirectToAction(nameof(DealersController.Become), "Dealers");
            }

            var car = this.cars.Details(id);

            if (car == null)
            {
                return BadRequest();
            }

            if (car.UserId != userId && !this.User.IsAdmin())
            {
                return Unauthorized();
            }

            var carForm = this.mapper.Map<CarFormModel>(car);
            carForm.Categories = this.cars.AllCategories();

            return View(carForm);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(int id, CarFormModel car)
        {
            var dealerId = this.dealers.IdByUser(this.User.Id()!);

            if (dealerId == 0 && !User.IsAdmin())
            {
                return RedirectToAction(nameof(DealersController.Become), "Dealers");
            }

            if (!this.cars.CategoryExists(car.CategoryId))
            {
                this.ModelState.AddModelError(nameof(car.CategoryId), "Category does not exist!");
            }

            if (!ModelState.IsValid)
            {
                car.Categories = this.cars.AllCategories();
                return View(car);
            }

            if (!this.cars.IsByDealer(id, dealerId) && !User.IsAdmin())
            {
                return BadRequest();
            }

            var edited = this.cars.Edit(
                 id,
                 car.Brand,
                 car.Model,
                 car.Description,
                 car.ImageUrl,
                 car.Year,
                 car.CategoryId,
                 this.User.IsAdmin());

            if (!edited)
            {
                return BadRequest();
            }

            TempData[GlobalMessageKey] = $"Your car was edited successfully{(this.User.IsAdmin() ? string.Empty : " and is awaiting for approval")}!";

            return RedirectToAction(nameof(Details), new
            {
                id,
                information = car.GetInformation()
            });
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            bool carExists = this.cars.ExistsById(id);
            if (!carExists)
            {
                TempData[ErrorMessage] = "Car with the provided Id does not exist!";

                return RedirectToAction("All", "Cars");
            }

            bool isUserDealer = this.dealers.IsDealer(User.Id()!);
            if (!isUserDealer && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "You must become a Dealer in order to edit or delete car info!";

                return RedirectToAction("Become", "Dealers");
            }

            int dealerId = this.dealers.IdByUser(User.Id()!);
            bool isDealerOwner = this.cars.IsByDealer(id, dealerId);
            if (!isDealerOwner && !User.IsAdmin())
            {
                TempData[ErrorMessage] =
                    "You must be the Dealer Owner of the car you want to edit or delete!";

                return RedirectToAction("Mine", "Cars");
            }

            try
            {
                CarDetailsServiceModel viewModel = this.cars.GetCarForDeleteById(id);

                return View(viewModel);
            }
            catch (Exception)
            {
                return GeneralError();
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult Delete(int id, CarDetailsServiceModel model)
        {
            bool carExists = this.cars.ExistsById(id);
            if (!carExists)
            {
                TempData[ErrorMessage] = "Car with the provided Id does not exist!";

                return RedirectToAction("All", "Cars");
            }

            bool isUserDealer = this.dealers.IsDealer(User.Id()!);
            if (!isUserDealer && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "You must become a Dealer in order to edit or delete car info!";

                return RedirectToAction("Become", "Dealers");
            }

            int dealerId = this.dealers.IdByUser(User.Id()!);
            bool isDealerOwner = this.cars.IsByDealer(id, dealerId);
            if (!isDealerOwner && !User.IsAdmin())
            {
                TempData[ErrorMessage] =
                    "You must be the Dealer Owner of the car you want to edit or delete!";

                return RedirectToAction("Mine", "Cars");
            }

            try
            {
                this.cars.DeleteCarById(id);

                TempData[WarningMessage] = "The car was successfully deleted!";
                return RedirectToAction("Mine", "Cars");
            }
            catch (Exception)
            {
                return GeneralError();
            }
        }

        [HttpPost]
        public IActionResult Rent(int id)
        {
            bool carExists = this.cars.ExistsById(id);
            if (!carExists)
            {
                TempData[ErrorMessage] = "Car with provided Id does not exist! Please try again!";

                return RedirectToAction("All", "Cars");
            }

            bool isCarRented = this.cars.IsRented(id);
            if (isCarRented)
            {
                TempData[ErrorMessage] =
                    "Selected Car is already rented by another user! Please select another Car.";

                return RedirectToAction("All", "Cars");
            }

            bool isUserDealer = this.dealers.IsDealer(this.User.Id()!);
            if (isUserDealer && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "Agents can't rent Cars. Please register as a user!";

                return RedirectToAction("Index", "Home");
            }

            try
            {
                cars.RentCar(id, this.User.Id()!);
            }
            catch (Exception)
            {
                return GeneralError();
            }

            return RedirectToAction("All", "Cars");
        }

        [HttpPost]
        public IActionResult ReturnCar(int id)
        {
            bool carExists = cars.ExistsById(id);
            if (!carExists)
            {
                TempData[ErrorMessage] = "Car with provided id does not exist! Please try again!";

                return RedirectToAction("All", "Cars");
            }

            bool isCarRented = cars.IsRented(id);
            if (!isCarRented)
            {
                TempData[ErrorMessage] = "Selected car is not rented!";

                return RedirectToAction("Mine", "Cars");
            }

            bool isCurrentUserRenterOfTheCar = cars.IsRentedByUserWithId(id, User.Id()!);
            if (!isCurrentUserRenterOfTheCar)
            {
                TempData[ErrorMessage] =
                    "You must be the renter of the car in order to leave it! Please try again with one of your rented cars if you wish to leave them.";

                return RedirectToAction("Mine", "Cars");
            }

            try
            {
                cars.ReturnCar(id);
            }
            catch (Exception)
            {
                return GeneralError();
            }

            return RedirectToAction("All", "Cars");
        }

        private IActionResult GeneralError()
        {
            TempData[ErrorMessage] =
                "Unexpected error occurred! Please try again later or contact administrator";

            return RedirectToAction("Index", "Home");
        }
    }
}
