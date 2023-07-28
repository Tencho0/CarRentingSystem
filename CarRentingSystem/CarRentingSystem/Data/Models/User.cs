namespace CarRentingSystem.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations;

    using static DataConstants.User;

    public class User : IdentityUser
    {
        [MaxLength(FullNameMaxLength)]
        public string FullName { get; set; } = null!;

        public virtual ICollection<Car> RentedCars { get; set; } = new HashSet<Car>();
    }
}
