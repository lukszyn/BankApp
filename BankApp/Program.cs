using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BankApp
{
    class Program
    {
        public static List<Account> accounts = new List<Account>();
        static void Main(string[] args)
        {
            do
            {
                Console.WriteLine();
                Console.WriteLine("Choose action: ");
                Console.WriteLine("Press 1 to create an account");
                Console.WriteLine("Press 2 to make a domestic transfer");
                Console.WriteLine("Press 3 to make an outgoing transfer");
                Console.WriteLine("Press 4 to display your accounts\' balance");
                Console.WriteLine("Press 5 to display transfers\' history");
                Console.WriteLine("Press 0 to Exit");

                int userChoice = GetIntFromUser("Select option");

                switch (userChoice)
                {
                    case 0:
                        return;
                    case 1:
                        CreateAccount();
                        break;
                    case 2:
                        HandleDomesticTransfer();
                        break;
                    case 3:
                        HandleOutgoingTransfer();
                        break;
                    case 4:
                        PrintAccountsBalance();
                        break;
                    case 5:
                        PrintAccountHistory();
                        break;
                    default:
                        Console.WriteLine("Unknown option");
                        break;
                }
            }

            while (true);
        }

        private static void HandleDomesticTransfer()
        {

            if (accounts.Count < 2)
            {
                Console.WriteLine("\nYou need to have at least two accounts to make a domestic transfer!");
            }

            else
            {
                try
                {
                    Account sender = ProvideTransactorData("Provide name of the account you want to send money from");
                    Account receiver = ProvideTransactorData("Provide name of the account you want to send money to");

                    if (sender != null && receiver != null)
                    {
                        decimal amount = GetDecimalFromUser("Input a transfer amount");
                        string transferName = GetTextFromUser("Input a transfer name");

                        sender.MakeTransfer(receiver, amount, transferName, "domestic");

                    }

                    else
                    {
                        Console.WriteLine("Invalid data. Try again.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                
            }
        }

        private static void HandleOutgoingTransfer()
        {
            if (accounts.Count < 1)
            {
                Console.WriteLine("\nYou need to have at least one account to make an outgoing transfer!");
                return;
            }

            else
            {
                try
                {
                    Account sender = ProvideTransactorData("Provide name of the account you want to send money from");
                    Guid receiverId = Guid.Parse(GetTextFromUser("Provide account number of the account you want to send money to"));
                    Account receiver = new Account(receiverId);

                    if (sender != null)
                    {
                        decimal amount = GetDecimalFromUser("Input a transfer amount");
                        string transferName = GetTextFromUser("Input a transfer name");

                        sender.MakeTransfer(receiver, amount, transferName, "external");
                    }
                    else
                    {
                        Console.WriteLine("Invalid data. Try again.");
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

        }

        private static Account ProvideTransactorData(string message)
        {
            string transactorId = GetTextFromUser(message);
            Account transactor = GetAccountFromList(transactorId);

            if (transactor != null)
            {
                return transactor;
            }
            else
            {
                throw new NullReferenceException("\nAn account with given name does not exist.");
            }
        }

        private static Account GetAccountFromList(string id)
        {
            foreach (Account account in accounts)
            {
                if (account.Name == id || account.AccountNumber.ToString() == id)
                {
                    return account;
                }
            }

            return null;
        }

        public static void CreateAccount()
        {
            string name = GetTextFromUser("Provide a name of your account");

            if (GetAccountFromList(name) == null)
            {
                accounts.Add(new Account(name));
                Console.WriteLine($"\nAccount {name} created successfully.");
            }
            else
            {
                Console.WriteLine($"\nAn account with name {name} already exists.");
            }

        }

        private static void PrintAccountsBalance()
        {
            if (accounts.Count == 0)
            {
                Console.WriteLine("\nNo accounts has been created.");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("\n{0,-20}{1,-40} {2,-20}\n", "Account Name", "Account Number", "Balance"));

            foreach (Account account in accounts)
            {
                sb.Append(account.ToString());
            }

            Console.WriteLine(sb);
        }

        public static void PrintAccountHistory()
        {
            if (accounts.Count == 0)
            {
                Console.WriteLine("\nNo accounts has been created");
                return;
            }

            try
            {

                foreach (Account account in accounts)
                {
                    if (account.TransferList.Count == 0)
                    {
                        Console.WriteLine("\nNo transfers has been sent");
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.Append(string.Format("{0, -10} {1, -20}\n", "Nazwa konta: ", account.Name));
                    sb.Append(string.Format(CultureInfo.GetCultureInfo("en-US"), "\n{0,-19} {1,-36} {2,-36} {3,-25} {4,-20} {5,-20}\n",
                        "Transfer Date", "Receiver Account Number", "Sender Account Number", "Transfer Name", "Transfer Amount", "Transfer Type"));

                    foreach (Transfer transfer in account.TransferList)
                    {
                        sb.Append(transfer.ToString());
                    }

                    Console.WriteLine(sb);
                }

            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static string GetTextFromUser(string message)
        {
            Console.Write($"{message}: ");
            return Console.ReadLine();
        }

        private static int GetIntFromUser(string message)
        {
            int number;
            while (!int.TryParse(GetTextFromUser(message), out number))
            {
                Console.WriteLine("Not na integer - try again.");
            }

            return number;
        }

        private static decimal GetDecimalFromUser(string message)
        {
            decimal number;

            while (!decimal.TryParse(GetTextFromUser(message), out number))
            {
                Console.WriteLine("Not a floating point number - try again.");
            }

            return number;

        }
    }

}
