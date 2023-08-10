using CarRentingSystem.Services.Models.Cars;

namespace CarRentingSystem.ViewModels.Cars
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static Common.DataConstants.Car;

    public class CarFormModel : ICarModel
    {
        [Required]
        [StringLength(BrandMaxLength, MinimumLength = BrandMinLength)]
        public string Brand { get; init; } = null!;

        [Required]
        [StringLength(ModelMaxLength, MinimumLength = ModelMinLength)]
        public string Model { get; init; } = null!;

        [Required]
        [StringLength(
            int.MaxValue,
            MinimumLength = DescriptionMinLength,
            ErrorMessage = "The field Description must be a string with minimum length of {2}")]
        public string Description { get; init; } = null!;

        [Display(Name = "Image Url")]
        [Required]
        public string ImageUrl { get; init; } = null!;

        [Range(YearMinValue, YearMaxValue)]
        public int Year { get; init; }

        [Display(Name = "Category")]
        public int CategoryId { get; init; }

        public IEnumerable<CarCategoryServiceModel>? Categories { get; set; }
    }
}
