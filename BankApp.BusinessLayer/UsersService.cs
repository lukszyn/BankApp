using BankApp.DataLayer;
using BankApp.DataLayer.Models;
using System.Linq;

namespace BankApp.BusinessLayer
{
    public class UsersService
    {
        public bool CheckIfUserExists(string email)
        {
            using (var context = new BankAppDbContext())
            {
                return context.Users.Any(user => user.Email == email);
            }
        }

        public void Add(User user)
        {
            using (var context = new BankAppDbContext())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public bool CheckCredentials(string email, string password)
        {
            using (var context = new BankAppDbContext())
            {
                return context.Users.Any(user => user.Email == email && user.Password == password);
            }
        }

        public User Get(string email)
        {
            using (var context = new BankAppDbContext())
            {
                return context.Users.FirstOrDefault(user => user.Email == email);
            }
        }
    }
}
