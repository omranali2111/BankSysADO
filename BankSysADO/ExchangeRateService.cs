using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSysADO
{
    internal class ExchangeRateService
    {
        private readonly HttpClient _httpClient;

        public ExchangeRateService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GetExchangeRateJsonAsync()
        {
            try
            {
                string apiUrl = "https://v6.exchangerate-api.com/v6/5b36f78c7abd8c6a32db5bf4/latest/OMR"; // Define the API URL here

                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine($"HTTP Error: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HTTP Request Error: {ex.Message}");
                return null;
            }
        }
        public async Task ViewExchangeRateData()
        {
            string jsonData = await GetExchangeRateJsonAsync();

            if (jsonData != null)
            {
                try
                {
                    ExchangeRateData exchangeData = JsonConvert.DeserializeObject<ExchangeRateData>(jsonData);
                    if (exchangeData.Result == "success")
                    {
                        string baseCurrency = exchangeData.BaseCode;
                        var conversionRates = exchangeData.ConversionRates;
                        var currencies = exchangeData.Currencies?.ToDictionary(info => info.Code, info => info.Name) ?? new Dictionary<string, string>();

                        Console.WriteLine($"Base Currency: {baseCurrency}");
                        Console.WriteLine("Exchange Rates:");
                        foreach (var rate in conversionRates)
                        {
                            string currencyCode = rate.Key;
                            double exchangeRate = rate.Value;
                            string currencyName = CurrencyInfo.CurrencyCodeToName.ContainsKey(currencyCode) ? CurrencyInfo.CurrencyCodeToName[currencyCode] : "Unknown";

                            Console.WriteLine($"{currencyName} ({currencyCode}): {exchangeRate}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to retrieve exchange rate data.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Failed to fetch JSON data from the API.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public async Task ConvertCurrency()
        {
            string jsonData = await GetExchangeRateJsonAsync();

            if (jsonData != null)
            {
                try
                {
                    ExchangeRateData exchangeData = JsonConvert.DeserializeObject<ExchangeRateData>(jsonData);
                    if (exchangeData.Result == "success")
                    {
                        string baseCurrency = exchangeData.BaseCode;
                        var conversionRates = exchangeData.ConversionRates;

                        Console.WriteLine("Available Currencies:");
                        foreach (var rate in conversionRates)
                        {
                            string currencyCode = rate.Key;
                            double exchangeRate = rate.Value;
                            string currencyName = CurrencyInfo.CurrencyCodeToName.ContainsKey(currencyCode) ? CurrencyInfo.CurrencyCodeToName[currencyCode] : "Unknown";

                            Console.WriteLine($"{currencyName} ({currencyCode})");
                        }

                        Console.Write("Enter the base currency code: ");
                        string baseCurrencyCode = Console.ReadLine().ToUpper();

                        if (!conversionRates.ContainsKey(baseCurrencyCode))
                        {
                            Console.WriteLine("Invalid base currency code.");
                            return;
                        }

                        Console.Write("Enter the target currency code: ");
                        string targetCurrencyCode = Console.ReadLine().ToUpper();

                        if (!conversionRates.ContainsKey(targetCurrencyCode))
                        {
                            Console.WriteLine("Invalid target currency code.");
                            return;
                        }

                        Console.Write("Enter the amount to convert: ");
                        if (double.TryParse(Console.ReadLine(), out double amountToConvert))
                        {
                            if (baseCurrencyCode == targetCurrencyCode)
                            {
                                Console.WriteLine("Base currency and target currency are the same. No conversion needed.");
                            }
                            else
                            {
                                double exchangeRate = conversionRates[targetCurrencyCode] / conversionRates[baseCurrencyCode];
                                double convertedAmount = amountToConvert * exchangeRate;

                                Console.WriteLine($"{amountToConvert} {baseCurrencyCode} is equal to {convertedAmount} {targetCurrencyCode}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid amount.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to retrieve exchange rate data.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Failed to fetch JSON data from the API.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }



    }
}

