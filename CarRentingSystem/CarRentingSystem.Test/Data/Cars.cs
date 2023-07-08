namespace CarRentingSystem.Test.Data
{
    using CarRentingSystem.Data.Models;

    public static class Cars
    {
        public static IEnumerable<Car> TenPublicCars()
            => Enumerable.Range(0, 10).Select(x => new Car
            {
                Brand = "BMW",
                Model = "M4 coupe",
                Description = "Very fast Bmw m4 coupe",
                ImageUrl = "https://www.driving.co.uk/wp-content/uploads/sites/5/2014/08/BMWM4.jpg",
                IsPublic = true
            });
    }
}
