namespace CarRentingSystem.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    
    using static CarRentingSystem.Common.DataConstants.Car;

    public class Car
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(BrandMaxLength)]
        public string Brand { get; set; } = null!;

        [Required]
        [MaxLength(ModelMaxLength)]
        public string Model { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public string ImageUrl { get; set; } = null!;

        public int Year { get; set; }

        public bool IsPublic { get; set; }

        public bool IsDeleted { get; set; } = false;

        public int CategoryId { get; set; }

        public virtual Category Category { get; init; } = null!;

        public int DealerId { get; init; }

        public virtual Dealer Dealer { get; init; } = null!;

        public string? RenterId { get; set; }

        public virtual User? Renter { get; set; }
    }
}
