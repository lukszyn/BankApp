using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BankApp
{
    class Transfer
    {
        private Account _sender;
        private Account _receiver;
        private decimal _amount;
        private string _name;
        private DateTime _transferDate;
        private string _type;

        public Transfer(Account sender, Account receiver, decimal amount, string name, string type)
        {
            _name = name;
            _sender = sender;
            _receiver = receiver;
            _amount = amount;
            _transferDate = DateTime.Now;
            _type = type;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format(CultureInfo.GetCultureInfo("en-US"), "{0,-0:dd/MM/yyyy HH:mm:ss} {1,-36} {2,-36} {3,-25} {4,-20:C2} {5,-20}\n", _transferDate,
                _receiver.AccountNumber, _sender.AccountNumber, _name, _amount, _type));
            return sb.ToString();
        }
    }
}
