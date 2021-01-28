using System;
using BankApp.DataLayer.Models;
using BankApp.DataLayer;
using System.Linq;
using System.Collections.Generic;
using BankingProject.OutgoingTransfers.Sender;
using BankApp.BusinessLayer.Serializers;
using BankApp.BusinessLayer.Models;

namespace BankApp.BusinessLayer
{
    public class AccountsService
    {
        private TransfersService _transfersService;
        private Func<IBankAppDbContext> _dbContextFactoryMethod;

        public AccountsService(TransfersService transfersService,
                Func<IBankAppDbContext> dbContextFactoryMethod)
        {
            _transfersService = transfersService;
            _dbContextFactoryMethod = dbContextFactoryMethod;
        }

        public void MakeTransfer(Transfer transfer)
        {
            var senderAccount = GetAccountById(transfer.AccountId);
            _transfersService.AddOutgoingTransfer(senderAccount.Id, transfer);
            UpdateBalance(senderAccount.Id, transfer.Amount);

            if (!CheckIfExternal(transfer.ReceiverId))
            {
                var incomingTransfer = new Transfer(transfer.AccountId,
                    transfer.ReceiverId,
                    transfer.Amount,
                    transfer.Name,
                    TransferType.INCOMING);

                _transfersService.AddIncomingTransfer(transfer.ReceiverId, incomingTransfer);
                UpdateBalance(transfer.ReceiverId, -transfer.Amount);
            }
            else
            {
                SendJsonInfo(transfer);
            }
        }

        private void SendJsonInfo(Transfer transfer)
        {
            transfer.Date = DateTime.Now;

            JsonSerializer jsonSerializer = new JsonSerializer();
            var transferJson = jsonSerializer.Serialize(transfer);

            var sender = new GlobalOutgoingTransfersSender();
            sender.Send(transferJson);
        }

        public bool CheckIfValidReceiver(Account sender, Account receiver)
        {
            using (var context = _dbContextFactoryMethod())
            {
                return !context.Accounts.Any(account => account.Id == sender.Id && account.Id == receiver.Id);
            }
        }

        public bool CheckIfSufficientFunds(Account sender, decimal amount)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var senderAccount = context.Accounts.FirstOrDefault(account => account.Id == sender.Id);
                return (senderAccount.Balance - amount < 0) ? false : true;
            }
        }

        public void Add(User user, string name)
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.Accounts.Add(new Account(user.Id, name));
                context.SaveChanges();
            }
        }

        public ICollection<Account> GetAllUserAccounts(User user)
        {
            using (var context = new BankAppDbContext())
            {
                return context.Accounts.Where(account => account.UserId == user.Id).ToList();
            }
        }

        public Account GetAccountByName(User user, string name)
        {
            using (var context = new BankAppDbContext())
            {
                return context.Accounts.FirstOrDefault(account => account.Name == name && account.UserId == user.Id);
            }
        }

        public Account GetAccountByNumber(string number)
        {
            using (var context = new BankAppDbContext())
            {
                return context.Accounts.FirstOrDefault(account => account.Number.ToString() == number);
            }
        }

        public Account GetAccountById(int id)
        {
            using (var context = new BankAppDbContext())
            {
                return context.Accounts.FirstOrDefault(account => account.Id == id);
            }
        }

        public bool UpdateBalance(int? accountId, decimal amount)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var account = context.Accounts.FirstOrDefault(account => account.Id == accountId);

                if (account == null)
                {
                    return false;
                }

                account.Balance -= amount;
                context.SaveChanges();
            }

            return true;
        }

        public bool CheckIfExternal(int? id)
        {
            using (var context = _dbContextFactoryMethod())
            {
                return context.Accounts.Any(account => account.Id == id && account.UserId == null);
            }
        }

        public bool CheckIfExternal(string number)
        {
            using (var context = _dbContextFactoryMethod())
            {
                return !context.Accounts.Any(account => account.Number.ToString() == number && account.UserId != null);
            }
        }

        public void AddToExternalTransfers(Transfer transfer)
        {
            ExternalTransfersService.Add(transfer);
        }

        public void ExecuteExternalTransfers()
        {
            var transfersToExecute = ExternalTransfersService.GetAll();

            if (transfersToExecute != null)
            {
                foreach (var transfer in transfersToExecute)
                {
                    MakeTransfer(transfer);
                }
            }

            ExternalTransfersService.Remove();
        }

        public AccountStatement CreateAccountStatement(Account account)
        {
            var accountStatement = new AccountStatement();

            accountStatement.OutgoingTransfersStatement = _transfersService.GetOutgoingTransfersStatement(account);
            accountStatement.IncomingTransfersStatement = _transfersService.GetIncomingTransfersStatement(account);
            accountStatement.OutgoingTransfersSum = _transfersService.GetOutgoingTransfersSum(account);
            accountStatement.IncomingTransfersSum = _transfersService.GetIncomingTransfersSum(account);
            accountStatement.MaxTransfer = _transfersService.GetMaxTransfer(account);
            accountStatement.MinTransfer = _transfersService.GetMinTransfer(account);
            accountStatement.AverageTransfer = _transfersService.GetAverageTransfer(account);

            return accountStatement;
        }
    }
}
