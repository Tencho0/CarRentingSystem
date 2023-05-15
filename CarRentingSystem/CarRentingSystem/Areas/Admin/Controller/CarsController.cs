namespace CarRentingSystem.Areas.Admin.Controller
{
    using Microsoft.AspNetCore.Mvc;

    public class CarsController : AdminController
    {
        public IActionResult Index() => View();
    }
}
