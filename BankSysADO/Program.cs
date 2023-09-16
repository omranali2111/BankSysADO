using BankSysADO;

internal class Program
{
    public static async Task Main(string[] args)
    {
        UserRegistration userRegistration = new UserRegistration();
       ExchangeRateService exchangeRateService = new ExchangeRateService(); // Create an instance of ExchangeRateService

        Menu menu = new Menu(userRegistration, exchangeRateService); // Pass the ExchangeRateService instance as an argument
         await menu.Start();

       
    }
}