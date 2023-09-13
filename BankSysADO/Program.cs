using BankSysADO;

internal class Program
{
    private static void Main(string[] args)
    {
        UserRegistration s = new UserRegistration();
        s.GetExchangeRatesAsync();
    }
}