using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using static Regx.Builder.RegexElement;

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
                            Console.WriteLine("---------------------------");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();

                        }
                        else
                        {
                            Console.WriteLine("Password is invalid.");
                            Console.WriteLine("---------------------------");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
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

                string selectQuery = "SELECT * FROM Users WHERE Email = @Email AND HashedPassword = @Password";

                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        //single - user login
                        currentUser = null;
                        // User exists, set currentUser to the user's information
                        currentUser = new Users
                        {
                            UserId = (int)reader["UserId"],
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            
                        };

                        Console.WriteLine("Login successful!");
                        Console.WriteLine("---------------------------");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        return true; // User exists, login successful
                    }
                    else
                    {
                        Console.WriteLine("Login failed. Check your email and password.");
                        Console.WriteLine("---------------------------");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
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
  
    

    

