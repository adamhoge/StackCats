using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class ItemBoughtOverlayScreen : OverlayScreen
    {
        public ShopListing ShopListing;
        public Image ShopListingIcon;
        public TextMeshProUGUI ShopListingLabel;
        public Button UseNowButton;
        public RectTransform ConfirmButtonRectTransform;

        private DataManager _dataManager;
        private Vector2 _useNowConfirmAnchorMin = new Vector2(0.6f, 0.0f);

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            ShopListingIcon.sprite = ShopListing.Item.Icon;
            ShopListingIcon.SetMaterialForItem(ShopListing.Item.ColorShift);
            ShopListingLabel.text = ShopListing.Item.Name;

            Type itemType = ShopListing.Item.GetType();
            bool isAutoUsable = itemType == typeof(WallpaperItem) || itemType == typeof(FloorItem) || itemType == typeof(WindowItem) || itemType == typeof(DresserItem);
            UseNowButton.gameObject.SetActive(isAutoUsable);
            ConfirmButtonRectTransform.anchorMin = isAutoUsable ? _useNowConfirmAnchorMin : Vector2.zero;
        }

        public void UseNow()
        {
            Item item = ShopListing.Item;
            Type itemType = item.GetType();
            if (itemType == typeof(WallpaperItem))
            {
                _dataManager.HomeData.CurrentWallpaperId = item.GetId();
            }
            else if (itemType == typeof(FloorItem))
            {
                _dataManager.HomeData.CurrentFloorId = item.GetId();
            }
            else if (itemType == typeof(WindowItem))
            {
                _dataManager.HomeData.CurrentWindowId = item.GetId();
            }
            else if (itemType == typeof(DresserItem))
            {
                _dataManager.HomeData.CurrentDresserId = item.GetId();
            }

            Dismiss();
        }

        protected void Awake()
        {
            _dataManager = GameManager.Instance.Data;
        }
    }
}