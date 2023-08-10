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
        public CarQueryServiceModel All([FromQuery] AllCarsApiRequestModel query)
            => this.cars.All(query.Brand, query.SearchTerm, query.Sorting, query.CurrentPage, query.CarsPerPage);
    }
}
