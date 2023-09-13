using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace BankSysADO
{
    internal class ExchangeRateData
    {
        public string Base { get; set; }
        public DateTime Date { get; set; }
        public ExchangeRates Rates { get; set; }

        public static ExchangeRateData FromJson(string json)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ExchangeRateData>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public class ExchangeRates
        {

            public decimal USD { get; set; }
            public decimal EUR { get; set; }
            public decimal GBP { get; set; }
            public decimal JPY { get; set; }
            public decimal CAD { get; set; }

        }
    }
}
