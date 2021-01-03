using BankApp.DataLayer;
using BankApp.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BankApp.BusinessLayer
{
    public interface ITransferRepository
    {
        public List<Transfer> GetOutgoingTransfers(Account account);
        public List<Transfer> GetIncomingTransfers(Account account);
    }

    public class TransferRepository : ITransferRepository
    {

        public List<Transfer> GetIncomingTransfers(Account account)
        {
            using (var context = new BankAppDbContext())
            {
                return context.Transfers.Include(t => (t.Receiver)).Include(t => t.Account)
                    .Where(t => (t.Receiver.Id == account.Id && t.Type == TransferType.INCOMING))
                    .ToList();
            }
        }

        public List<Transfer> GetOutgoingTransfers(Account account)
        {
            using (var context = new BankAppDbContext())
            {
                return context.Transfers.Include(t => (t.Receiver)).Include(t => t.Account)
                    .Where(t => t.Account.Id == account.Id && (t.Type == TransferType.INTERNAL
                    || t.Type == TransferType.EXTERNAL))
                    .ToList();
            }
        }
    }
}
