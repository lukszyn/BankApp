using System;
using System.Collections.Generic;

namespace BankApp.DataLayer.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid Number { get; set; }
        public decimal Balance { get; set; }
        public List<Transfer> Transfers { get; set; }
        public int UserId { get; set; }
        public User User{ get; set; }

        public Account(Guid number)
        {
            Number = number;
        }

        public Account()
        {
        }

        public Account(int userId, string name) : this()
        {
            UserId = userId;
            Name = name;
            Number = Guid.NewGuid();
            Balance = 1000.00m;
        }
    }
}
