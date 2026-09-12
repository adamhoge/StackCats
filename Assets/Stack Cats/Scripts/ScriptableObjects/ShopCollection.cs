using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    [Serializable]
    public class ShopListing
    {
        public Item Item;
        public PuzzleArea RequiredPuzzleArea;
        public CurrencyAmountDictionary Price;
    }

    [CreateAssetMenu(fileName = "Shop Collection", menuName = "Stack Cats/Items/Shop Collection")]
    public class ShopCollection : ScriptableObject
    {
        public List<ShopListing> ShopListings = new List<ShopListing>();
    }
}