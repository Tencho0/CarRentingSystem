namespace CarRentingSystem.Test.Data
{
    using CarRentingSystem.Data.Models;

    using static TestingConstants;

    public static class Users
    {
        public static User OneUser()
            => new User
            {
                Id = UserData.Id,
                PhoneNumber = UserData.PhoneNumber,
                FullName = UserData.FullName
            };
    }
}
