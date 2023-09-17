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
                                                RecordTransaction("Withdrawal", withdrawalAmount, accountNumber, null);
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

            if (int.TryParse(Console.ReadLine(), out int AccountNumber))
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
                                checkAccountCommand.Parameters.AddWithValue("@accountNumber", AccountNumber);
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
                                        updateBalanceCommand.Parameters.AddWithValue("@accountNumber", AccountNumber);

                                        int rowsAffected = updateBalanceCommand.ExecuteNonQuery();

                                        if (rowsAffected > 0)
                                        {
                                            Console.WriteLine("Deposit successful!");
                                            RecordTransaction("Deposit", depositAmount, null, AccountNumber);
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

        public void Transfer(int userId)
        {
            // Display the user's accounts and ask for the source account number
            Console.WriteLine("Transfer Money");
            Console.WriteLine("Your Accounts:");
            ViewAccountsForUser(userId);

            Console.WriteLine("Enter the Account Number to transfer money from: ");

            if (int.TryParse(Console.ReadLine(), out int sourceAccountNumber))
            {
                Console.WriteLine("Enter the target Account Number to transfer money to: ");

                if (int.TryParse(Console.ReadLine(), out int targetAccountNumber))
                {
                    Console.WriteLine("Enter the amount to transfer: ");

                    if (decimal.TryParse(Console.ReadLine(), out decimal transferAmount))
                    {
                        string connectionString = "Data Source=(local);Initial Catalog=BankSystem; Integrated Security=true";

                        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                        {
                            try
                            {
                                sqlConnection.Open();

                                // Check if the source account belongs to the current user
                                string checkSourceAccountQuery = "SELECT Balance FROM dbo.Accounts WHERE AccountNumber = @sourceAccountNumber AND UserId = @userId";

                                using (SqlCommand checkSourceAccountCommand = new SqlCommand(checkSourceAccountQuery, sqlConnection))
                                {
                                    checkSourceAccountCommand.Parameters.AddWithValue("@sourceAccountNumber", sourceAccountNumber);
                                    checkSourceAccountCommand.Parameters.AddWithValue("@userId", userId);

                                    object sourceBalanceResult = checkSourceAccountCommand.ExecuteScalar();

                                    if (sourceBalanceResult != null)
                                    {
                                        decimal sourceCurrentBalance = (decimal)sourceBalanceResult;

                                        if (sourceCurrentBalance >= transferAmount)
                                        {
                                            // Update the source account balance with the new amount after transfer
                                            string updateSourceBalanceQuery = "UPDATE dbo.Accounts SET Balance = @newSourceBalance WHERE AccountNumber = @sourceAccountNumber";

                                            using (SqlCommand updateSourceBalanceCommand = new SqlCommand(updateSourceBalanceQuery, sqlConnection))
                                            {
                                                decimal newSourceBalance = sourceCurrentBalance - transferAmount;
                                                updateSourceBalanceCommand.Parameters.AddWithValue("@newSourceBalance", newSourceBalance);
                                                updateSourceBalanceCommand.Parameters.AddWithValue("@sourceAccountNumber", sourceAccountNumber);

                                                int sourceRowsAffected = updateSourceBalanceCommand.ExecuteNonQuery();

                                                if (sourceRowsAffected > 0)
                                                {
                                                    // Check if the target account belongs to the current user or another user
                                                    string checkTargetAccountQuery = "SELECT UserId FROM dbo.Accounts WHERE AccountNumber = @targetAccountNumber";

                                                    using (SqlCommand checkTargetAccountCommand = new SqlCommand(checkTargetAccountQuery, sqlConnection))
                                                    {
                                                        checkTargetAccountCommand.Parameters.AddWithValue("@targetAccountNumber", targetAccountNumber);

                                                        object targetUserIdResult = checkTargetAccountCommand.ExecuteScalar();

                                                        if (targetUserIdResult != null)
                                                        {
                                                            int targetUserId = (int)targetUserIdResult;

                                                            // Check if the target account belongs to the current user or another user
                                                            if (targetUserId == userId)
                                                            {
                                                                // Transfer to own account
                                                                string updateTargetBalanceQuery = "UPDATE dbo.Accounts SET Balance = @newTargetBalance WHERE AccountNumber = @targetAccountNumber";

                                                                using (SqlCommand updateTargetBalanceCommand = new SqlCommand(updateTargetBalanceQuery, sqlConnection))
                                                                {
                                                                    // Get the current balance of the target account
                                                                    string getCurrentTargetBalanceQuery = "SELECT Balance FROM dbo.Accounts WHERE AccountNumber = @targetAccountNumber";

                                                                    using (SqlCommand getCurrentTargetBalanceCommand = new SqlCommand(getCurrentTargetBalanceQuery, sqlConnection))
                                                                    {
                                                                        getCurrentTargetBalanceCommand.Parameters.AddWithValue("@targetAccountNumber", targetAccountNumber);
                                                                        object targetBalanceResult = getCurrentTargetBalanceCommand.ExecuteScalar();

                                                                        if (targetBalanceResult != null)
                                                                        {
                                                                            decimal targetCurrentBalance = (decimal)targetBalanceResult;

                                                                            // Update the target account balance with the new amount after transfer
                                                                            decimal newTargetBalance = targetCurrentBalance + transferAmount;
                                                                            updateTargetBalanceCommand.Parameters.AddWithValue("@newTargetBalance", newTargetBalance);
                                                                            updateTargetBalanceCommand.Parameters.AddWithValue("@targetAccountNumber", targetAccountNumber);

                                                                            int targetRowsAffected = updateTargetBalanceCommand.ExecuteNonQuery();

                                                                            if (targetRowsAffected > 0)
                                                                            {
                                                                                Console.WriteLine("Transfer successful!");
                                                                                RecordTransaction("Transfer", transferAmount, sourceAccountNumber, targetAccountNumber);
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Transfer to target account failed. Please try again.");
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Target account not found.");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("You can only transfer to your own accounts.");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("Target account not found.");
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Transfer from source account failed. Please try again.");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Insufficient funds in the source account.");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("The specified source account does not belong to you.");
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
                        Console.WriteLine("Invalid input for transfer amount.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input for target account number.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input for source account number.");
            }

            Console.WriteLine("---------------------------");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        private void RecordTransaction(string transactionType, decimal amount, int? sourceAccountNumber, int? targetAccountNumber)
        {
            string connectionString = "Data Source=(local);Initial Catalog=BankSystem; Integrated Security=true";

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string insertTransactionQuery = "INSERT INTO dbo.Transactions (Timestamp, Type, Amount, SrcAccNO, TargetAccNO) " +
                        "VALUES (@timestamp, @type, @amount, @srcAccount, @targetAccount)";

                    using (SqlCommand insertTransactionCommand = new SqlCommand(insertTransactionQuery, sqlConnection))
                    {
                        insertTransactionCommand.Parameters.AddWithValue("@timestamp", DateTime.Now);
                        insertTransactionCommand.Parameters.AddWithValue("@type", transactionType);
                        insertTransactionCommand.Parameters.AddWithValue("@amount", amount);
                        insertTransactionCommand.Parameters.AddWithValue("@srcAccount", sourceAccountNumber ?? (object)DBNull.Value);
                        insertTransactionCommand.Parameters.AddWithValue("@targetAccount", targetAccountNumber ?? (object)DBNull.Value);

                        int rowsAffected = insertTransactionCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Transaction recorded successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to record the transaction.");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred while recording the transaction: " + e.Message);
                }
            }

        }
        public void ViewTransactionHistory(int userId, string period)
        {
            DateTime minSqlDate = new DateTime(1753, 1, 1);
            DateTime startDate;

            switch (period.ToLower())
            {
                case "last transaction":
                    startDate = minSqlDate; // Set to minimum date
                    break;
                case "last day":
                    startDate = DateTime.Now.AddDays(-1);
                    break;
                case "last 5 days":
                    startDate = DateTime.Now.AddDays(-5);
                    break;
                case "last 1 month":
                    startDate = DateTime.Now.AddMonths(-1);
                    break;
                case "last 2 months":
                    startDate = DateTime.Now.AddMonths(-2);
                    break;
                default:
                    Console.WriteLine("Invalid period. Showing all transactions.");
                    startDate = minSqlDate; // Set to minimum date
                    break;
            
            }

            string connectionString = "Data Source=(local);Initial Catalog=BankSystem; Integrated Security=true";

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    // Define the SQL SELECT query to fetch transaction history for the user's accounts within the specified period
                    string selectQuery = "SELECT TransId, Timestamp, Type, Amount, SrcAccNO, TargetAccNO FROM dbo.Transactions " +
                        "WHERE (SrcAccNO IN (SELECT AccountNumber FROM dbo.Accounts WHERE UserId = @userId) " +
                        "OR TargetAccNO IN (SELECT AccountNumber FROM dbo.Accounts WHERE UserId = @userId)) " +
                        "AND Timestamp >= @startDate " +
                        "ORDER BY Timestamp DESC";

                    // Create and configure SqlCommand with parameters
                    using (SqlCommand sqlCommand = new SqlCommand(selectQuery, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@userId", userId);
                        sqlCommand.Parameters.AddWithValue("@startDate", startDate);

                        // Execute the query and read the results
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                Console.WriteLine($"Transaction History (Last {period}):");
                                while (reader.Read())
                                {
                                    int transId = reader.GetInt32(0);
                                    DateTime timestamp = reader.GetDateTime(1);
                                    string type = reader.GetString(2);
                                    decimal amount = reader.GetDecimal(3);
                                    int? srcAccountNumber = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4);
                                    int? targetAccountNumber = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5);

                                    Console.WriteLine($"Transaction ID: {transId}");
                                    Console.WriteLine($"Timestamp: {timestamp}");
                                    Console.WriteLine($"Type: {type}");
                                    Console.WriteLine($"Amount: {amount} OMR");

                                    if (srcAccountNumber.HasValue)
                                    {
                                        Console.WriteLine($"Source Account: {srcAccountNumber}");
                                    }

                                    if (targetAccountNumber.HasValue)
                                    {
                                        Console.WriteLine($"Target Account: {targetAccountNumber}");
                                    }

                                    Console.WriteLine("---------------------------");
                                }
                                Console.WriteLine("Press any key to continue...");
                                Console.ReadKey();
                            }
                            else
                            {
                                Console.WriteLine("No transaction history found.");
                                Console.WriteLine("Press any key to continue...");
                                Console.ReadKey();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        public void DeleteUser(int userId)
        {
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

                    // Check if the provided email and password match the user's credentials
                    string checkCredentialsQuery = "SELECT UserId FROM dbo.Users WHERE UserId = @userId AND Email = @email AND HashedPassword = @password";

                    using (SqlCommand checkCredentialsCommand = new SqlCommand(checkCredentialsQuery, sqlConnection))
                    {
                        checkCredentialsCommand.Parameters.AddWithValue("@userId", userId);
                        checkCredentialsCommand.Parameters.AddWithValue("@email", email);
                        checkCredentialsCommand.Parameters.AddWithValue("@password", password);

                        object userIdResult = checkCredentialsCommand.ExecuteScalar();

                        if (userIdResult != null)
                        {
                            // User provided correct email and password, delete the user, accounts, and transactions
                            string deleteTransactionsQuery = "DELETE FROM dbo.Transactions WHERE SrcAccNO IN (SELECT AccountNumber FROM dbo.Accounts WHERE UserId = @userId) OR TargetAccNO IN (SELECT AccountNumber FROM dbo.Accounts WHERE UserId = @userId)";
                            string deleteAccountsQuery = "DELETE FROM dbo.Accounts WHERE UserId = @userId";
                            string deleteUserQuery = "DELETE FROM dbo.Users WHERE UserId = @userId";

                            using (SqlCommand deleteTransactionsCommand = new SqlCommand(deleteTransactionsQuery, sqlConnection))
                            using (SqlCommand deleteAccountsCommand = new SqlCommand(deleteAccountsQuery, sqlConnection))
                            using (SqlCommand deleteUserCommand = new SqlCommand(deleteUserQuery, sqlConnection))
                            {
                                deleteTransactionsCommand.Parameters.AddWithValue("@userId", userId);
                                deleteAccountsCommand.Parameters.AddWithValue("@userId", userId);
                                deleteUserCommand.Parameters.AddWithValue("@userId", userId);

                                // Begin a SQL transaction to ensure data consistency
                                using (SqlTransaction transaction = sqlConnection.BeginTransaction())
                                {
                                    deleteTransactionsCommand.Transaction = transaction;
                                    deleteAccountsCommand.Transaction = transaction;
                                    deleteUserCommand.Transaction = transaction;

                                    try
                                    {
                                        // Delete transactions
                                        deleteTransactionsCommand.ExecuteNonQuery();

                                        // Delete accounts
                                        deleteAccountsCommand.ExecuteNonQuery();

                                        // Delete the user
                                        deleteUserCommand.ExecuteNonQuery();

                                        // Commit the transaction
                                        transaction.Commit();

                                        Console.WriteLine("User deleted successfully.");
                                        Console.WriteLine("---------------------------");
                                        Console.WriteLine("Press any key to continue...");
                                        Console.ReadKey();
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("An error occurred while deleting the user, accounts, and transactions: " + e.Message);
                                        transaction.Rollback();
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid email or password. Deletion failed.");
                            Console.WriteLine("---------------------------");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                }
            }
        }

        public void DeleteUserAccount(int userId)
        {
            // Display the user's accounts
            ViewAccountsForUser(userId);

            Console.WriteLine("Enter the Account Number you want to delete: ");

            if (int.TryParse(Console.ReadLine(), out int accountNumberToDelete))
            {
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

                        // Check if the specified account belongs to the current user
                        string checkAccountQuery = "SELECT UserId FROM dbo.Accounts WHERE AccountNumber = @accountNumber AND UserId = @userId";

                        using (SqlCommand checkAccountCommand = new SqlCommand(checkAccountQuery, sqlConnection))
                        {
                            checkAccountCommand.Parameters.AddWithValue("@accountNumber", accountNumberToDelete);
                            checkAccountCommand.Parameters.AddWithValue("@userId", userId);

                            object userIdResult = checkAccountCommand.ExecuteScalar();

                            if (userIdResult != null)
                            {
                                // The specified account belongs to the current user
                                // Check if the provided email and password match the user's credentials
                                string checkCredentialsQuery = "SELECT UserId FROM dbo.Users WHERE UserId = @userId AND Email = @email AND HashedPassword = @password";

                                using (SqlCommand checkCredentialsCommand = new SqlCommand(checkCredentialsQuery, sqlConnection))
                                {
                                    checkCredentialsCommand.Parameters.AddWithValue("@userId", userId);
                                    checkCredentialsCommand.Parameters.AddWithValue("@email", email);
                                    checkCredentialsCommand.Parameters.AddWithValue("@password", password);

                                    userIdResult = checkCredentialsCommand.ExecuteScalar();

                                    if (userIdResult != null)
                                    {
                                        // User provided correct email and password, delete the account
                                        string deleteAccountQuery = "DELETE FROM dbo.Accounts WHERE AccountNumber = @accountNumberToDelete";

                                        using (SqlCommand deleteAccountCommand = new SqlCommand(deleteAccountQuery, sqlConnection))
                                        {
                                            deleteAccountCommand.Parameters.AddWithValue("@accountNumberToDelete", accountNumberToDelete);

                                            int rowsAffected = deleteAccountCommand.ExecuteNonQuery();

                                            if (rowsAffected > 0)
                                            {
                                                Console.WriteLine("Account deleted successfully.");
                                                Console.WriteLine("---------------------------");
                                                Console.WriteLine("Press any key to continue...");
                                                Console.ReadKey();
                                            }
                                            else
                                            {
                                                Console.WriteLine("Failed to delete the account. Please try again.");
                                                Console.WriteLine("---------------------------");
                                                Console.WriteLine("Press any key to continue...");
                                                Console.ReadKey();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid email or password. Deletion failed.");
                                        Console.WriteLine("---------------------------");
                                        Console.WriteLine("Press any key to continue...");
                                        Console.ReadKey();
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("The specified account does not belong to you.");
                                Console.WriteLine("---------------------------");
                                Console.WriteLine("Press any key to continue...");
                                Console.ReadKey();
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
                Console.WriteLine("Invalid input for account number.");
                Console.WriteLine("---------------------------");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }




    }
}


