using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace BankSysADO
{
    internal class  UserRegistration
    {
        private Users currentUser;

        public void RegisterUser()
        {
            Console.WriteLine("Enter your name: ");
            string name = Console.ReadLine();

            Console.WriteLine("Enter your email: ");
            string email = Console.ReadLine();


            Console.WriteLine("Enter your password: ");
            string password = Console.ReadLine();


            string connectionString = "Data Source=(local);Initial Catalog=BankSystem; Integrated Security=true";
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    // Define the SQL INSERT query with parameter placeholders
                    string insertQuery = "INSERT INTO dbo.Users (Name, Email, HashedPassword) VALUES (@name, @email, @hashed)";

                    // Create and configure SqlCommand with parameters
                    using (SqlCommand sqlCommand = new SqlCommand(insertQuery, sqlConnection))
                    {

                        if (IsPasswordValid(password))
                        {
                            sqlCommand.Parameters.AddWithValue("@name", name);
                            sqlCommand.Parameters.AddWithValue("@email", email);
                            sqlCommand.Parameters.AddWithValue("@hashed", password);
                            Console.WriteLine("Registration successful!");

                        }
                        else
                        {
                            Console.WriteLine("Password is invalid.");
                        }


                        // Execute the query
                        sqlCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                }
            }
            static bool IsPasswordValid(string password)
            {
                // Define the regular expression pattern
                string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";

                // Create a regex object
                Regex regex = new Regex(pattern);

                // Use regex.IsMatch to check if the password matches the pattern
                return regex.IsMatch(password);
            }
        }

        public async Task<bool> UserLogin()
        {
            Console.WriteLine("Enter your email: ");
            string email = Console.ReadLine();

            Console.WriteLine("Enter your password: ");
            string password = Console.ReadLine();

            string connectionString = "Data Source=(local);Initial Catalog=BankSystem; Integrated Security=true";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND HashedPassword = @Password";

                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    int userCount = (int)command.ExecuteScalar();

                    if (userCount > 0)
                    {
                        Console.WriteLine("Login successful!");
                        return true; // User exists, login successful
                    }
                    else
                    {
                        Console.WriteLine("Login failed. Check your email and password.");
                        return false; // Email or password is incorrect
                    }
                }
            }
        }

      


        public Users GetCurrentUser()
        {
            return currentUser;
        }

    }
}
  
    

    

