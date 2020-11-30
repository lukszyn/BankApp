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
            return $"{_transferDate,-0:dd/MM/yyyy HH:mm:ss} {_receiver.Number,-36} {_sender.Number,-36} {_name,-25} {_amount,-20:C2} {_type,-20}\n";
        }
    }
}
