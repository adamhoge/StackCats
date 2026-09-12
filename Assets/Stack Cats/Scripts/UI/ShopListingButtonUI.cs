using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    [RequireComponent(typeof(Button))]
    public class ShopListingButtonUI : MonoBehaviour
    {
        public ShopListing ShopListing;
        public Image ItemIconImage;
        public RectTransform CostRectTransform;
        public CurrencyAmountUI CurrencyAmountPrefab;

        /// <summary>
        /// The button associated with the shop item.
        /// </summary>
        public Button Button { get { return _button; } }

        private CurrencyManager _currencyManager;
        private Button _button;

        protected void Awake()
        {
            _currencyManager = GameManager.Instance.Currency;
            _button = GetComponent<Button>();
        }

        protected void Start()
        {
            _button.interactable = _currencyManager.HasAmount(ShopListing.Price);

            ItemIconImage.sprite = ShopListing.Item.Icon;
            ItemIconImage.SetMaterialForItem(ShopListing.Item.ColorShift);
            
            foreach(KeyValuePair<Currency, int> currencyAmount in ShopListing.Price)
            {
                CurrencyAmountUI currencyAmountUI = Instantiate(CurrencyAmountPrefab, CostRectTransform);
                currencyAmountUI.CurrencyType = currencyAmount.Key;
                currencyAmountUI.Amount = currencyAmount.Value;
            }
        }
    }
}