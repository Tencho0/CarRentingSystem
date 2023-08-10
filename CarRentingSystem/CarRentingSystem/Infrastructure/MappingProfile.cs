namespace CarRentingSystem.Infrastructure
{
    using AutoMapper;

    using Data.Models;
    using Services.Models.Cars;
    using ViewModels.Cars;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Category, CarCategoryServiceModel>();

            this.CreateMap<Car, LatestCarServiceModel>();
            this.CreateMap<Car, CarServiceModel>()
                .ForMember(c => c.CategoryName,
                                       cfg => cfg.MapFrom(c => c.Category.Name));
            this.CreateMap<CarDetailsServiceModel, CarFormModel>();

            this.CreateMap<Car, CarDetailsServiceModel>()
                .ForMember(c => c.UserId, cfg => cfg.MapFrom(c => c.Dealer.UserId))
                .ForMember(c => c.CategoryName,
                    cfg => cfg.MapFrom(c => c.Category.Name));
        }
    }
}
