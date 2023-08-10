namespace CarRentingSystem.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using static CarRentingSystem.Common.DataConstants.Category;

    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        public virtual IEnumerable<Car> Cars { get; set; } = new List<Car>();
    }
}
