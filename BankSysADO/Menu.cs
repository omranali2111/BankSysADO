using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSysADO
{
    internal class Menu
    {
        private UserRegistration userRegistration;
        private ExchangeRateService exchangeRateService;
        private AccountOperation accountOperation;
        private Users currentUser;
        bool loginSuccessful;

        public Menu()
        {
        }

        public Menu(UserRegistration userRegistration, ExchangeRateService exchangeRateService, AccountOperation accountOperation)
        {
            this.userRegistration = userRegistration;
            this.exchangeRateService = exchangeRateService;
            this.accountOperation = accountOperation; 
        }



        public async Task Start()
        {

        
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("Bank System Menu:");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. View Exchange Rates");
                Console.WriteLine("4. Convert Currency"); 
                Console.WriteLine("5. Exit");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        userRegistration.RegisterUser();
                        break;
                    case "2":
                        loginSuccessful = await userRegistration.UserLogin(); // Use await to get the result
                        if (loginSuccessful)
                        {
                            currentUser = userRegistration.GetCurrentUser();
                            if (currentUser != null)
                            {
                                Console.WriteLine("Login successful!");
                                AccountMenu();
                            }
                            else
                            {
                                Console.WriteLine("Failed to get the current user.");
                            }
                        }
                        break;

                    case "3":
                   
                        await exchangeRateService.ViewExchangeRateData();
                      
                        break;

                    case "4":
                        await exchangeRateService.ConvertCurrency(); 
                        break;
                    case "5":
                        exit = true;
                        Console.WriteLine("Exiting the application. Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private void AccountMenu()
        {
            bool exitAccountMenu = false;

            while (!exitAccountMenu)
            {
                Console.Clear();
                Console.WriteLine("Account Menu:");
                Console.WriteLine("1. Create Account");
                Console.WriteLine("2. View Account");
                Console.WriteLine("3. Make a new transaction");
                Console.WriteLine("4. View Transaction History"); 
                Console.WriteLine("5. Back to Main Menu");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        accountOperation.AddAccount(currentUser.UserId);
                        break;
                    case "2":
                        accountOperation.ViewAccountsForUser(currentUser.UserId);
                        break;
                    case "3":
                        TransactionMenu(); 
                        break;
                    case "4":
                        ViewTransactionHistoryMenu(); 
                        break;
                    case "5":
                        exitAccountMenu = true;
                        Console.WriteLine("Returning to Main Menu.");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }

           
        }
        private void ViewTransactionHistoryMenu()
        {
            Console.WriteLine("View Transaction History:");
            Console.WriteLine("1. Last transaction");
            Console.WriteLine("2. Last day");
            Console.WriteLine("3. Last 5 days");
            Console.WriteLine("4. Last 1 month");
            Console.WriteLine("5. Last 2 months");
            Console.WriteLine("6. All");
            Console.WriteLine("7. Back to Account Menu");

            Console.WriteLine("Enter the number corresponding to the desired period or option: ");
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                string period;
                switch (choice)
                {
                    case 1:
                        period = "last transaction";
                        break;
                    case 2:
                        period = "last day";
                        break;
                    case 3:
                        period = "last 5 days";
                        break;
                    case 4:
                        period = "last 1 month";
                        break;
                    case 5:
                        period = "last 2 months";
                        break;
                    case 6:
                        period = "all";
                        break;
                    case 7:
                        Console.WriteLine("Returning to Account Menu.");
                        return; // Exit the method
                    default:
                        period = "all";
                        break;
                }
                accountOperation.ViewTransactionHistory(currentUser.UserId, period);
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }


        private void TransactionMenu()
        {
            bool exitTransactionMenu = false;

            while (!exitTransactionMenu)
            {
                Console.Clear();
                Console.WriteLine("Transaction Menu:");
                Console.WriteLine("1. Withdraw");
                Console.WriteLine("2. Deposit");
                Console.WriteLine("3. Transfer");
                Console.WriteLine("4. Back to Account Menu");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        accountOperation.Withdraw(currentUser.UserId);
                        break;
                    case "2":
                        accountOperation.Deposit(currentUser.UserId);

                        break;
                    case "3":
                        accountOperation.Transfer(currentUser.UserId);

                        break;
                    case "4":
                        exitTransactionMenu = true;
                        Console.WriteLine("Returning to Account Menu.");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }
}
