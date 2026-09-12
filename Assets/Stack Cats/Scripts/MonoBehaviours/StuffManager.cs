using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Tofuwu.StackCats.Data;
using RotaryHeart.Lib.SerializableDictionary;

namespace Tofuwu.StackCats
{
    [Serializable]
    public class ItemAmountDictionary : SerializableDictionaryBase<Item, int> { }

    public delegate void PresentAdded(PresentInfo presentInfo);
    public delegate void PresentRemoved(PresentInfo presentInfo);
    public delegate void ItemAdded(Item item, int amount);

    public class StuffManager : MonoBehaviour
    {
        public event PresentAdded onPresentAdded;
        public event PresentRemoved onPresentRemoved;
        public event ItemAdded onItemAdded;

        public WallpaperItemCollection WallpaperItems;
        public FloorItemCollection FloorItems;
        public WindowItemCollection WindowItems;
        public DresserItemCollection DresserItems;
        public PlaceableObjectItemCollection PlaceableItems;
        public TrophyItemCollection TrophyItems;
        public ItemCollection MiscellaneousItems;
        public ItemCollection AdPresentItems;
        public Cat DefaultFromCat;

        public List<Item> AllItems { get { return _allItems; } }

        private List<Item> _allItems;
        private GameManager _gameManager;
        private CatManager _catManager;
        private IStuffData _data;

        public void AddPresent(Cat fromCat, CurrencyAmountDictionary currency, ItemAmountDictionary items)
        {
            Dictionary<string, int> itemsData = new Dictionary<string, int>();

            foreach(KeyValuePair<Item, int> itemAmount in items)
            {
                itemsData.Add(itemAmount.Key.GetId(), itemAmount.Value);
            }

            if (!fromCat) fromCat = DefaultFromCat;

            string presentId = _data.AddPresent(fromCat.GetId(), currency, itemsData);
            if (onPresentAdded != null) onPresentAdded(new PresentInfo { PresentId = presentId, FromCat = fromCat, CurrencyContents = currency, ItemContents = items });
        }

        public void RemovePresent(string presentId)
        {
            _data.RemovePresent(presentId);
            if (onPresentRemoved != null) onPresentRemoved(new PresentInfo { PresentId = presentId });
        }

        public List<PresentInfo> GetPresents()
        {
            List<PresentInfo> result = new List<PresentInfo>();

            foreach (KeyValuePair<string, IPresentData> presents in _data.GetPresents())
            {
                IPresentData presentData = presents.Value;

                CurrencyAmountDictionary currencyContents = new CurrencyAmountDictionary();
                foreach(KeyValuePair<Currency, int> currency in presentData.Currency)
                {
                    currencyContents.Add(currency.Key, currency.Value);
                }

                ItemAmountDictionary itemContents = new ItemAmountDictionary();
                foreach(KeyValuePair<string, int> itemData in presentData.Items)
                {
                    itemContents.Add(_allItems.Find(i => i.GetId() == itemData.Key), itemData.Value);
                }

                PresentInfo presentInfo = new PresentInfo
                {
                    PresentId = presents.Key,
                    FromCat = _catManager.CatCollection.GetById(presentData.FromCatID),
                    CurrencyContents = currencyContents,
                    ItemContents = itemContents
                };

                result.Add(presentInfo);
            }

            return result;
        }

        public void AddItem(Item item, int amount = 1)
        {
            _data.AddItem(item.GetId(), amount);
            if (onItemAdded != null) onItemAdded(item, amount);
        }

        public void RemoveItem(Item item, int amount = 1)
        {
            _data.RemoveItem(item.GetId(), amount);
        }

        public bool HasItem(Item item, bool includePresents = false)
        {
            return _data.HasItem(item.GetId(), includePresents);
        }

        public int GetItemCount(Item item)
        {
            return _data.GetItemCount(item.GetId());
        }

        public Item GetRandomAdPresentItem(ItemRarity? rarity = null, bool unowned = false)
        {
            IEnumerable<Item> itemPool = AdPresentItems.List;
            if(rarity != null)
            {
                itemPool = itemPool.Where(i => i.Rarity == rarity.Value);
            }
            if (unowned)
            {
                itemPool = itemPool.Where(i => !i.IsUnique || !HasItem(i, true));
            }

            List<Item> itemPoolList = itemPool.ToList();
            return itemPoolList.Count > 0 ? itemPoolList[UnityEngine.Random.Range(0, itemPoolList.Count)] : null;
        }

        protected void Awake()
        {
            _gameManager = GameManager.Instance;
            _catManager = _gameManager.Cats;

            _data = GameManager.Instance.Data.StuffData;

            _allItems = new List<Item>();
            _allItems.AddRange(WallpaperItems.List);
            _allItems.AddRange(FloorItems.List);
            _allItems.AddRange(WindowItems.List);
            _allItems.AddRange(DresserItems.List);
            _allItems.AddRange(PlaceableItems.List);
            _allItems.AddRange(TrophyItems.List);
            _allItems.AddRange(MiscellaneousItems.List);
        }
    }
}