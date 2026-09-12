using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public delegate void DresserSet(DresserItem dresser);

    public class HomeDecor : MonoBehaviour
    {
        public class PlacedObject
        {
            public PlaceableObjectItem PlaceableObjectItem;
            public PlaceableObject PlaceableObjectInstance;
        }

        public event DresserSet onDresserSet;

        public SpriteRenderer Wallpaper;
        public SpriteRenderer Floor;
        public SpriteRenderer Window;
        public SpriteRenderer Dresser;
        public Transform DresserSurfaceTransform;
        public float DresserWidth;
        public int NumObjectsPlaceableOnDresser = 5;

        public WallpaperItem CurrentWallpaper { get => _currentWallpaper; }

        public FloorItem CurrentFloor { get => _currentFloor; }

        public WindowItem CurrentWindow { get => _currentWindow; }

        public DresserItem CurrentDresser { get => _currentDresser; }

        private DataManager _dataManager;
        private StuffManager _stuffManager;
        private WallpaperItem _currentWallpaper;
        private FloorItem _currentFloor;
        private WindowItem _currentWindow;
        private DresserItem _currentDresser;
        private List<PlacedObject> _objectsPlacedOnDresser;

        public void SetWallpaper(WallpaperItem wallpaper)
        {
            Wallpaper.sprite = wallpaper.WallpaperSprite;
            Wallpaper.SetMaterialForItem(wallpaper.ColorShift);
            _dataManager.HomeData.CurrentWallpaperId = wallpaper.GetId();
            _currentWallpaper = wallpaper;
        }

        public void SetFloor(FloorItem floor)
        {
            Floor.sprite = floor.FloorSprite;
            Floor.SetMaterialForItem(floor.ColorShift); ;
            _dataManager.HomeData.CurrentFloorId = floor.GetId();
            _currentFloor = floor;
        }

        public void SetWindow(WindowItem window)
        {
            Window.sprite = window.WindowSprite;
            _dataManager.HomeData.CurrentWindowId = window.GetId();
            _currentWindow = window;
        }

        public void SetDresser(DresserItem dresser)
        {
            Dresser.sprite = dresser.DresserSprite;
            Dresser.SetMaterialForItem(dresser.ColorShift);
            _currentDresser = dresser;
            _dataManager.HomeData.CurrentDresserId = dresser.GetId();

            if (onDresserSet != null) onDresserSet(dresser);
        }

        public void PlaceObject(PlaceableObjectItem placeableObject, int dresserPositionIndex, DateTime? placedDateTime = null, DateTime? activatedDateTime = null)
        {
            int currentIndex = GetPlaceableObjectIndex(placeableObject);
            if (currentIndex != -1) PutAwayObject(currentIndex);

            PutAwayObject(dresserPositionIndex);

            if (placeableObject == null) return;

            if (activatedDateTime == null) activatedDateTime = DateTime.Now;
            LoadPlaceableObject(placeableObject, dresserPositionIndex, activatedDateTime.Value);
            _dataManager.HomeData.SetDresserObjectId(dresserPositionIndex, placeableObject.GetId(), placedDateTime, activatedDateTime);
        }

        public void PutAwayObject(int dresserPositionIndex)
        {
            PlacedObject placedObject = _objectsPlacedOnDresser[dresserPositionIndex];

            if (placedObject != null)
            {
                placedObject.PlaceableObjectInstance.OnPutAway();
                placedObject.PlaceableObjectInstance.onPlaceableObjectActivated -= OnPlaceableObjectActivated;
                Destroy(placedObject.PlaceableObjectInstance.gameObject);
                _objectsPlacedOnDresser[dresserPositionIndex] = null;
                _dataManager.HomeData.SetDresserObjectId(dresserPositionIndex, null);
            }
        }

        public void MoveObject(int fromDresserPositionIndex, int toDresserPositionIndex)
        {
            PlacedObject fromPlacement = _objectsPlacedOnDresser[fromDresserPositionIndex];
            PlaceableObjectItem fromPlaceableObjectItem = null;
            DateTime? fromPlacedDateTime = null;
            DateTime? fromActivatedDateTime = null;
            if (fromPlacement != null)
            {
                fromPlaceableObjectItem = fromPlacement.PlaceableObjectItem;
                fromPlacedDateTime = _dataManager.HomeData.GetDresserObjectPlacedDateTime(fromPlaceableObjectItem.GetId());
                fromActivatedDateTime = _dataManager.HomeData.GetDresserObjectActivatedDateTime(fromPlaceableObjectItem.GetId());
            }

            PlacedObject toPlacement = _objectsPlacedOnDresser[toDresserPositionIndex];
            PlaceableObjectItem toPlaceableObjectItem = null;
            DateTime? toPlacedDateTime = null;
            DateTime? toActivatedDateTime = null;
            if (toPlacement != null)
            {
                toPlaceableObjectItem = toPlacement.PlaceableObjectItem;
                toPlacedDateTime = _dataManager.HomeData.GetDresserObjectPlacedDateTime(toPlaceableObjectItem.GetId());
                toActivatedDateTime = _dataManager.HomeData.GetDresserObjectActivatedDateTime(toPlaceableObjectItem.GetId());
            }
            PlaceObject(fromPlaceableObjectItem, toDresserPositionIndex, fromPlacedDateTime, fromActivatedDateTime);
            PlaceObject(toPlaceableObjectItem, fromDresserPositionIndex, toPlacedDateTime, toActivatedDateTime);
        }

        public PlacedObject GetObjectPlacedAtIndex(int dresserPositionIndex)
        {
            if (dresserPositionIndex < 0 || dresserPositionIndex >= NumObjectsPlaceableOnDresser)
            {
                return null;
            }

            return _objectsPlacedOnDresser[dresserPositionIndex];
        }

        public Vector3 GetObjectLocalPositionByIndex(int index)
        {
            float dresserItemWidth = DresserWidth / NumObjectsPlaceableOnDresser;
            float xPosition = -DresserWidth / 2 + dresserItemWidth * index + dresserItemWidth / 2;
            return Vector3.right * xPosition;
        }

        public Vector3 GetObjectWorldPositionByIndex(int index)
        {
            return DresserSurfaceTransform.position + GetObjectLocalPositionByIndex(index);
        }

        protected void Awake()
        {
            _dataManager = GameManager.Instance.Data;
            _stuffManager = GameManager.Instance.Stuff;
        }

        protected void Start()
        {
            SetWallpaper(_stuffManager.WallpaperItems.GetById(_dataManager.HomeData.CurrentWallpaperId));
            SetFloor(_stuffManager.FloorItems.GetById(_dataManager.HomeData.CurrentFloorId));
            SetWindow(_stuffManager.WindowItems.GetById(_dataManager.HomeData.CurrentWindowId));
            SetDresser(_stuffManager.DresserItems.GetById(_dataManager.HomeData.CurrentDresserId));
            LoadPlaceableObjects();
        }

        private void LoadPlaceableObject(PlaceableObjectItem placeableObjectItem, int dresserPositionIndex, DateTime lastActivationTime)
        {
            if (placeableObjectItem == null) return;

            PlaceableObject objectInstance = Instantiate(placeableObjectItem.PlaceableObjectPrefab, DresserSurfaceTransform);
            objectInstance.LastActivationTime = lastActivationTime;
            objectInstance.onPlaceableObjectActivated += OnPlaceableObjectActivated;
            objectInstance.transform.localPosition = GetObjectLocalPositionByIndex(dresserPositionIndex);
            objectInstance.transform.localScale = GetPlaceableObjectScale();
            _objectsPlacedOnDresser[dresserPositionIndex] = new PlacedObject { PlaceableObjectItem = placeableObjectItem, PlaceableObjectInstance = objectInstance };
            objectInstance.OnPlaced();
        }

        private void LoadPlaceableObjects()
        {
            _objectsPlacedOnDresser = new List<PlacedObject>(NumObjectsPlaceableOnDresser);
            for (int i = 0; i < NumObjectsPlaceableOnDresser; i++)
            {
                _objectsPlacedOnDresser.Add(null);
                string placeableObjectId = _dataManager.HomeData.GetDresserObjectId(i);
                DateTime lastActivationTime = _dataManager.HomeData.GetDresserObjectActivatedDateTime(placeableObjectId);
                PlaceableObjectItem placeableObjectItem = _stuffManager.PlaceableItems.GetById(placeableObjectId);
                LoadPlaceableObject(placeableObjectItem, i, lastActivationTime);
            }
        }

        private int GetPlaceableObjectIndex(PlaceableObjectItem placeableObjectItem)
        {
            for (int i = 0; i < NumObjectsPlaceableOnDresser; i++)
            {
                PlacedObject currentPlacedObject = _objectsPlacedOnDresser[i];
                if (currentPlacedObject != null && currentPlacedObject.PlaceableObjectItem == placeableObjectItem)
                {
                    return i;
                }
            }

            return -1;
        }

        private Vector3 GetPlaceableObjectScale()
        {
            return Vector3.one * (DresserWidth / NumObjectsPlaceableOnDresser);
        }

        private void OnPlaceableObjectActivated(PlaceableObject placeableObject, DateTime activationTime)
        {
            PlacedObject placedObject = _objectsPlacedOnDresser.FirstOrDefault(po => po != null && po.PlaceableObjectInstance == placeableObject);
            if (placedObject != null)
            {
                _dataManager.HomeData.ActivateDresserObjectId(placedObject.PlaceableObjectItem.GetId(), activationTime);
            }
        }
    }
}