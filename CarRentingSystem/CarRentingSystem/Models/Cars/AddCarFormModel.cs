namespace CarRentingSystem.Models.Cars
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants;

    public class AddCarFormModel
    {
        [Required]
        [StringLength(CarBrandMaxLength, MinimumLength = CarBrandMinLength)]
        public string Brand { get; init; } = null!;

        [Required]
        [StringLength(CarModelMaxLength, MinimumLength = CarModelMinLength)]
        public string Model { get; init; } = null!;

        [Required]
        [StringLength(
            int.MaxValue,
            MinimumLength = CarDescriptionMinLength,
            ErrorMessage = "The field Description must be a string with minimum length of {2}")]
        public string Description { get; init; } = null!;

        [Display(Name = "Image Url")]
        [Required]
        public string ImageUrl { get; init; } = null!;

        [Range(CarYearMinValue, CarYearMaxValue)]
        public int Year { get; init; }

        [Display(Name = "Category")]
        public int CategoryId { get; init; }

        public IEnumerable<CarCategoryViewModel>? Categories { get; set; }
    }
}
