using System.Collections.Generic;

namespace Tofuwu.StackCats.Data
{
    public interface IStuffData
    {
        Dictionary<string, IPresentData> GetPresents();
        string AddPresent(string fromCatID, CurrencyAmountDictionary currency, Dictionary<string, int> items);
        void RemovePresent(string presentID);
        void AddItem(string itemID, int amount);
        void RemoveItem(string itemID, int amount);
        bool HasItem(string itemID, bool includePresents = false);
        int GetItemCount(string itemID);
    }
}