using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BankApp.DataLayer;
using BankApp.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApp.BusinessLayer
{
    public class TransfersService
    {
        public void Add(Transfer transfer)
        {
            using (var context = new BankAppDbContext())
            {
                context.Transfers.Add(transfer);
                context.SaveChanges();
            }
        }

        public List<Transfer> GetAll(Account account)
        {
            using (var context = new BankAppDbContext())
            {
                return context.Transfers.Include(account => account.Account).Where(transfer => transfer.Account.Id == account.Id).ToList();
            }
        }
    }
}
