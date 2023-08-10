namespace CarRentingSystem.Infrastructure.Extensions
{
    using Services.Models.Cars;

    public static class ModelExtensions
    {
        public static string GetInformation(this ICarModel car)
            => $"{car.Brand}-{car.Model}-{car.Year}";
    }
}
