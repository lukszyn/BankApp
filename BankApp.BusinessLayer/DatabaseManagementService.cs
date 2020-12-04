using BankApp.DataLayer;

namespace BankApp.BusinessLayer
{
    public class DatabaseManagementService
    {
        public void EnsureDatabaseCreation()
        {
            using (var context = new BankAppDbContext())
            {
                context.Database.EnsureCreated();
            }
        }
    }
}