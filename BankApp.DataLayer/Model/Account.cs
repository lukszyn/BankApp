using System;
using System.Collections.Generic;

namespace BankApp.DataLayer.Model
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid Number { get; set; }
        public decimal Balance { get; set; }
        public List<Transfer> Transfers { get; set; }

        public Account(Guid number)
        {
            Number = number;
        }

        public Account()
        {
        }

        public Account(string name) : this()
        {
            Name = name;
            Number = Guid.NewGuid();
            Balance = 1000.00m;
        }
    }
}
