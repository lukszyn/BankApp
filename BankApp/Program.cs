using System;
using BankApp.BusinessLayer;
using BankApp.DataLayer;
using BankApp.DataLayer.Models;

namespace BankApp
{
    class Program
    {
        private Menu _loggingMenu = new Menu();
        private Menu _menu = new Menu();
        private IoHelper _ioHelper = new IoHelper();
        private AccountsService _accountsService = new AccountsService(new TransfersService(new TransferRepository()), () => new BankAppDbContext());
        private TransfersService _transfersService = new TransfersService(new TransferRepository());
        private UsersService _usersService = new UsersService();
        private DatabaseManagementService _databaseManagementService = new DatabaseManagementService();
        private User _loggedUser;
        private TimerService _timer = new TimerService();
        private FilesService _filesService = new FilesService();

        static void Main()
        {
            new Program().Run();
        }

        void Run()
        {
            _databaseManagementService.EnsureDatabaseCreation();
            _timer.SetTimer(_accountsService.ExecuteExternalTransfers, 1000 * 60 * 5);

            Console.WriteLine("Welcome to the BankApp.\n");
            RegisterLogMenuOptions();
            int userChoice;

            do
            {
                userChoice = GetUserOption(_loggingMenu);
                _loggingMenu.ExecuteOption(userChoice);

                if (userChoice == 0) return;
                else if (_loggedUser != null) break;
            }
            while (userChoice != 0 || _loggedUser == null);

            RegisterMenuOptions();

            do
            {
                userChoice = GetUserOption(_menu);

                _menu.ExecuteOption(userChoice);

                if (userChoice == 0) return;
            }
            while (userChoice != 0);

        }

        private int GetUserOption(Menu menu)
        {
            menu.PrintAvailableOptions();
            Console.WriteLine("Press 0 to exit.");
            return _ioHelper.GetIntFromUser("\nChoose action");
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
            _menu.AddOption(new MenuItem { Key = 6, Action = GenerateAccountStatement, Description = "Press 6 to Generate account\'s statement" });
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
            HandleTransfer(TransferType.INTERNAL, 2);
        }

        public void HandleExternalTransfer()
        {
            HandleTransfer(TransferType.EXTERNAL, 1);
        }

        public void HandleTransfer(TransferType type, int accountsNeeded)
        {
            if (_accountsService.GetAllUserAccounts(_loggedUser).Count < accountsNeeded)
            {
                Console.WriteLine($"You need to have at least {accountsNeeded} account(s) to make a {type} transfer!\n");
                return;
            }

            var sender = ProvideAccount("Provide name of the account you want to send money from");
            
            if (!CheckAccountExist(sender)) return;

            Account receiver = GetReceiver(type);

            if (!CheckAccountExist(receiver)) return;

            if (!_accountsService.CheckIfValidReceiver(sender, receiver))
            {
                Console.WriteLine("Receiver account is the same!\n");
                return;
            }

            var amount = _ioHelper.GetDecimalFromUser("Input a transfer amount");

            if (!ValidateAmount(sender, amount)) return;

            var transferName = _ioHelper.GetTextFromUser("Input a transfer name");
            var transfer = new Transfer(sender.Id, receiver.Id, amount, transferName, type);

            if (_accountsService.CheckIfExternal(receiver.Number.ToString()))
            {
                transfer.ReceiverId = null;
                transfer.Receiver = receiver;
                _accountsService.AddToExternalTransfers(transfer);
                Console.WriteLine("Transfer executed successfully.\n");
                return;
            }

            _accountsService.MakeTransfer(transfer);
            Console.WriteLine("Transfer executed successfully.\n");
        }

        private Account GetReceiver(TransferType type)
        {
            Account receiver;

            if (type == TransferType.EXTERNAL)
            {
                var extReceiver = ProvideExternalReceiver("Provide account number of the account you want to send money to");
                receiver = _accountsService.GetAccountByNumber(extReceiver.Number.ToString());

                if (receiver == null)
                {
                    receiver = new Account { Number = extReceiver.Number };
                }
            }
            else
            {
                receiver = ProvideAccount("Provide name of the account you want to send money to");
            }

            return receiver;
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

        private Account ProvideAccount(string message)
        {
            return _accountsService.GetAccountByName(_loggedUser, _ioHelper.GetTextFromUser(message));
        }

        private bool CheckAccountExist(Account transactor)
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
            var accounts = _accountsService.GetAllUserAccounts(_loggedUser);

            if (accounts.Count == 0)
            {
                Console.WriteLine("No accounts has been created.\n");
                return;
            }

            _ioHelper.PrintAccountsBalance(accounts);
        }

        private void PrintAccountHistory()
        {
            var accounts = _accountsService.GetAllUserAccounts(_loggedUser);

            if (accounts.Count == 0)
            {
                Console.WriteLine("No accounts has been created.\n");
                return;
            }

            foreach (var account in accounts)
            {
                var transfers = _transfersService.GetAll(account);
                _ioHelper.PrintAccountName(account);

                if (transfers == null)
                {
                    Console.WriteLine("No transfers has been sent.\n");
                    return;
                }

                _ioHelper.PrintTransfers(transfers);
            }
        }

        private void GenerateAccountStatement()
        {
            Account account = GetAccount();

            if (account == null)
            {
                return;
            }

            var accountStatement = _accountsService.CreateAccountStatement(account);

            _ioHelper.PrintStatement(accountStatement);

            var userAnswer = _ioHelper
                .GetTextFromUser("Press y to export account statement to file, press any other key to return to the menu")
                .ToLower();

            if (userAnswer != "y") return;
            
            if (_filesService.ExportToFile(_ioHelper.GetTextFromUser("Provide the path to save the file:"), accountStatement))
            {
                Console.WriteLine("Account statement exported successfully.\n");
            }
            else
            {
                Console.WriteLine("Error occurred during statement export.\n");
            }
        }

        private Account GetAccount()
        {
            
            if (_accountsService.GetAllUserAccounts(_loggedUser).Count < 1)
            {
                Console.WriteLine($"You need to have at least one account to make an account statement.\n");
                return null;
            }

            var account = ProvideAccount("Provide name of the account for which you want to generate the statement");

            if (!CheckAccountExist(account)) return null;

            return account;
        }
    }
}
