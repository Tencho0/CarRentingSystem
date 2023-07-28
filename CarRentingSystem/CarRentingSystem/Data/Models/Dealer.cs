namespace CarRentingSystem.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static DataConstants.Dealer;

    public class Dealer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(PhoneNumberMaxLength)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public virtual IEnumerable<Car> Cars { get; set; } = new List<Car>();
    }
}
