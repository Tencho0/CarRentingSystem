namespace CarRentingSystem
{
    using CarRentingSystem.Data;
    using CarRentingSystem.Data.Models;
    using CarRentingSystem.Infrastructure;
    using CarRentingSystem.Infratructure;
    using CarRentingSystem.Services.Cars;
    using CarRentingSystem.Services.Dealers;
    using CarRentingSystem.Services.Statistics;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<CarRentingDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<User>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<CarRentingDbContext>();

            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddMemoryCache();

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
            });

            builder.Services.AddTransient<ICarService, CarService>();
            builder.Services.AddTransient<IDealerService, DealerService>();
            builder.Services.AddTransient<IStatisticsService, StatisticsService>();

            var app = builder.Build();

            app.PrepareDatabase();

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app
                .UseHttpsRedirection()
                .UseStaticFiles()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization();

            app.MapDefaultAreaRoute();
            app.MapDefaultControllerRoute();
            app.MapRazorPages();
            app.UseAuthentication(); ;

            app.Run();
        }
    }
}