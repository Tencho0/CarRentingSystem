namespace CarRentingSystem.Areas.Admin.Controller
{
    using Microsoft.AspNetCore.Mvc;
    using CarRentingSystem.Services.Cars;

    public class CarsController : AdminController
    {
        private readonly ICarService cars;

        public CarsController(ICarService cars) => this.cars = cars;

        public IActionResult All() => View(this.cars.All(publicOnly: false).Cars);

        public IActionResult ChangeVisibility(int id)
        {
            this.cars.ChangeVisibility(id);

            return RedirectToAction(nameof(All));
        }
    }
}
