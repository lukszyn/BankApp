using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;

namespace BankApp
{
    static class Program
    {
        private static List<Account> accounts = new List<Account>();
        static void Main(string[] args)
        {
            int userChoice;

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

                userChoice = GetIntFromUser("Select option");

                switch (userChoice)
                {
                    case 0:
                        return;
                    case 1:
                        CreateAccount();
                        break;
                    case 2:
                        HandleTransfer("domestic", 2);
                        break;
                    case 3:
                        HandleTransfer("external", 1);
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
            while (userChoice != 0);
        }

        private static void HandleTransfer(string type, int accountsNeeded)
        {
            if (accounts.Count < accountsNeeded)
            {
                Console.WriteLine($"\nYou need to have at least {accountsNeeded} accounts to make a {type} transfer!");
            }

            Account sender = ProvideTransactorData("Provide name of the account you want to send money from");
            Account receiver;

            if (type =="external")
            {
                Guid receiverId = Guid.Parse(GetTextFromUser("Provide account number of the account you want to send money to"));
                receiver = new Account(receiverId);
            }

            receiver = ProvideTransactorData("Provide name of the account you want to send money to");

            if (sender != null && receiver != null)
            {
                ExecuteTransfer(sender, receiver, type);
            }

        }

        private static void ExecuteTransfer(Account sender, Account receiver, string type)
        {
            decimal amount = GetDecimalFromUser("Input a transfer amount");
            string transferName = GetTextFromUser("Input a transfer name");

            sender.MakeTransfer(receiver, amount, transferName, type);
        }


        private static Account ProvideTransactorData(string message)
        {
            string transactorId = GetTextFromUser(message);
            Account transactor = GetAccountFromList(transactorId);

            if (transactor == null)
            {
                Console.WriteLine("\nAn account with given name does not exist.");
            }

            return transactor;
        }

        private static Account GetAccountFromList(string id)
        {
            foreach (Account account in accounts)
            {
                if (account.Name == id || account.Number.ToString() == id)
                {
                    return account;
                }
            }
            return null;
        }

        private static void CreateAccount()
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
            sb.Append(string.Format("\n{0,-20}{1,-40}{2,-20}\n", "Account Name", "Account Number", "Balance"));

            foreach (Account account in accounts)
            {
                sb.Append(account.ToString());
            }

            Console.WriteLine(sb.ToString());
        }

        private static void PrintAccountHistory()
        {
            if (accounts.Count == 0)
            {
                Console.WriteLine("\nNo accounts has been created");
                return;
            }

            foreach (Account account in accounts)
            {
                if (account.TransferList.Count == 0)
                {
                    Console.WriteLine("\nNo transfers has been sent");
                }

                Console.WriteLine($"{"Nazwa konta: ",-10} {account.Name,-20}\n");
                
                StringBuilder sb = new StringBuilder();
                sb.Append($"\n{"Transfer Date",-19}" +
                          $" {"Receiver Account Number",-36}" +
                          $" {"Sender Account Number",-36}" +
                          $" {"Transfer Name",-25}" +
                          $" {"Transfer Amount",-20}" +
                          $" {"Transfer Type",-20}\n");

                foreach (Transfer transfer in account.TransferList)
                {
                    sb.Append(transfer.ToString());
                }

                Console.WriteLine(sb);
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
