namespace CarRentingSystem.Services.Models.Cars
{
    public interface ICarModel
    {
        string Brand { get; }

        string Model { get; }

        int Year { get; }
    }
}
