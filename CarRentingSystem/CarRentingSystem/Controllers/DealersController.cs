namespace CarRentingSystem.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Services.Dealers;
    using ViewModels.Dealers;
    using Infrastructure.Extensions;

    using static Common.WebConstants;

    public class DealersController : Controller
    {
        private readonly IDealerService dealers;

        public DealersController(IDealerService dealers)
        {
            this.dealers = dealers;
        }

        [Authorize]
        public IActionResult Become() => View();

        [HttpPost]
        [Authorize]
        public IActionResult Become(BecomeDealerFromModel dealer)
        {
            var userId = this.User.Id()!;
            var userIdAlreadyDealer = this.dealers.IsDealer(userId);

            if (userIdAlreadyDealer)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(dealer);
            }

            this.dealers.CreateDealer(userId, dealer.Name, dealer.PhoneNumber);

            TempData[GlobalMessageKey] = "Thank you for becoming a dealer!";

            return RedirectToAction("All", "Cars");
        }
    }
}
