namespace CarRentingSystem.ViewModels.Dealers
{
    using System.ComponentModel.DataAnnotations;

    using static Common.DataConstants.Dealer;

    public class BecomeDealerFromModel
    {
        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(PhoneNumberMaxLength, MinimumLength = PhoneNumberMinLength)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = null!;
    }
}
