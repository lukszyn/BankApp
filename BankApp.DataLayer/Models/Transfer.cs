using Newtonsoft.Json;
using System;

namespace BankApp.DataLayer.Models
{
    public enum TransferType
    {
        EXTERNAL,
        INTERNAL,
        INCOMING
    }

    public class Transfer
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        [JsonIgnore]
        public Account Account { get; set; }
        [JsonIgnore]
        public int? ReceiverId { get; set; }
        [JsonIgnore]
        public Account Receiver { get; set; }
        public decimal Amount { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public TransferType Type { get; set; }

        public Transfer()
        {
        }

        public Transfer(int accountId, int? receiverId, decimal amount, string name, TransferType type)
        {
            AccountId = accountId;
            ReceiverId = receiverId;
            Amount = amount;
            Name = name;
            Type = type;
            Date = DateTime.Now;
        }
    }
}
