using Endava_Project.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Endava_Project.Server
{
    public static class CurrencyManager
    {
        public static List<string> Currencies { get; }
        public static List<Currency> CurrencyDb;

        static CurrencyManager()
        {
            CurrencyDb = new List<Currency>
            {
                new Currency
                {
                    Name = "USD",
                    Value = 18
                },

                new Currency
                {
                    Name = "EUR",
                    Value = 20
                },

                new Currency
                {
                    Name = "MDL",
                    Value = 1
                }
            };

            Currencies = CurrencyDb.Select(e => e.Name).ToList();
        }

        public static decimal CheckCurrency(decimal amount,string source_currency,string destination_currency)
        {
           
            decimal destination_amount=0;
            if (source_currency == destination_currency)
                return amount;
            else
            {
                decimal source_value = CurrencyDb.FirstOrDefault(e => e.Name == source_currency).Value;
                decimal destination_value = CurrencyDb.FirstOrDefault(e => e.Name == destination_currency).Value;
                destination_amount = amount * source_value / destination_value;
            }
            return destination_amount;
        }
    }
}
