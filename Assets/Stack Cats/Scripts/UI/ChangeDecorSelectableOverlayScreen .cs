using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public abstract class ChangeDecorSelectableOverlayScreen<T> : OverlayScreen where T : Item
    {
        public HomeDecor HomeDecor;
        public ItemButton ItemButtonPrefab;
        public RectTransform ItemButtonsRectTransform;
        public Button PreviousPageButton;
        public Button NextPageButton;

        public abstract T CurrentSelection { get; }

        protected StuffManager _stuffManager;
        protected int _pageCount;
        protected int _itemsPerPage;
        protected int _currentPageIndex;

        private List<T> _items;
        private List<ItemButton> _pageItemButtons = new List<ItemButton>();
        private T _originalSelection;

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            _originalSelection = CurrentSelection;
        }

        public void GoToPreviousPage()
        {
            GoToPage(_currentPageIndex - 1);
        }

        public void GoToNextPage()
        {
            GoToPage(_currentPageIndex + 1);
        }

        public void GoToPage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex > _pageCount) return;

            foreach (ItemButton itemButton in _pageItemButtons)
            {
                Destroy(itemButton.gameObject);
            }
            _pageItemButtons.Clear();

            _currentPageIndex = pageIndex;
            int startIndex = pageIndex * _itemsPerPage;
            int endIndex = startIndex + _itemsPerPage;
            for (int i = startIndex; i < endIndex && i < _items.Count; i++)
            {
                ItemButton newItemButton = Instantiate(ItemButtonPrefab, ItemButtonsRectTransform);
                newItemButton.Item = _items[i];
                newItemButton.Button.onClick.AddListener(delegate { OnSelect((T)newItemButton.Item); });
                _pageItemButtons.Add(newItemButton);
            }

            UpdatePageNavigationButtons();
        }

        public abstract List<T> GetSelectionOptions();

        public void Save()
        {
            Dismiss();
        }

        public void Cancel()
        {
            OnSelect(_originalSelection);
            Dismiss();
        }

        protected void OnEnable()
        {
            PreviousPageButton.onClick.AddListener(GoToPreviousPage);
            NextPageButton.onClick.AddListener(GoToNextPage);
        }

        protected void OnDisable()
        {
            PreviousPageButton.onClick.RemoveListener(GoToPreviousPage);
            NextPageButton.onClick.RemoveListener(GoToNextPage);
        }

        protected virtual void Awake()
        {
            _stuffManager = GameManager.Instance.Stuff;
        }

        protected virtual void Start()
        {
            _items = GetSelectionOptions();
            _itemsPerPage = 6;
            _pageCount = Mathf.CeilToInt(_items.Count / (float)_itemsPerPage);

            // TODO: Go to page of currently selected item.
            GoToPage(0);
        }

        protected abstract void OnSelect(T selection);

        private void UpdatePageNavigationButtons()
        {
            PreviousPageButton.interactable = _currentPageIndex != 0;
            NextPageButton.interactable = _currentPageIndex + 1 < _pageCount;
        }
    }
}