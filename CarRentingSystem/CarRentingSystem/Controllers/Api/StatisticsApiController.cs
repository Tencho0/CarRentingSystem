namespace CarRentingSystem.Controllers.Api
{
    using CarRentingSystem.Services.Statistics;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/statistics")]
    public class StatisticsApiController : ControllerBase
    {
        private readonly IStatisticsServices statistics;

        public StatisticsApiController(IStatisticsServices statistics) 
            => this.statistics = statistics;

        [HttpGet]
        public StatisticsServiceModel GetStatistics() 
            => this.statistics.Total();
    }
}
