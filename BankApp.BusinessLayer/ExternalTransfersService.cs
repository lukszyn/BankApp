using BankApp.DataLayer.Models;
using System.Collections.Generic;

namespace BankApp.BusinessLayer
{
    public static class ExternalTransfersService
    {
        private static List<Transfer> _transfers = new List<Transfer>();

        public static void Add(Transfer transfer)
        {
            _transfers.Add(transfer);
        }

        public static List<Transfer> GetAll()
        {
            return _transfers;
        }

        public static void Remove()
        {
            _transfers.Clear();
        }

    }
}
