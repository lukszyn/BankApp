using BankApp.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace BankApp.DataLayer
{
    public interface IBankAppDbContext : IDisposable
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<User> Users { get; set; }
        int SaveChanges();
    }

    public class BankAppDbContext : DbContext, IBankAppDbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.;Database=BankAppDB;Trusted_Connection=True");
        }
    }
}
