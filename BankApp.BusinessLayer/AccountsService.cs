using System;
using BankApp.DataLayer.Models;
using BankApp.DataLayer;
using System.Linq;
using System.Collections.Generic;

namespace BankApp.BusinessLayer
{
    public class AccountsService
    {
        public void MakeTransfer(User user, Transfer transfer)
        {
            using (var context = new BankAppDbContext())
            {
                var senderAccount = GetAccountById(user, transfer.AccountId);
                new TransfersService().Add(transfer);
                UpdateBalance(user, senderAccount.Id, transfer.Amount);

                if (transfer.Type == "domestic")
                {
                    var receiverAccount = GetAccountByNumber(user, transfer.Receiver);
                    UpdateBalance(user, receiverAccount.Id, -transfer.Amount);
                }

                context.SaveChanges();
            }
        }

        public bool CheckIfValidReceiver(Account sender, Account receiver)
        {
            using (var context = new BankAppDbContext())
            {
                return !context.Accounts.Any(account => account.Id == sender.Id && account.Id == receiver.Id);
            }
        }

        public bool CheckIfSufficientFunds(Account sender, decimal amount)
        {
            using (var context = new BankAppDbContext())
            {
                var senderAccount = context.Accounts.FirstOrDefault(account => account.Id == sender.Id);
                return (senderAccount.Balance - amount < 0) ? false : true;
            }
        }

        public void Add(User user, string name)
        {
            using (var context = new BankAppDbContext())
            {
                context.Accounts.Add(new Account(user.Id, name));
                context.SaveChanges();
            }
        }

        public void Add(Guid guid)
        {
            using (var context = new BankAppDbContext())
            {
                context.Accounts.Add(new Account(guid));
                context.SaveChanges();
            }
        }

        public bool UpdateBalance(User user, int accountId, decimal amount)
        {
            using (var context = new BankAppDbContext())
            {
                var account = context.Accounts.FirstOrDefault(account => account.Id == accountId && account.UserId == user.Id);

                if (account == null)
                {
                    return false;
                }

                account.Balance -= amount;
                context.SaveChanges();
            }

            return true;
        }

        public List<Account> GetAllAccounts(User user)
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

        public Account GetAccountByNumber(User user, string number)
        {
            using (var context = new BankAppDbContext())
            {
                return context.Accounts.FirstOrDefault(account => account.Number.ToString() == number && account.UserId == user.Id);
            }
        }

        public Account GetAccountById(User user, int id)
        {
            using (var context = new BankAppDbContext())
            {
                return context.Accounts.FirstOrDefault(account => account.Id == id && account.UserId == user.Id);
            }
        }
    }
}
