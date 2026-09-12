using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats.UI
{
    public delegate void PageChanged<T>(List<T> pageItems);

    public class PageableList<T> where T : class
    {
        public event PageChanged<T> onPageChanged;

        public List<T> CurrentPageItems { get { return _currentPageItems; } }

        public bool IsFirstPage { get { return _currentPageIndex == 0; } }

        public bool IsLastPage { get { return _currentPageIndex + 1 == _pageCount; } }

        protected List<T> _list;
        protected int _pageCount;
        protected int _itemsPerPage;
        protected int _currentPageIndex;
        protected List<T> _currentPageItems;

        public PageableList(List<T> list, int itemsPerPage, int initialPage = 0)
        {
            _list = new List<T>(list);
            _itemsPerPage = itemsPerPage;
            _pageCount = Mathf.CeilToInt(_list.Count / (float)_itemsPerPage);
            initialPage = initialPage < 0 || initialPage > _pageCount ? 0 : initialPage;
            GoToPage(initialPage);
        }

        public void GoToPage(int pageNumber)
        {
            if (pageNumber < 0 || pageNumber > _pageCount) return;

            _currentPageIndex = pageNumber;
            int count = _itemsPerPage < _list.Count ? _itemsPerPage : _list.Count;
            _currentPageItems = _list.GetRange(_currentPageIndex * _itemsPerPage, count);
            if (onPageChanged != null) onPageChanged(_currentPageItems);
        }

        public void GoToPreviousPage()
        {
            GoToPage(--_currentPageIndex);
        }

        public void GoToNextPage()
        {
            GoToPage(++_currentPageIndex);
        }
    }
}
