using System;
using BankApp.BusinessLayer;
using BankApp.DataLayer.Models;

namespace BankApp
{
    class Program
    {
        private Menu _loggingMenu = new Menu();
        private Menu _menu = new Menu();
        private IoHelper _ioHelper = new IoHelper();
        private AccountsService _accountsService = new AccountsService();
        private TransfersService _transfersService = new TransfersService();
        private UsersService _usersService = new UsersService();
        private DatabaseManagementService _databaseManagementService = new DatabaseManagementService();
        private User _loggedUser;

        static void Main()
        {
            new Program().Run();
        }

        void Run()
        {
            _databaseManagementService.EnsureDatabaseCreation();
            RegisterLogMenuOptions();
            int userChoice;

            Console.WriteLine("Welcome to the BankApp.\n");

            do
            {
                _loggingMenu.PrintAvailableOptions();
                Console.WriteLine("Press 0 to exit.");
                userChoice = _ioHelper.GetIntFromUser("\nChoose action");

                _loggingMenu.ExecuteOption(userChoice);

                if (userChoice == 0) return;
                else if (_loggedUser != null) break;
            }
            while (userChoice != 0 || _loggedUser == null);

            RegisterMenuOptions();

            do
            {
                _menu.PrintAvailableOptions();
                Console.WriteLine("Press 0 to exit.");

                userChoice = _ioHelper.GetIntFromUser("\nChoose action");

                _menu.ExecuteOption(userChoice);

                if (userChoice == 0) return;
            }
            while (userChoice != 0);

        }

        private void RegisterLogMenuOptions()
        {
            _loggingMenu.AddOption(new MenuItem { Key = 1, Action = SignUp, Description = "Press 1 to Sign up to the BankApp" });
            _loggingMenu.AddOption(new MenuItem { Key = 2, Action = SignIn, Description = "Press 2 to Sign in to your BankApp account" });
        }

        private void RegisterMenuOptions()
        {
            _menu.AddOption(new MenuItem { Key = 1, Action = CreateAccount, Description = "Press 1 to Create an account" });
            _menu.AddOption(new MenuItem { Key = 2, Action = HandleDomesticTransfer, Description = "Press 2 to Make a domestic transfer" });
            _menu.AddOption(new MenuItem { Key = 3, Action = HandleExternalTransfer, Description = "Press 3 to Make an outgoing transfer" });
            _menu.AddOption(new MenuItem { Key = 4, Action = PrintAccountsBalance, Description = "Press 4 to Display your accounts\' balance" });
            _menu.AddOption(new MenuItem { Key = 5, Action = PrintAccountHistory, Description = "Press 5 to Display transfers\' history" });
        }

        private void SignUp()
        {
            var email = _ioHelper.GetTextFromUser("Provide an email");

            if (!_ioHelper.ValidateEmail(email))
            {
                Console.WriteLine("Email must contain \'@\' character!");
                return;
            }

            var password = _ioHelper.GetTextFromUser("Provide a password (minimum 6 characters)");

            if (!_ioHelper.ValidatePassword(password))
            {
                Console.WriteLine("Password must have at least 6 characters!\n");
                return;
            }

            var phoneNumber = _ioHelper.GetTextFromUser("Provide a phone number");

            if (!_ioHelper.ValidatePhoneNumber(phoneNumber))
            {
                Console.WriteLine("Phone number must consist of 9 digits!\n");
                return;
            }

            var newUser = new User(email, phoneNumber, password);

            if (_usersService.CheckIfUserExists(newUser.Email))
            {
                Console.WriteLine("User with given email already exists!\n");
                return;
            }

            _usersService.Add(newUser);
            Console.WriteLine("User account registered successfully!\n");

        }

        private void SignIn()
        {
            var email = _ioHelper.GetTextFromUser("Provide an email");
            var password = _ioHelper.GetTextFromUser("Provide a password");

            if(!_usersService.CheckCredentials(email, password))
            {
                Console.WriteLine("Invalid email or password.\n");
                return;
            }

            Console.WriteLine($"\nWelcome to your account, {email}\n");
            _loggedUser = _usersService.Get(email);
        }

        private void CreateAccount()
        {
            var name = _ioHelper.GetTextFromUser("Provide a name of your account");

            if (_accountsService.GetAccountByName(_loggedUser, name) == null)
            {
                _accountsService.Add(_loggedUser, name);
                Console.WriteLine($"Account {name} created successfully.\n");
            }
            else
            {
                Console.WriteLine($"An account with name {name} already exists.\n");
            }
        }

        public void HandleDomesticTransfer()
        {
            HandleTransfer("domestic", 2);
        }

        public void HandleExternalTransfer()
        {
            HandleTransfer("external", 1);
        }

        public void HandleTransfer(string type, int accountsNeeded)
        {
            if (_accountsService.GetAllAccounts(_loggedUser).Count < accountsNeeded)
            {
                Console.WriteLine($"You need to have at least {accountsNeeded} account(s) to make a {type} transfer!\n");
                return;
            }

            var sender = ProvideTransactor("Provide name of the account you want to send money from");
            Account receiver;

            if (!CheckTransactor(sender)) return;

            if (type == "external")
            {
                receiver = ProvideExternalReceiver("Provide account number of the account you want to send money to");
            }
            else
            {
                receiver = ProvideTransactor("Provide name of the account you want to send money to");
                if (!CheckTransactor(receiver)) return;
            }

            if (!_accountsService.CheckIfValidReceiver(sender, receiver))
            {
                Console.WriteLine("Receiver account is the same!\n");
                return;
            }

            var amount = _ioHelper.GetDecimalFromUser("Input a transfer amount");
            if (!ValidateAmount(sender, amount)) return;

            var transferName = _ioHelper.GetTextFromUser("Input a transfer name");
            var transfer = new Transfer(sender.Id, receiver, amount, transferName, type);

            _accountsService.MakeTransfer(_loggedUser, transfer);
            Console.WriteLine("Transfer executed successfully.\n");
        }

        private Account ProvideExternalReceiver(string message)
        {
            Guid receiverId;

            while (!Guid.TryParse(_ioHelper.GetTextFromUser(message), out receiverId))
            {
                Console.WriteLine("Invalid account number.");
            }

            return new Account() { Number = receiverId };
        }

        private Account ProvideTransactor(string message)
        {
            return _accountsService.GetAccountByName(_loggedUser, _ioHelper.GetTextFromUser(message));
        }

        private bool CheckTransactor(Account transactor)
        {
            if (transactor == null)
            {
                Console.WriteLine("An account with given identifier does not exist!\n");
                return false;
            }

            return true;
        }

        private bool ValidateAmount(Account sender, decimal amount)
        {
            if (_ioHelper.CheckIfNegative(amount))
            {
                Console.WriteLine("Given amount must be greater than 0!\n");
                return false;
            }

            else if (!_accountsService.CheckIfSufficientFunds(sender, amount))
            {
                Console.WriteLine("You do not have sufficient amount of cash on the account.\n");
                return false;
            }

            return true;
        }

        private void PrintAccountsBalance()
        {
            var accounts = _accountsService.GetAllAccounts(_loggedUser);

            if (accounts.Count == 0)
            {
                Console.WriteLine("No accounts has been created.\n");
                return;
            }

            _ioHelper.PrintAccountsBalance(accounts);
        }

        private void PrintAccountHistory()
        {
            var accounts = _accountsService.GetAllAccounts(_loggedUser);

            if (accounts.Count == 0)
            {
                Console.WriteLine("No accounts has been created.\n");
                return;
            }

            foreach (var account in accounts)
            {
                var transfers = _transfersService.GetAll(account);
                _ioHelper.PrintAccountName(account);

                if (transfers.Count == 0)
                {
                    Console.WriteLine("No transfers has been sent.\n");
                    return;
                }

                _ioHelper.PrintTransfers(transfers);
            }
        }
    }
}
