using System;
using System.Collections.Generic;
using System.Linq;

namespace Tofuwu.StackCats.Data
{
    [Serializable]
    public class LocalStuffData : LocalData<LocalStuffData>, IStuffData
    {
        private readonly Dictionary<string, IPresentData> _unopenedPresents;
        private readonly Dictionary<string, int> _items;

        public LocalStuffData(string dataPath) : base(dataPath)
        {
            _unopenedPresents = new Dictionary<string, IPresentData>();
            _items = new Dictionary<string, int>();
            Save();
        }

        public string AddPresent(string fromCatID, CurrencyAmountDictionary currency, Dictionary<string, int> items)
        {
            string presentID = Guid.NewGuid().ToString();
            IPresentData presentData = new LocalPresentData(fromCatID, currency, items);

            _unopenedPresents.Add(presentID, presentData);
            Save();

            return presentID;
        }

        public void RemovePresent(string presentID)
        {
            if (_unopenedPresents.Remove(presentID)) Save();
        }

        public Dictionary<string, IPresentData> GetPresents()
        {
            return _unopenedPresents;
        }

        public void AddItem(string itemID, int amount)
        {
            if (_items.ContainsKey(itemID))
            {
                _items[itemID] += amount;
            }
            else
            {
                _items.Add(itemID, amount);
            }

            Save();
        }

        public void RemoveItem(string itemID, int amount)
        {
            if (_items.ContainsKey(itemID))
            {
                _items[itemID] -= amount;
                if (_items[itemID] <= 0) _items.Remove(itemID);
            }

            Save();
        }

        public bool HasItem(string itemID, bool includePresents = false)
        {
            if (_items.ContainsKey(itemID))
            {
                return true;
            }

            return includePresents && _unopenedPresents.Values.Any(p => p.Items.ContainsKey(itemID));
        }

        public int GetItemCount(string itemID)
        {
            return _items.ContainsKey(itemID) ? _items[itemID] : 0;
        }
    }
}