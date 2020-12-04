using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApp.DataLayer.Model
{
    public class Transfer
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public string Receiver { get; set; }
        public decimal Amount { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }


        public Transfer()
        {
        }

        public Transfer(int accountId, Account receiver, decimal amount, string name, string type)
        {
            AccountId = accountId;
            Receiver = receiver.Number.ToString();
            Amount = amount;
            Name = name;
            Type = type;
            Date = DateTime.Now;
        }
    }
}
