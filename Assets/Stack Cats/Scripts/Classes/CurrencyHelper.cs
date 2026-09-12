using UnityEngine;

namespace Tofuwu.StackCats
{
    public static class CurrencyHelper
    {
        public static string ToCurrencyString(this Currency currency, int amount)
        {
            return string.Format("{0:n0}", amount);
        }
    }
}
