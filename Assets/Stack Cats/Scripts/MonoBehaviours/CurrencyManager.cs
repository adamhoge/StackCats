using System;
using System.Collections.Generic;
using UnityEngine;
using Tofuwu.StackCats.Data;
using RotaryHeart.Lib.SerializableDictionary;

namespace Tofuwu.StackCats
{
    public delegate void CurrencyChanged(Currency currency, int amount, int total);

    [Serializable]
    public class CurrencyAmountDictionary : SerializableDictionaryBase<Currency, int> { }

    [Serializable]
    public class CurrencyDetailsDictionary : SerializableDictionaryBase<Currency, CurrencyDetails> { }

    public class CurrencyManager : MonoBehaviour
    {
        public event CurrencyChanged onCurrencyChanged;

        public CurrencyDetailsDictionary CurrencyDetails;

        private ICurrencyData _data;

        public bool HasAmount(CurrencyAmountDictionary currencyAmount)
        {
            foreach(KeyValuePair<Currency, int> currency in currencyAmount)
            {
                if (GetCurrencyHeld(currency.Key) < currency.Value) return false;
            }

            return true;
        }

        public int GetCurrencyHeld(Currency currency)
        {
            return _data.GetCurrencyHeld(currency);
        }

        public bool ChangeCurrency(Currency currency, int amount, bool allowRemainder = false)
        {
            if (_data.ChangeCurrency(currency, amount, allowRemainder))
            {
                if (onCurrencyChanged != null) onCurrencyChanged(currency, amount, _data.GetCurrencyHeld(currency));
                return true;
            }

            return false;
        }

        public bool ChangeCurrency(CurrencyAmountDictionary currencyAmount, bool allowRemainder = false)
        {
            if (!allowRemainder && !HasAmount(currencyAmount)) return false;

            foreach (KeyValuePair<Currency, int> currency in currencyAmount)
            {
                ChangeCurrency(currency.Key, currency.Value, true);
            }

            return true;
        }

        protected void Awake()
        {
            _data = GameManager.Instance.Data.CurrencyData;
        }
    }
}