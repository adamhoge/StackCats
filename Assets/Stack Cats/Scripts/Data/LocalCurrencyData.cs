using System;

namespace Tofuwu.StackCats.Data
{
    [Serializable]
    public class LocalCurrencyData : LocalData<LocalCurrencyData>, ICurrencyData
    {
        private readonly int[] _currencyHeld;

        public LocalCurrencyData(string dataPath) : base(dataPath)
        {
            _currencyHeld = new int[7];
            Save();
        }

        public int GetCurrencyHeld(Currency currency)
        {
            return _currencyHeld[(int)currency];
        }

        public bool ChangeCurrency(Currency currency, int amount, bool allowRemainder = false)
        {
            int currencyHeld = _currencyHeld[(int)currency];

            if (-amount > currencyHeld)
            {
                if (!allowRemainder) return false;

                amount = -currencyHeld;
            }

            _currencyHeld[(int)currency] += amount;
            Save();
            return true;
        }
    }
}