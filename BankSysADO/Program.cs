using BankSysADO;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var userRegistration = new UserRegistration();
        var exchangeRateService = new ExchangeRateService();
        var accountOperation = new AccountOperation(); 

        var menu = new Menu(userRegistration, exchangeRateService, accountOperation);

        menu.Start().Wait();


    }
}