using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BankApp
{
    class Account
    {
        public string Name { get; private set; }
        public Guid AccountNumber { get; }
        public decimal AccountBalance { get; private set; }
        public List<Transfer> TransferList { get; }

        public Account(Guid number)
        {
            AccountNumber = number;
        }

        public Account(string name)
        {
            Name = name;
            AccountNumber = Guid.NewGuid();
            AccountBalance = 1000.00m;
            TransferList = new List<Transfer>();
        }

        public void MakeTransfer(Account receiver, decimal amount, string name, string type)
        {
            try
            {
                CheckIfValidAmount(amount);

                if (type == "domestic")
                {
                    CheckIfValidReceiver(receiver);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            AccountBalance -= amount;
            Transfer transfer = new Transfer(this, receiver, amount, name, type);
            TransferList.Add(transfer);

            if (type == "domestic")
            {
                receiver.AccountBalance += amount;
            }

            Console.WriteLine("\nTransfer executed successfully.");
        }

        private void CheckIfValidReceiver(Account receiver)
        {
            if (object.ReferenceEquals(receiver, this))
            {
                throw new ArgumentException("Receiver account is the same");
            }
        }

        private void CheckIfValidAmount(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException("Amount of transfer must be positive");
            }
            else if (AccountBalance - amount < 0)
            {
                throw new InvalidOperationException("Not sufficient funds for this transfer");
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format(CultureInfo.GetCultureInfo("en-US"), "{0,-20}{1,-40} {2,-20:C2}", Name, AccountNumber, AccountBalance));
            sb.Append("\n");
            return sb.ToString();
        }

    }
}
