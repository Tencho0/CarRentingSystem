namespace CarRentingSystem.Infrastructure.Extensions
{
    public static class ModelExtensions
    {
        public static string GetInformation(this ICarModel car)
            => $"{car.Brand}-{car.Model}-{car.Year}";
    }
}
