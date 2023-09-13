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
        //private AccountOperations accountOperations;
        private Users currentUser;
        bool loginSuccessful;

        public Menu(UserRegistration userRegistration)
        {
            this.userRegistration = userRegistration;
          
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
                Console.WriteLine("3. View Exchange Rates"); // New option to view exchange rates
                Console.WriteLine("4. Exit");

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
                   
                        await userRegistration.ViewExchangeRates(); // Call the method asynchronously
                        break;
                        
                    case "4":
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
            // Implement your account-related menu here
        }
    }
}
