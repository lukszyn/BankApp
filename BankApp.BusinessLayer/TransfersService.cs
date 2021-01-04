using System;
using System.Collections.Generic;
using System.Linq;
using BankApp.BusinessLayer.Models;
using BankApp.DataLayer;
using BankApp.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApp.BusinessLayer
{
    public class TransfersService
    {
        ITransferRepository _transferRepository;

        public TransfersService(ITransferRepository transferRepository)
        {
            _transferRepository = transferRepository;
        }

        public void AddOutgoingTransfer(int accountId, Transfer transfer)
        {
            using (var context = new BankAppDbContext())
            {
                context.Accounts
                    .Include(account => account.OutgoingTransfers)
                    .FirstOrDefault(acc => acc.Id == accountId)
                    .OutgoingTransfers
                    .Add(transfer);
                context.SaveChanges();
            }
        }

        public void AddIncomingTransfer(int? accountId, Transfer transfer)
        {
            using (var context = new BankAppDbContext())
            {
                context.Accounts
                    .Include(account => account.IncomingTransfers)
                    .FirstOrDefault(acc => acc.Id == accountId)
                    .IncomingTransfers
                    .Add(transfer);
                context.SaveChanges();
            }
        }

        public List<Transfer> GetAll(Account account)
        {
            var incomingTransfers = _transferRepository.GetIncomingTransfers(account);
            var outgoingTransfers = _transferRepository.GetOutgoingTransfers(account);

            if (!incomingTransfers.Any() && !outgoingTransfers.Any()) return null; 

            return incomingTransfers.Concat(outgoingTransfers).OrderBy(t => t.Date).ToList();
        }

        internal Dictionary<string, decimal> GetOutgoingTransfersStatement(Account account)
        {
            var outgoingStatement = new Dictionary<string, decimal>();
            var outgoingTransfers = _transferRepository.GetOutgoingTransfers(account);

            if (!outgoingTransfers.Any()) return outgoingStatement;

            foreach (var transfer in outgoingTransfers)
            {
                var key = transfer.Receiver.Number.ToString();

                if (outgoingStatement.ContainsKey(key))
                {
                    outgoingStatement[key] += transfer.Amount;
                }
                else
                {
                    outgoingStatement.Add(key, transfer.Amount);
                }
            }

            return outgoingStatement;
        }

        internal Dictionary<string, decimal> GetIncomingTransfersStatement(Account account)
        {
            var incomingStatement = new Dictionary<string, decimal>();
            var incomingTransfers = _transferRepository.GetIncomingTransfers(account);

            if (!incomingTransfers.Any()) return incomingStatement;

            foreach (var transfer in incomingTransfers)
            {
                var key = transfer.Account.Number.ToString();

                if (incomingStatement.ContainsKey(key))
                {
                    incomingStatement[key] += transfer.Amount;
                }
                else
                {
                    incomingStatement.Add(key, transfer.Amount);
                }
            }

            return incomingStatement;
        }

        internal decimal GetOutgoingTransfersSum(Account account)
        {
            return _transferRepository.GetOutgoingTransfers(account)
                .Select(x => x.Amount)
                .DefaultIfEmpty(0)
                .Sum();
        }

        internal decimal GetIncomingTransfersSum(Account account)
        {
            return _transferRepository.GetIncomingTransfers(account)
                .Select(x => x.Amount)
                .DefaultIfEmpty(0)
                .Sum();
        }

        internal decimal GetMaxTransfer(Account account)
        {
            return _transferRepository.GetOutgoingTransfers(account)
                .Select(x => x.Amount)
                .DefaultIfEmpty(0)
                .Max();
        }

        internal decimal GetMinTransfer(Account account)
        {
            return _transferRepository.GetOutgoingTransfers(account)
                .Select(x => x.Amount)
                .DefaultIfEmpty(0)
                .Min();
        }

        internal decimal GetAverageTransfer(Account account)
        {
            return _transferRepository.GetOutgoingTransfers(account)
                .Select(x => x.Amount)
                .DefaultIfEmpty(0)
                .Average();
        }
    }
}
