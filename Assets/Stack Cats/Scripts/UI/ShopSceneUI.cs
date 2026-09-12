using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class ShopSceneUI : MonoBehaviour
    {
        public class PuzzleAreaFilterOption
        {
            public string Label { get; set; }

            public PuzzleArea PuzzleArea { get; set; }
        }

        public class ItemTypeFilterOption
        {
            public string Label { get; set; }

            public Type ItemType { get; set; }
        }

        public ShopScene ShopScene;
        public Dropdown PuzzleAreaFilterDropdown;
        public Dropdown ItemTypeFilterDropdown;
        public RectTransform ShopItemsRectTransform;
        public ShopListingButtonUI ShopListingButtonPrefab;
        public OverlayScreenManager OverlayScreenManager;
        public ConfirmBuyItemOverlayScreen ConfirmBuyItemOverlayScreen;
        public ItemBoughtOverlayScreen ItemBoughtOverlayScreen;

        private StuffManager _stuffManager;
        private PuzzleManager _puzzleManager;
        private List<ShopListing> _allShopListings;
        private List<ShopListingButtonUI> _shopListingButtons = new List<ShopListingButtonUI>();
        private PuzzleArea _puzzleAreaFilter;
        private List<PuzzleAreaFilterOption> _puzzleAreaFilterOptions;
        private Type _itemTypeFilter;
        private List<ItemTypeFilterOption> _itemTypeFilterOptions;

        protected void Awake()
        {
            _stuffManager = GameManager.Instance.Stuff;
            _puzzleManager = GameManager.Instance.Puzzles;
            _allShopListings = ShopScene.ShopCollection.ShopListings;

            _puzzleAreaFilterOptions = new List<PuzzleAreaFilterOption>();
            _puzzleAreaFilterOptions.Add(new PuzzleAreaFilterOption { PuzzleArea = null, Label = "All" });
            foreach (PuzzleArea puzzleArea in _puzzleManager.PuzzleAreaCollection.List)
            {
                if (!_puzzleManager.IsAreaLocked(puzzleArea))
                {
                    _puzzleAreaFilterOptions.Add(new PuzzleAreaFilterOption { PuzzleArea = puzzleArea, Label = puzzleArea.AreaTitle });
                }
            }
            PuzzleAreaFilterDropdown.ClearOptions();
            foreach (PuzzleAreaFilterOption puzzleAreaFilterOption in _puzzleAreaFilterOptions)
            {
                PuzzleAreaFilterDropdown.options.Add(new Dropdown.OptionData { text = puzzleAreaFilterOption.Label });
            }

            _itemTypeFilterOptions = new List<ItemTypeFilterOption> {
                new ItemTypeFilterOption{ ItemType = null, Label = "All" },
                new ItemTypeFilterOption{ ItemType = typeof(WallpaperItem), Label = "Wallpapers" },
                new ItemTypeFilterOption{ ItemType = typeof(FloorItem), Label = "Floors" },
                new ItemTypeFilterOption{ ItemType = typeof(WindowItem), Label = "Windows" },
                new ItemTypeFilterOption{ ItemType = typeof(DresserItem), Label = "Dressers" },
                new ItemTypeFilterOption{ ItemType = typeof(PlaceableObjectItem), Label = "Objects" },
                new ItemTypeFilterOption{ ItemType = typeof(Item), Label = "Other" }
            };
            ItemTypeFilterDropdown.ClearOptions();
            foreach (ItemTypeFilterOption itemTypeFilterOption in _itemTypeFilterOptions)
            {
                ItemTypeFilterDropdown.options.Add(new Dropdown.OptionData { text = itemTypeFilterOption.Label });
            }
        }

        protected void Start()
        {
            UpdateButtons();
        }

        protected void OnEnable()
        {
            ShopScene.onItemBought += OnItemBought;
            PuzzleAreaFilterDropdown.onValueChanged.AddListener(OnPuzzleAreaFilterChanged);
            ItemTypeFilterDropdown.onValueChanged.AddListener(OnItemTypeFilterChanged);
        }

        protected void OnDisable()
        {
            ShopScene.onItemBought -= OnItemBought;
            PuzzleAreaFilterDropdown.onValueChanged.RemoveListener(OnPuzzleAreaFilterChanged);
            ItemTypeFilterDropdown.onValueChanged.RemoveListener(OnItemTypeFilterChanged);
        }

        private void FilterByPuzzleArea(PuzzleArea puzzleArea)
        {
            _puzzleAreaFilter = puzzleArea;
            UpdateButtons();
        }

        private void FilterByType(Type itemType)
        {
            _itemTypeFilter = itemType;
            UpdateButtons();
        }

        private List<ShopListing> GetListingsForFilters()
        {
            IEnumerable<ShopListing> shopListings = _allShopListings
                .Where(sl => sl.RequiredPuzzleArea == null || !_puzzleManager.IsAreaLocked(sl.RequiredPuzzleArea))
                .Where(sl => !sl.Item.IsUnique || !_stuffManager.HasItem(sl.Item) /* || sl.Item.GetType() == typeof(ConsumableItem)*/);

            if (_itemTypeFilter != null)
            {
                shopListings = shopListings.Where(sl => sl.Item.GetType() == _itemTypeFilter);
            }

            if (_puzzleAreaFilter != null)
            {
                shopListings = shopListings.Where(sl => sl.RequiredPuzzleArea == _puzzleAreaFilter);
            }

            return shopListings.ToList();
        }

        private void ClearButtons()
        {
            foreach (ShopListingButtonUI shopListingButton in _shopListingButtons)
            {
                Destroy(shopListingButton.gameObject);
            }
            _shopListingButtons.Clear();
        }

        private void UpdateButtons()
        {
            ClearButtons();

            List<ShopListing> shopListings = GetListingsForFilters();
            foreach (ShopListing shopListing in shopListings)
            {
                ShopListingButtonUI shopListingButton = Instantiate(ShopListingButtonPrefab, ShopItemsRectTransform);
                shopListingButton.ShopListing = shopListing;
                shopListingButton.Button.onClick.AddListener(delegate { ConfirmBuyItem(shopListing); });
                _shopListingButtons.Add(shopListingButton);
            }
        }

        private void ConfirmBuyItem(ShopListing shopListing)
        {
            ConfirmBuyItemOverlayScreen.ShopListing = shopListing;
            OverlayScreenManager.EnqueueScreen(ConfirmBuyItemOverlayScreen);
        }

        private void OnItemBought(ShopListing shopListing)
        {
            UpdateButtons();
            ItemBoughtOverlayScreen.ShopListing = shopListing;
            OverlayScreenManager.EnqueueScreen(ItemBoughtOverlayScreen);
        }

        private void OnPuzzleAreaFilterChanged(int dropdownOption)
        {
            FilterByPuzzleArea(_puzzleAreaFilterOptions[dropdownOption].PuzzleArea);
        }

        private void OnItemTypeFilterChanged(int dropdownOption)
        {
            FilterByType(_itemTypeFilterOptions[dropdownOption].ItemType);
        }
    }
}