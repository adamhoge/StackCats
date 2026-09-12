namespace Tofuwu.StackCats.Data
{
    public interface ICurrencyData
    {
        int GetCurrencyHeld(Currency currency);
        bool ChangeCurrency(Currency currency, int amount, bool allowRemainder = false);
    }
}