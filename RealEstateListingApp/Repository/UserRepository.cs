using Microsoft.EntityFrameworkCore;
using RealEstateListingApp.Models;
using RealEstateListingApp.Services;
//using System.Linq.Dynamic.Core;

namespace RealEstateListingApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DbContextFile db;

        private DbSet<User> users;

        public UserRepository(DbContextFile db)
        {
            this.db = db;
            users = db.Set<User>();
        }

        public void DeleteUserData(int id)
        {
            User data = GetUserById(id);
            db.Users.Remove(data);
            db.SaveChanges();

        }

        public IEnumerable<User> GetAllUsers()
        {
            return db.Users.ToList();
        }

        public User GetUserById(int id)
        {
            return db.Users.Where(x => x.UserId == id).FirstOrDefault();
        }

        public User IsValidUser(string username,string password)
        {
            var pass = db.Users.Where(x => x.UserName == username && x.Password == password).Select(x => x.Password).FirstOrDefault();
            return db.Users.Where(x => x.UserName == username && pass == password).FirstOrDefault();
        }

        public void SaveUserData(User data)
        {
            db.Users.Add(data);
            db.SaveChanges();
        }

        public void UpdateUserData(User data)
        {
            users.Attach(data);
            db.Users.Update(data);
            db.SaveChanges();
        }
    }
}
