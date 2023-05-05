namespace CarRentingSystem.Data.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public IEnumerable<Car> Cars { get; set; } = new List<Car>();
    }
}
