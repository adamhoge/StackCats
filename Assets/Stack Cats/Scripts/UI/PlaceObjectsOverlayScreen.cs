using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class PlaceObjectsOverlayScreen : OverlayScreen
    {
        private class OriginalObjectPlacementInfo
        {
            public DateTime PlacedDateTime;
            public DateTime ActivatedDateTime;
        }

        public Camera MainCamera;
        public HomeDecor HomeDecor;
        public RectTransform ItemButtonsRectTransform;
        public PlaceableObjectItemDragButton ItemDragButtonPrefab;
        public DresserLocationDragButton DresserLocationDragButtonPrefab;
        public Image DraggedItemImagePrefab;
        public Button PreviousPageButton;
        public Button NextPageButton;

        private PageableList<PlaceableObjectItem> _items;
        private StuffManager _stuffManager;
        private List<PlaceableObjectItemDragButton> _placeableObjectItemDragButtons = new List<PlaceableObjectItemDragButton>();
        private List<DresserLocationDragButton> _dresserLocationDragButtons = new List<DresserLocationDragButton>();
        private int _selectedDresserPositionIndex;
        private PlaceableObjectItem _selectedObjectItem;
        private List<HomeDecor.PlacedObject> _originalObjectPlacements = new List<HomeDecor.PlacedObject>();
        private Dictionary<string, OriginalObjectPlacementInfo> _originalObjectPlacementInfo = new Dictionary<string, OriginalObjectPlacementInfo>();
        private Image _draggedItemImage = null;

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            ClearPlacementMarkers();

            SetOriginalObjectPlacements();

            _items = new PageableList<PlaceableObjectItem>(GetOwnedPlaceableObjects(), 6);
            OnPageChanged(_items.CurrentPageItems);
        }

        public void GoToPreviousPage()
        {
            _items.GoToPreviousPage();
        }

        public void GoToNextPage()
        {
            _items.GoToNextPage();
        }

        public void GoToPage(int pageIndex)
        {
            _items.GoToPage(pageIndex);
        }

        public void Save()
        {
            Dismiss();
        }

        public void Cancel()
        {
            for (int i = 0; i < _originalObjectPlacements.Count; i++)
            {
                HomeDecor.PlacedObject placedObject = _originalObjectPlacements[i];
                PlaceableObjectItem placedObjectItem = placedObject != null ? placedObject.PlaceableObjectItem : null;
                PlaceWithData(placedObjectItem, i);
            }

            Dismiss();
        }

        protected void Awake()
        {
            _stuffManager = GameManager.Instance.Stuff;

            for (int i = 0; i < HomeDecor.NumObjectsPlaceableOnDresser; i++)
            {
                DresserLocationDragButton dresserLocationDragButton = Instantiate(DresserLocationDragButtonPrefab, transform);
                dresserLocationDragButton.DresserLocationIndex = i;
                Vector3 dresserPlacementWorldPosition = HomeDecor.GetObjectWorldPositionByIndex(i);
                dresserLocationDragButton.transform.position = MainCamera.WorldToScreenPoint(dresserPlacementWorldPosition + Vector3.up * 0.2f);
                dresserLocationDragButton.onClickDresserLocation += OnSelectDresserLocation;
                dresserLocationDragButton.onBeginDragDresserLocation += OnBeginDragDresserLocation;
                dresserLocationDragButton.onEndDragDresserLocation += OnEndDragDresserLocation;
                _dresserLocationDragButtons.Add(dresserLocationDragButton);
            }
        }

        protected override void Update()
        {
            base.Update();

            if (this._isActive && _draggedItemImage != null)
            {
                Vector2 currentPosition = _draggedItemImage.rectTransform.position;
                _draggedItemImage.rectTransform.position = Vector2.MoveTowards(currentPosition, Input.mousePosition, 32.0f);
            }
        }

        private List<PlaceableObjectItem> GetOwnedPlaceableObjects()
        {
            return _stuffManager.PlaceableItems.List.Where(poi => _stuffManager.HasItem(poi)).ToList();
        }

        private void SetOriginalObjectPlacements()
        {
            _originalObjectPlacements.Clear();
            _originalObjectPlacementInfo.Clear();
            for (int i = 0; i < HomeDecor.NumObjectsPlaceableOnDresser; i++)
            {
                HomeDecor.PlacedObject placedObject = HomeDecor.GetObjectPlacedAtIndex(i);
                _originalObjectPlacements.Add(placedObject);

                if (placedObject != null)
                {
                    string placeableObjectId = placedObject.PlaceableObjectItem.GetId();
                    _originalObjectPlacementInfo.Add(placeableObjectId, new OriginalObjectPlacementInfo
                    {
                        PlacedDateTime = GameManager.Instance.Data.HomeData.GetDresserObjectPlacedDateTime(placeableObjectId),
                        ActivatedDateTime = GameManager.Instance.Data.HomeData.GetDresserObjectActivatedDateTime(placeableObjectId)
                    });
                }
            }
        }

        private void PlaceWithData(PlaceableObjectItem placedObjectItem, int dresserPosition)
        {
            DateTime? placedDateTime = null;
            DateTime? activatedDateTime = null;
            if (placedObjectItem != null)
            {
                string objectId = placedObjectItem.GetId();
                if (_originalObjectPlacementInfo.ContainsKey(objectId))
                {
                    placedDateTime = _originalObjectPlacementInfo[objectId].PlacedDateTime;
                    activatedDateTime = _originalObjectPlacementInfo[objectId].ActivatedDateTime;
                }
            }
            HomeDecor.PlaceObject(placedObjectItem, dresserPosition, placedDateTime, activatedDateTime);
        }

        private void ClearPlacementMarkers()
        {
            _selectedDresserPositionIndex = -1;
            _selectedObjectItem = null;
            if (_draggedItemImage)
            {
                Destroy(_draggedItemImage.gameObject);
            }
        }

        private void OnSelectDresserLocation(int dresserPositionIndex)
        {
            if (_selectedObjectItem)
            {
                PlaceWithData(_selectedObjectItem, dresserPositionIndex);
                ClearPlacementMarkers();
            }
            else if (_selectedDresserPositionIndex != -1)
            {
                HomeDecor.PlacedObject placedObject = HomeDecor.GetObjectPlacedAtIndex(dresserPositionIndex);
                if (placedObject == null) placedObject = HomeDecor.GetObjectPlacedAtIndex(_selectedDresserPositionIndex);

                if (placedObject != null)
                {
                    HomeDecor.MoveObject(_selectedDresserPositionIndex, dresserPositionIndex);
                    ClearPlacementMarkers();
                }
            }
            else
            {
                _selectedDresserPositionIndex = dresserPositionIndex;
            }
        }

        private void OnBeginDragDresserLocation(PointerEventData pointerEvent, int dresserLocationIndex)
        {
            ClearPlacementMarkers();
            HomeDecor.PlacedObject placedObject = HomeDecor.GetObjectPlacedAtIndex(dresserLocationIndex);
            if (placedObject != null)
            {
                _draggedItemImage = Instantiate(DraggedItemImagePrefab, transform);
                _draggedItemImage.sprite = placedObject.PlaceableObjectItem.Icon;
                _draggedItemImage.SetMaterialForItem(placedObject.PlaceableObjectItem.ColorShift);
                _draggedItemImage.transform.position = pointerEvent.position;
            }
        }

        private void OnEndDragDresserLocation(PointerEventData pointerEvent, int dresserLocationIndex)
        {
            if (dresserLocationIndex != -1)
            {
                int hoveredDresserLocationIndex = -1;
                foreach (DresserLocationDragButton dresserLocationDragButton in _dresserLocationDragButtons)
                {
                    if (dresserLocationDragButton.DresserLocationIndex != dresserLocationIndex &&
                        dresserLocationDragButton.IsPositionWithinBounds(pointerEvent.position))
                    {
                        hoveredDresserLocationIndex = dresserLocationDragButton.DresserLocationIndex;
                        continue;
                    }
                }
                if (hoveredDresserLocationIndex != -1)
                {
                    HomeDecor.MoveObject(dresserLocationIndex, hoveredDresserLocationIndex);
                }
                // TODO: Else if dragged over "remove" area (whatever that ends up being), remove it.
            }

            ClearPlacementMarkers();
        }

        private void OnSelectPlaceableObjectItem(PlaceableObjectItem selection)
        {
            if (_selectedDresserPositionIndex != -1)
            {
                PlaceWithData(selection, _selectedDresserPositionIndex);
                ClearPlacementMarkers();
            }
            else
            {
                _selectedObjectItem = selection;
            }
        }

        private void OnBeginDragPlaceableObjectItem(PointerEventData pointerEvent, PlaceableObjectItem draggedItem)
        {
            ClearPlacementMarkers();

            if (draggedItem)
            {
                _draggedItemImage = Instantiate(DraggedItemImagePrefab, transform);
                _draggedItemImage.sprite = draggedItem.Icon;
                _draggedItemImage.SetMaterialForItem(draggedItem.ColorShift);
                _draggedItemImage.transform.position = pointerEvent.position;
            }
        }

        private void OnEndDragPlaceableObjectItem(PointerEventData pointerEvent, PlaceableObjectItem objectItem)
        {
            foreach (DresserLocationDragButton locationButton in _dresserLocationDragButtons)
            {
                if (locationButton.IsPositionWithinBounds(pointerEvent.position))
                {
                    PlaceWithData(objectItem, locationButton.DresserLocationIndex);
                    continue;
                }
            }

            ClearPlacementMarkers();
        }

        private void OnPageChanged(List<PlaceableObjectItem> pageItems)
        {
            foreach (PlaceableObjectItemDragButton itemButton in _placeableObjectItemDragButtons)
            {
                itemButton.onClickPlaceableObjectItem -= OnSelectPlaceableObjectItem;
                itemButton.onBeginDragPlaceableObjectItem -= OnBeginDragPlaceableObjectItem;
                itemButton.onEndDragPlaceableObjectItem -= OnEndDragPlaceableObjectItem;
                Destroy(itemButton.gameObject);
            }
            _placeableObjectItemDragButtons.Clear();

            foreach (PlaceableObjectItem item in pageItems)
            {
                PlaceableObjectItemDragButton newItemButton = Instantiate(ItemDragButtonPrefab, ItemButtonsRectTransform);
                newItemButton.PlaceableObjectItem = item;
                newItemButton.onClickPlaceableObjectItem += OnSelectPlaceableObjectItem;
                newItemButton.onBeginDragPlaceableObjectItem += OnBeginDragPlaceableObjectItem;
                newItemButton.onEndDragPlaceableObjectItem += OnEndDragPlaceableObjectItem;
                _placeableObjectItemDragButtons.Add(newItemButton);
            }

            UpdatePageNavigationButtons();
        }

        private void UpdatePageNavigationButtons()
        {
            PreviousPageButton.interactable = !_items.IsFirstPage;
            NextPageButton.interactable = !_items.IsLastPage;
        }
    }
}