using System.Collections.Generic;

namespace Tofuwu.StackCats
{
    public delegate void ItemBought(ShopListing shopListing);

    public class ShopScene : SceneBehaviour
    {
        public event ItemBought onItemBought;

        public ShopCollection ShopCollection;

        private CurrencyManager _currencyManager;
        private StuffManager _stuffManager;

        public void GoHome()
        {
            _gameManager.GoHome();
        }

        public bool BuyItem(ShopListing shopListing)
        {
            if (!_currencyManager.HasAmount(shopListing.Price)) return false;

            CurrencyAmountDictionary inverseListPrice = new CurrencyAmountDictionary();
            foreach(KeyValuePair<Currency, int> currency in shopListing.Price)
            {
                inverseListPrice.Add(currency.Key, -currency.Value);
            }
            _currencyManager.ChangeCurrency(inverseListPrice);

            _stuffManager.AddItem(shopListing.Item);

            if (onItemBought != null) onItemBought(shopListing);

            return true;
        }

        protected override void Awake()
        {
            base.Awake();

            _currencyManager = _gameManager.Currency;
            _stuffManager = _gameManager.Stuff;
        }
    }
}