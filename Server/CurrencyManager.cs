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

        public static decimal CheckCurrency(decimal sourceAmount,string sourceCurrency,string destinationCurrency)
        {
           
            decimal destinationAmount=0;
            if (sourceCurrency == destinationCurrency)
                return sourceAmount;
            else
            {
                decimal sourceValue = CurrencyDb.FirstOrDefault(e => e.Name == sourceCurrency).Value;
                decimal destinationValue = CurrencyDb.FirstOrDefault(e => e.Name == destinationCurrency).Value;
                destinationAmount = sourceAmount * sourceValue / destinationValue;
            }
            return destinationAmount;
        }
    }
}
