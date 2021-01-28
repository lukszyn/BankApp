using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApp.DataLayer.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public Guid Number { get; set; }
        public decimal? Balance { get; set; }

        [InverseProperty("Account")]
        public ICollection<Transfer> OutgoingTransfers { get; set; }

        [InverseProperty("Receiver")]
        public ICollection<Transfer> IncomingTransfers { get; set; }


        public int? UserId { get; set; }
        public User User{ get; set; }

        public Account(Guid number)
        {
            Number = number;
        }

        public Account()
        {
        }

        public Account(int userId, string name)
        {
            UserId = userId;
            Name = name;
            Number = Guid.NewGuid();
            Balance = 1000.00m;
        }
    }
}
