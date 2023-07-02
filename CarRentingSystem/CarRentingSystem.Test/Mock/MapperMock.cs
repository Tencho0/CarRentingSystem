namespace CarRentingSystem.Test.Mock
{
    using AutoMapper;
    using CarRentingSystem.Infratructure;

    public static class MapperMock
    {
        public static IMapper Instance
        {
            get
            {
                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                });

                return new Mapper(mapperConfig);
            }
        }
    }
}
