using System;
using System.Collections.Generic;

namespace Tofuwu.StackCats
{
    [Serializable]
    public class PresentInfo
    {
        public string PresentId;
        public Cat FromCat;
        public CurrencyAmountDictionary CurrencyContents;
        public ItemAmountDictionary ItemContents;
    }
}