using RealEstateListingApp.Models;

namespace RealEstateListingApp.Services
{
    public interface IUserRepository
    {
        void SaveUserData(User data);

        IEnumerable<User> GetAllUsers();

        User GetUserById(int id);

        User IsValidUser(string username,string password);

        void DeleteUserData(int id);

        void UpdateUserData(User data);

    }
}
