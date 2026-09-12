using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class ConfirmBuyItemOverlayScreen : OverlayScreen
    {
        public ShopScene ShopScene;
        public ShopListing ShopListing;
        public Image ShopListingIcon;
        public TextMeshProUGUI ShopListingLabel;
        public RectTransform ShopListingPriceRectTransform;
        public CurrencyAmountUI CurrencyAmountPrefab;

        private List<CurrencyAmountUI> _currencyAmounts = new List<CurrencyAmountUI>();

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            ShopListingIcon.sprite = ShopListing.Item.Icon;
            ShopListingIcon.SetMaterialForItem(ShopListing.Item.ColorShift);
            ShopListingLabel.text = ShopListing.Item.Name;
            ClearCurrencyAmounts();
            AddCurrencyAmounts(ShopListing.Price);
        }

        public void Confirm()
        {
            ShopScene.BuyItem(ShopListing);
            Dismiss();
        }

        private void ClearCurrencyAmounts()
        {
            foreach(CurrencyAmountUI currencyAmount in _currencyAmounts)
            {
                Destroy(currencyAmount.gameObject);
            }
            _currencyAmounts.Clear();
        }

        private void AddCurrencyAmounts(CurrencyAmountDictionary currencyAmounts)
        {
            foreach (KeyValuePair<Currency, int> currencyAmount in ShopListing.Price)
            {
                CurrencyAmountUI currencyAmountUI = Instantiate(CurrencyAmountPrefab, ShopListingPriceRectTransform);
                currencyAmountUI.CurrencyType = currencyAmount.Key;
                currencyAmountUI.Amount = currencyAmount.Value;
                _currencyAmounts.Add(currencyAmountUI);
            }
        }
    }
}