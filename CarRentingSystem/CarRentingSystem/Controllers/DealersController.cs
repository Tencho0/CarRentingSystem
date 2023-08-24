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
        public async Task<IActionResult> Become(BecomeDealerFromModel dealer)
        {
            var userId = this.User.Id()!;
            var userIdAlreadyDealer = await this.dealers.IsDealerAsync(userId);

            if (userIdAlreadyDealer)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(dealer);
            }

            await this.dealers.CreateDealerAsync(userId, dealer.Name, dealer.PhoneNumber);

            TempData[GlobalMessageKey] = "Thank you for becoming a dealer!";

            return RedirectToAction("All", "Cars");
        }
    }
}
