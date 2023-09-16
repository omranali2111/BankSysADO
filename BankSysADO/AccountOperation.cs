using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace BankSysADO
{
    internal class AccountOperation
    {
        public void AddAccount(int userId)
        {
            Console.WriteLine("Enter Account Holder's Name: ");
            string accountHolderName = Console.ReadLine();

            Console.WriteLine("Enter Current Balance: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal currentBalance))
            {
                string connectionString = "Data Source=(local);Initial Catalog=BankSystem; Integrated Security=true";

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    try
                    {
                        sqlConnection.Open();

                        // Define the SQL INSERT query with parameter placeholders
                        string insertQuery = "INSERT INTO dbo.Accounts (AccountHolderName, Balance, UserId) VALUES (@name, @balance, @userId)";

                        // Create and configure SqlCommand with parameters
                        using (SqlCommand sqlCommand = new SqlCommand(insertQuery, sqlConnection))
                        {
                            sqlCommand.Parameters.AddWithValue("@name", accountHolderName);
                            sqlCommand.Parameters.AddWithValue("@balance", currentBalance);
                            sqlCommand.Parameters.AddWithValue("@userId", userId); // Associate the account with the logged-in user

                            // Execute the query
                            sqlCommand.ExecuteNonQuery();
                            Console.WriteLine("Account added successfully!");
                            Console.WriteLine("---------------------------");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("An error occurred: " + e.Message);
                        Console.WriteLine("---------------------------");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid input for Current Balance.");
                Console.WriteLine("---------------------------");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }


        public void ViewAccountsForUser(int userId)
        {
            string connectionString = "Data Source=(local);Initial Catalog=BankSystem; Integrated Security=true";

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    // Define the SQL SELECT query to fetch accounts for the user
                    string selectQuery = "SELECT AccountNumber, AccountHolderName, Balance FROM dbo.Accounts WHERE UserID = @userId";

                    // Create and configure SqlCommand with parameters
                    using (SqlCommand sqlCommand = new SqlCommand(selectQuery, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@userId", userId);

                        // Execute the query and read the results
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                Console.WriteLine("Accounts for the current user:");
                                while (reader.Read())
                                {
                                    int accountNumber = reader.GetInt32(0);
                                    string accountHolderName = reader.GetString(1);
                                    decimal currentBalance = reader.GetDecimal(2);

                                    Console.WriteLine($"Account Number: {accountNumber}");
                                    Console.WriteLine($"Account Holder: {accountHolderName}");
                                    Console.WriteLine($"Current Balance: {currentBalance} OMR");
                                    Console.WriteLine("---------------------------");
                                   
                                }
                                Console.WriteLine("---------------------------");
                                Console.WriteLine("Press any key to continue...");
                                Console.ReadKey();
                            }
                            else
                            {
                                Console.WriteLine("No accounts found for this user.");
                                Console.WriteLine("---------------------------");
                                Console.WriteLine("Press any key to continue...");
                                Console.ReadKey();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    Console.WriteLine("---------------------------");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }
        public void Withdraw(int userId)
        {
            // Display the user's accounts and ask for the account number
            ViewAccountsForUser(userId);
            Console.WriteLine("Enter the Account Number from which you want to withdraw: ");
            if (int.TryParse(Console.ReadLine(), out int accountNumber))
            {
                Console.WriteLine("Enter the amount to withdraw: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal withdrawalAmount))
                {
                    string connectionString = "Data Source=(local);Initial Catalog=BankSystem; Integrated Security=true";

                    using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            sqlConnection.Open();

                            // Check if the specified account belongs to the current user
                            string checkAccountQuery = "SELECT Balance FROM dbo.Accounts WHERE AccountNumber = @accountNumber AND UserId = @userId";

                            using (SqlCommand checkAccountCommand = new SqlCommand(checkAccountQuery, sqlConnection))
                            {
                                checkAccountCommand.Parameters.AddWithValue("@accountNumber", accountNumber);
                                checkAccountCommand.Parameters.AddWithValue("@userId", userId);

                                object balanceResult = checkAccountCommand.ExecuteScalar();

                                if (balanceResult != null)
                                {
                                    decimal currentBalance = (decimal)balanceResult;

                                    if (currentBalance >= withdrawalAmount)
                                    {
                                        // Update the balance with the new amount after withdrawal
                                        string updateBalanceQuery = "UPDATE dbo.Accounts SET Balance = @newBalance WHERE AccountNumber = @accountNumber";

                                        using (SqlCommand updateBalanceCommand = new SqlCommand(updateBalanceQuery, sqlConnection))
                                        {
                                            decimal newBalance = currentBalance - withdrawalAmount;
                                            updateBalanceCommand.Parameters.AddWithValue("@newBalance", newBalance);
                                            updateBalanceCommand.Parameters.AddWithValue("@accountNumber", accountNumber);

                                            int rowsAffected = updateBalanceCommand.ExecuteNonQuery();

                                            if (rowsAffected > 0)
                                            {
                                                Console.WriteLine("Withdrawal successful!");
                                            }
                                            else
                                            {
                                                Console.WriteLine("Withdrawal failed. Please try again.");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Insufficient funds in the selected account.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("The specified account does not belong to you.");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("An error occurred: " + e.Message);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input for withdrawal amount.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input for account number.");
            }

            Console.WriteLine("---------------------------");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public void Deposit(int userId)
        {
            // Display the user's accounts and ask for the account number
            ViewAccountsForUser(userId);
            Console.WriteLine("Enter the Account Number to which you want to deposit: ");

            if (int.TryParse(Console.ReadLine(), out int accountNumber))
            {
                Console.WriteLine("Enter the amount to deposit: ");

                if (decimal.TryParse(Console.ReadLine(), out decimal depositAmount))
                {
                    string connectionString = "Data Source=(local);Initial Catalog=BankSystem; Integrated Security=true";

                    using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            sqlConnection.Open();

                            // Check if the specified account belongs to the current user
                            string checkAccountQuery = "SELECT Balance FROM dbo.Accounts WHERE AccountNumber = @accountNumber AND UserId = @userId";

                            using (SqlCommand checkAccountCommand = new SqlCommand(checkAccountQuery, sqlConnection))
                            {
                                checkAccountCommand.Parameters.AddWithValue("@accountNumber", accountNumber);
                                checkAccountCommand.Parameters.AddWithValue("@userId", userId);

                                object balanceResult = checkAccountCommand.ExecuteScalar();

                                if (balanceResult != null)
                                {
                                    decimal currentBalance = (decimal)balanceResult;

                                    // Update the balance with the new amount after deposit
                                    string updateBalanceQuery = "UPDATE dbo.Accounts SET Balance = @newBalance WHERE AccountNumber = @accountNumber";

                                    using (SqlCommand updateBalanceCommand = new SqlCommand(updateBalanceQuery, sqlConnection))
                                    {
                                        decimal newBalance = currentBalance + depositAmount;
                                        updateBalanceCommand.Parameters.AddWithValue("@newBalance", newBalance);
                                        updateBalanceCommand.Parameters.AddWithValue("@accountNumber", accountNumber);

                                        int rowsAffected = updateBalanceCommand.ExecuteNonQuery();

                                        if (rowsAffected > 0)
                                        {
                                            Console.WriteLine("Deposit successful!");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Deposit failed. Please try again.");
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("The specified account does not belong to you.");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("An error occurred: " + e.Message);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input for deposit amount.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input for account number.");
            }

            Console.WriteLine("---------------------------");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }


    }
}
