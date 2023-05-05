namespace CarRentingSystem.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using static DataConstants;

    public class Car
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(CarBrandMaxLength)]
        public string Brand { get; set; } = null!;

        [Required]
        [MaxLength(CarModelMaxLength)]
        public string Model { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public string ImageUrl { get; set; } = null!;

        public int Year { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; init; } = null!;
    }
}
