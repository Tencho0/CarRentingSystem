namespace CarRentingSystem.Areas.Admin.Controller
{
    using Microsoft.AspNetCore.Mvc;

    using Services.Cars;

    public class CarsController : AdminController
    {
        private readonly ICarService cars;

        public CarsController(ICarService cars) => this.cars = cars;

        public async Task<IActionResult> All()
        {
            var carQueryResult = await this.cars.AllAsync(publicOnly: false);
            return View(carQueryResult.Cars);
        }

        public async Task<IActionResult> ChangeVisibility(int id)
        {
            await this.cars.ChangeVisibilityAsync(id);

            return RedirectToAction(nameof(All));
        }
    }
}
