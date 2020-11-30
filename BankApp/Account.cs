using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BankApp
{
    class Account
    {
        public string Name { get; private set; }
        public Guid Number { get; }
        public decimal Balance { get; private set; }
        public List<Transfer> TransferList { get; }

        public Account(Guid number)
        {
            Number = number;
        }

        public Account(string name)
        {
            Name = name;
            Number = Guid.NewGuid();
            Balance = 1000.00m;
            TransferList = new List<Transfer>();
        }

        public void MakeTransfer(Account receiver, decimal amount, string name, string type)
        {

            if (!CheckIfValidAmount(amount))
            {
                return;
            }

            if (type == "domestic")
            {
                CheckIfValidReceiver(receiver);
                Console.WriteLine("Receiver account is the same");
            }

            Balance -= amount;
            Transfer transfer = new Transfer(this, receiver, amount, name, type);
            TransferList.Add(transfer);
            receiver.TransferList.Add(transfer);

            if (type == "domestic")
            {
                receiver.Balance += amount;
            }

            Console.WriteLine("\nTransfer executed successfully.");
        }

        private bool CheckIfValidReceiver(Account receiver)
        {
            return !(object.ReferenceEquals(receiver, this) || receiver == null);
        }

        private bool CheckIfValidAmount(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("Amount of transfer must be positive");
                return false;
            }
            else if (Balance - amount < 0)
            {
                Console.WriteLine("Not sufficient funds for this transfer");
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return $"{Name,-20}{Number,-40}{Balance,-20:C2}\n";
        }
    }
}
