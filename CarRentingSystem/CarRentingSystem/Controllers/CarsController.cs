﻿namespace CarRentingSystem.Controllers
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

        public async Task<IActionResult> All([FromQuery] AllCarsQueryModel query)
        {
            var queryResult = await this.cars
                .AllAsync(query.Brand, query.SearchTerm, query.Sorting, query.CurrentPage, AllCarsQueryModel.CarsPerPage);

            var carBrands = await this.cars.AllBrandsAsync();

            query.Brands = carBrands;
            query.TotalCars = queryResult.TotalCars;
            query.Cars = queryResult.Cars;

            return View(query);
        }

        [Authorize]
        public async Task<IActionResult> Mine()
        {
            var myCars = await this.cars.ByUserAsync(this.User.Id()!);

            return View(myCars);
        }

        public async Task<IActionResult> Details(int id, string information)
        {
            var car = await this.cars.DetailsAsync(id);

            if (car == null || information != car.GetInformation())
            {
                return BadRequest();
            }

            return View(car);
        }

        [Authorize]
        public async Task<IActionResult> Add()
        {
            if (!await this.dealers.IsDealerAsync(this.User.Id()!))
            {
                return RedirectToAction(nameof(DealersController.Become), "Dealers");
            }

            return View(new CarFormModel
            {
                Categories = await this.cars.AllCategoriesAsync()
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(CarFormModel car)
        {
            var dealerId = await this.dealers.IdByUserAsync(this.User.Id()!);

            if (dealerId == 0)
            {
                return RedirectToAction(nameof(DealersController.Become), "Dealers");
            }

            if (!await this.cars.CategoryExistsAsync(car.CategoryId))
            {
                this.ModelState.AddModelError(nameof(car.CategoryId), "Category does not exist!");
            }

            if (!ModelState.IsValid)
            {
                car.Categories = await this.cars.AllCategoriesAsync();
                return View(car);
            }

            var carId = await this.cars.CreateAsync(
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
        public async Task<IActionResult> Edit(int id)
        {
            var userId = this.User.Id()!;

            if (!await this.dealers.IsDealerAsync(userId) && !this.User.IsAdmin())
            {
                return RedirectToAction(nameof(DealersController.Become), "Dealers");
            }

            var car = await this.cars.DetailsAsync(id);

            if (car == null)
            {
                return BadRequest();
            }

            if (car.UserId != userId && !this.User.IsAdmin())
            {
                return Unauthorized();
            }

            var carForm = this.mapper.Map<CarFormModel>(car);
            carForm.Categories = await this.cars.AllCategoriesAsync();

            return View(carForm);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, CarFormModel car)
        {
            var dealerId = await this.dealers.IdByUserAsync(this.User.Id()!);

            if (dealerId == 0 && !User.IsAdmin())
            {
                return RedirectToAction(nameof(DealersController.Become), "Dealers");
            }

            if (!await this.cars.CategoryExistsAsync(car.CategoryId))
            {
                this.ModelState.AddModelError(nameof(car.CategoryId), "Category does not exist!");
            }

            if (!ModelState.IsValid)
            {
                car.Categories = await this.cars.AllCategoriesAsync();
                return View(car);
            }

            if (!await this.cars.IsByDealerAsync(id, dealerId) && !User.IsAdmin())
            {
                return BadRequest();
            }

            var edited = await this.cars.EditAsync(
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
        public async Task<IActionResult> Delete(int id)
        {
            bool carExists = await this.cars.ExistsByIdAsync(id);
            if (!carExists)
            {
                TempData[ErrorMessage] = "Car with the provided Id does not exist!";

                return RedirectToAction("All", "Cars");
            }

            bool isUserDealer = await this.dealers.IsDealerAsync(User.Id()!);
            if (!isUserDealer && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "You must become a Dealer in order to edit or delete car info!";

                return RedirectToAction("Become", "Dealers");
            }

            int dealerId = await this.dealers.IdByUserAsync(User.Id()!);
            bool isDealerOwner = await this.cars.IsByDealerAsync(id, dealerId);
            if (!isDealerOwner && !User.IsAdmin())
            {
                TempData[ErrorMessage] =
                    "You must be the Dealer Owner of the car you want to edit or delete!";

                return RedirectToAction("Mine", "Cars");
            }

            try
            {
                CarDetailsServiceModel viewModel = await this.cars.GetCarForDeleteByIdAsync(id);

                return View(viewModel);
            }
            catch (Exception)
            {
                return GeneralError();
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id, CarDetailsServiceModel model)
        {
            bool carExists = await this.cars.ExistsByIdAsync(id);
            if (!carExists)
            {
                TempData[ErrorMessage] = "Car with the provided Id does not exist!";

                return RedirectToAction("All", "Cars");
            }

            bool isUserDealer = await this.dealers.IsDealerAsync(User.Id()!);
            if (!isUserDealer && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "You must become a Dealer in order to edit or delete car info!";

                return RedirectToAction("Become", "Dealers");
            }

            int dealerId = await this.dealers.IdByUserAsync(User.Id()!);
            bool isDealerOwner = await this.cars.IsByDealerAsync(id, dealerId);
            if (!isDealerOwner && !User.IsAdmin())
            {
                TempData[ErrorMessage] =
                    "You must be the Dealer Owner of the car you want to edit or delete!";

                return RedirectToAction("Mine", "Cars");
            }

            try
            {
                await this.cars.DeleteCarByIdAsync(id);

                TempData[WarningMessage] = "The car was successfully deleted!";
                return RedirectToAction("Mine", "Cars");
            }
            catch (Exception)
            {
                return GeneralError();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Rent(int id)
        {
            bool carExists = await this.cars.ExistsByIdAsync(id);
            if (!carExists)
            {
                TempData[ErrorMessage] = "Car with provided Id does not exist! Please try again!";

                return RedirectToAction("All", "Cars");
            }

            bool isCarRented = await this.cars.IsRentedAsync(id);
            if (isCarRented)
            {
                TempData[ErrorMessage] =
                    "Selected Car is already rented by another user! Please select another Car.";

                return RedirectToAction("All", "Cars");
            }

            bool isUserDealer = await this.dealers.IsDealerAsync(this.User.Id()!);
            if (isUserDealer && !User.IsAdmin())
            {
                TempData[ErrorMessage] = "Agents can't rent Cars. Please register as a user!";

                return RedirectToAction("Index", "Home");
            }

            try
            {
                await cars.RentCarAsync(id, this.User.Id()!);
            }
            catch (Exception)
            {
                return GeneralError();
            }

            return RedirectToAction("All", "Cars");
        }

        [HttpPost]
        public async Task<IActionResult> ReturnCar(int id)
        {
            bool carExists = await cars.ExistsByIdAsync(id);
            if (!carExists)
            {
                TempData[ErrorMessage] = "Car with provided id does not exist! Please try again!";

                return RedirectToAction("All", "Cars");
            }

            bool isCarRented = await cars.IsRentedAsync(id);
            if (!isCarRented)
            {
                TempData[ErrorMessage] = "Selected car is not rented!";

                return RedirectToAction("Mine", "Cars");
            }

            bool isCurrentUserRenterOfTheCar = await cars.IsRentedByUserWithIdAsync(id, User.Id()!);
            if (!isCurrentUserRenterOfTheCar)
            {
                TempData[ErrorMessage] =
                    "You must be the renter of the car in order to leave it! Please try again with one of your rented cars if you wish to leave them.";

                return RedirectToAction("Mine", "Cars");
            }

            try
            {
                await cars.ReturnCarAsync(id);
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
