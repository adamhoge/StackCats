using System;
using System.Collections.Generic;

namespace Tofuwu.StackCats.Data
{
    [Serializable]
    public class LocalPresentData : IPresentData
    {
        public string FromCatID { get { return _fromCatID; } }

        public Dictionary<Currency, int> Currency { get { return _currency; } }

        public Dictionary<string, int> Items { get { return _items; } }

        public LocalPresentData(string fromCatID, CurrencyAmountDictionary currencies, Dictionary<string, int> items)
        {
            Dictionary<Currency, int> currencyDictionary = new Dictionary<Currency, int>();
            foreach (KeyValuePair<Currency, int> currency in currencies)
            {
                currencyDictionary.Add(currency.Key, currency.Value);
            }

            _fromCatID = fromCatID;
            _currency = currencyDictionary;
            _items = items;
        }

        private readonly string _fromCatID;
        private readonly Dictionary<Currency, int> _currency;
        private readonly Dictionary<string, int> _items;
    }
}