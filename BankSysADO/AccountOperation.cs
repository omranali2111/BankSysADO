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
        public void AddAccount()
        {
            Console.WriteLine("Enter Account Holder's Name: ");
            string accountHolderName = Console.ReadLine();

            Console.WriteLine("Enter Current Balance: ");
            if (double.TryParse(Console.ReadLine(), out double currentBalance))
            {
                string connectionString = "Data Source=(local);Initial Catalog=BankSystem; Integrated Security=true";

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    try
                    {
                        sqlConnection.Open();

                        // Define the SQL INSERT query with parameter placeholders
                        string insertQuery = "INSERT INTO dbo.Accounts (AccountHolderName, CurrentBalance) VALUES (@name, @balance)";

                        // Create and configure SqlCommand with parameters
                        using (SqlCommand sqlCommand = new SqlCommand(insertQuery, sqlConnection))
                        {
                            sqlCommand.Parameters.AddWithValue("@name", accountHolderName);
                            sqlCommand.Parameters.AddWithValue("@balance", currentBalance);

                            // Execute the query
                            sqlCommand.ExecuteNonQuery();
                            Console.WriteLine("Account added successfully!");
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
                Console.WriteLine("Invalid input for Current Balance.");
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
                    string selectQuery = "SELECT AccountNumber, AccountHolderName, CurrentBalance FROM dbo.Accounts WHERE User_ID = @userId";

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
                                    Console.WriteLine($"Current Balance: {currentBalance:C}");
                                    Console.WriteLine("---------------------------");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No accounts found for this user.");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                }
            }
        }

    }
}
