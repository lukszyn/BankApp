using System;
using BankApp.DataLayer.Model;
using BankApp.DataLayer;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace BankApp.BusinessLayer
{
    public class AccountsService
    {
        public void MakeTransfer(Transfer transfer)
        {
            using (var context = new BankAppDbContext())
            {
                var senderAccount = GetAccountById(transfer.AccountId);
                new TransfersService().Add(transfer);
                UpdateBalance(senderAccount.Id, transfer.Amount);

                if (transfer.Type == "domestic")
                {
                    var receiverAccount = GetAccountByNumber(transfer.Receiver);
                    UpdateBalance(receiverAccount.Id, -transfer.Amount);
                }

                context.SaveChanges();
            }

        }
        public bool CheckIfTransactorExists(Account transactor)
        {
            return transactor == null ? false : true;
        }

        public bool CheckIfValidReceiver(Account sender, Account receiver)
        {

            using (var context = new BankAppDbContext())
            {
                return !context.Accounts.Any(account => account.Id == sender.Id && account.Id == receiver.Id);
            }
        }

        public bool CheckIfValidAmount(Account sender, decimal amount)
        {
            using (var context = new BankAppDbContext())
            {
                var senderAccount = context.Accounts.FirstOrDefault(account => account.Id == sender.Id);

                if (senderAccount.Balance - amount < 0)
                {
                    return false;
                }

                return true;
            }
        }

        public void Add(string name)
        {
            using (var context = new BankAppDbContext())
            {
                context.Accounts.Add(new Account(name));
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

        public bool UpdateBalance(int accountId, decimal amount)
        {
            using (var context = new BankAppDbContext())
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

        public List<Account> GetAllAccounts()
        {
            using (var context = new BankAppDbContext())
            {
                return context.Accounts.ToList();
            }
        }

        public Account GetAccountByName(string name)
        {
            using (var context = new BankAppDbContext())
            {
                return context.Accounts.FirstOrDefault(account => account.Name == name);
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
    }
}
