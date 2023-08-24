namespace CarRentingSystem.Controllers.Api
{
    using Microsoft.AspNetCore.Mvc;

    using Services.Cars;
    using Services.Models.Cars;
    using CarRentingSystem.ViewModels.Api.Cars;

    [ApiController]
    [Route("api/cars")]
    public class CarsApiController : ControllerBase
    {
        private readonly ICarService cars;

        public CarsApiController(ICarService cars)
            => this.cars = cars;

        [HttpGet]
        public async Task<CarQueryServiceModel> All([FromQuery] AllCarsApiRequestModel query)
            => await this.cars.AllAsync(query.Brand, query.SearchTerm, query.Sorting, query.CurrentPage, query.CarsPerPage);
    }
}
