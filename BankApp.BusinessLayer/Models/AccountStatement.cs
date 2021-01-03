using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.BusinessLayer.Models
{
    public class AccountStatement
    {
        public Dictionary<string, decimal> OutgoingTransfersStatement { get; set; }
        public Dictionary<string, decimal> IncomingTransfersStatement { get; set; }
        public decimal OutgoingTransfersSum { get; set; }
        public decimal IncomingTransfersSum { get; set; }
        public decimal MaxTransfer { get; set; }
        public decimal MinTransfer { get; set; }
        public decimal AverageTransfer { get; set; }
    }
}
