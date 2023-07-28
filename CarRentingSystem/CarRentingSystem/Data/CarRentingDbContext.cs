namespace CarRentingSystem.Data
{
    using System.Reflection.Emit;
    using CarRentingSystem.Data.Models;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class CarRentingDbContext : IdentityDbContext<User>
    {
        public CarRentingDbContext(DbContextOptions<CarRentingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Car> Cars { get; init; } = null!;

        public DbSet<Category> Categories { get; init; } = null!;

        public DbSet<Dealer> Dealers { get; init; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                  .Entity<Car>()
                  .HasOne(c => c.Category)
                  .WithMany(c => c.Cars)
                  .HasForeignKey(c => c.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Car>()
                .HasOne(c => c.Dealer)
                .WithMany(d => d.Cars)
                .HasForeignKey(c => c.DealerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Dealer>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<Dealer>(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<User>()
                .HasMany(u => u.RentedCars)
                .WithOne(c => c.Renter)
                .HasForeignKey(c => c.RenterId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
        }
    }
}