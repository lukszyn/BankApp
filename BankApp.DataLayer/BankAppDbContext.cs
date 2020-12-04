using BankApp.DataLayer.Model;
using Microsoft.EntityFrameworkCore;

namespace BankApp.DataLayer
{
    public class BankAppDbContext : DbContext
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
