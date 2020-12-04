using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.DataLayer.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public List<Account> Accounts { get; set; }

        public User() { }

        public User(string email, string phoneNumber, string password)
        {
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
        }
    }
}
