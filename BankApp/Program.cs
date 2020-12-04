using System;
using System.Text;
using BankApp.BusinessLayer;
using BankApp.DataLayer.Model;

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

            do
            {
                _loggingMenu.PrintAvailableOptions();

                userChoice = _ioHelper.GetIntFromUser("Choose action");

                _loggingMenu.ExecuteOption(userChoice);

                if (userChoice == 0) return;
                else if (_loggedUser != null) break;
            }
            while (userChoice != 0 || _loggedUser == null);

            RegisterMenuOptions();

            do
            {
                _menu.PrintAvailableOptions();

                userChoice = _ioHelper.GetIntFromUser("Choose action");

                _menu.ExecuteOption(userChoice);

                if (userChoice == 0) return;
            }
            while (userChoice != 0);

        }

        private void RegisterLogMenuOptions()
        {
            _loggingMenu.AddOption(new MenuItem { Key = 1, Action = SignUp, Description = "Sign up to the BankApp" });
            _loggingMenu.AddOption(new MenuItem { Key = 2, Action = SignIn, Description = "Sign in to your BankApp account" });
        }

        private void RegisterMenuOptions()
        {
            _menu.AddOption(new MenuItem { Key = 3, Action = CreateAccount, Description = "Create an account" });
            _menu.AddOption(new MenuItem { Key = 4, Action = HandleDomesticTransfer, Description = "Make a domestic transfer" });
            _menu.AddOption(new MenuItem { Key = 5, Action = HandleExternalTransfer, Description = "Make an outgoing transfer" });
            _menu.AddOption(new MenuItem { Key = 6, Action = PrintAccountsBalance, Description = "Display your accounts\' balance" });
            _menu.AddOption(new MenuItem { Key = 7, Action = PrintAccountHistory, Description = "Display transfers\' history" });
        }

        private void SignUp()
        {
            string email = _ioHelper.GetTextFromUser("Provide an email");

            if (!_ioHelper.ValidateEmail(email))
            {
                Console.WriteLine("Email must contain \'@\' character!");
                return;
            }

            string password = _ioHelper.GetTextFromUser("Provide a password (minimum 6 characters)");

            if (!_ioHelper.ValidatePassword(password))
            {
                Console.WriteLine("Password must have at least 6 characters!");
                return;
            }

            string phoneNumber = _ioHelper.GetTextFromUser("Provide a phone number");

            if (!_ioHelper.ValidatePhoneNumber(phoneNumber))
            {
                Console.WriteLine("Phone number must consist of 9 digits!");
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
            string email = _ioHelper.GetTextFromUser("Provide an email");
            string password = _ioHelper.GetTextFromUser("Provide a password");

            if(!_usersService.CheckCredentials(email, password))
            {
                Console.WriteLine("Invalid email or password.\n");
                return;
            }

            _loggedUser = _usersService.Get(email);

        }

        private void CreateAccount()
        {
            string name = _ioHelper.GetTextFromUser("Provide a name of your account");

            if (_accountsService.GetAccountByName(name) == null)
            {
                _accountsService.Add(name);
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
            if (_accountsService.GetAllAccounts().Count < accountsNeeded)
            {
                Console.WriteLine($"You need to have at least {accountsNeeded} account(s) to make a {type} transfer!\n");
            }

            var sender = ProvideSender("Provide name of the account you want to send money from");

            if (!_accountsService.CheckIfTransactorExists(sender))
            {
                Console.WriteLine("An account with given identifier does not exist!\n");
                return;
            }

            Account receiver;

            if (type == "external")
            {
                Guid receiverId;

                if (!Guid.TryParse(_ioHelper.GetTextFromUser("Provide account number of the account you want to send money to"), out receiverId))
                {
                    Console.WriteLine("Invalid account number.");
                    return;
                }

                receiver = new Account()
                {
                    Number = receiverId
                };
            }

            else
            {
                receiver = ProvideReceiver("Provide name of the account you want to send money to", type);

                if(!_accountsService.CheckIfTransactorExists(receiver))
                {
                    Console.WriteLine("An account with given identifier does not exist!\n");
                    return;
                }
            }

            if (!_accountsService.CheckIfValidReceiver(sender, receiver))
            {
                Console.WriteLine("Receiver account is the same!\n");
                return;
            }

            decimal amount = _ioHelper.GetDecimalFromUser("Input a transfer amount");

            if (!_accountsService.CheckIfValidAmount(sender, amount))
            {
                return;
            }

            var transferName = _ioHelper.GetTextFromUser("Input a transfer name");
            var transfer = new Transfer(sender.Id, receiver, amount, transferName, type);

            _accountsService.MakeTransfer(transfer);
            Console.WriteLine("Transfer executed successfully.\n");

        }

        private Account ProvideSender(string message)
        {
            string transactorId = _ioHelper.GetTextFromUser(message);
            return _accountsService.GetAccountByName(transactorId);
        }

        private Account ProvideReceiver(string message, string type)
        {
            string receiverId = _ioHelper.GetTextFromUser(message);
            Account receiver;

            if (type == "external")
            {
                receiver = _accountsService.GetAccountByNumber(receiverId);
            }
            else
            {
                receiver = _accountsService.GetAccountByName(receiverId);
            }

            return receiver;
        }

        private void PrintAccountsBalance()
        {
            var accounts = _accountsService.GetAllAccounts();

            if (accounts.Count == 0)
            {
                Console.WriteLine("No accounts has been created.\n");
                return;
            }

            _ioHelper.PrintAccountsBalance(accounts);
        }

        private void PrintAccountHistory()
        {
            var accounts = _accountsService.GetAllAccounts();

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
