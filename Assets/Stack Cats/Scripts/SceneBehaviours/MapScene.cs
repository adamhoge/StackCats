using UnityEngine;
using System.Collections.Generic;

namespace Tofuwu.StackCats
{
    public delegate void PuzzleAreaMapActive(MapScene.MapItem mapItem);
    public delegate void PuzzleAreaMapInactive(MapScene.MapItem mapItem);
    public delegate void NavigatingToMapItem(MapScene.MapItem mapItem);
    public delegate void RevealingPuzzleAreaMap(MapScene.MapItem mapItem);
    public delegate void RevealedPuzzleAreaMap(MapScene.MapItem mapItem);

    // TODO: Optimization, currently when a map is visited, it stays active.
    //   Could inactivate maps off screen, but probably not worth the effort
    //   since players won't spend very long on this screen.
    public class MapScene : SceneBehaviour
    {
        public class MapItem
        {
            public PuzzleArea PuzzleArea;
            public PuzzleAreaMap PuzzleAreaMap;
        }

        private enum MapSceneState
        {
            None,
            ViewingMap,
            Navigating,
            RevealingArea
        }

        /// <summary>
        /// Invoked whenever a puzzle area map becomes active.
        /// </summary>
        public event PuzzleAreaMapActive onPuzzleAreaMapActive;

        /// <summary>
        /// Invoked whenever a puzzle area map becomes inactive.
        /// </summary>
        public event PuzzleAreaMapInactive onPuzzleAreaMapInactive;

        /// <summary>
        /// Invoked when navigating to a new map item.
        /// </summary>
        public event NavigatingToMapItem onNavigatingToMapItem;

        /// <summary>
        /// Invoked whenever a puzzle area map is being revealed.
        /// </summary>
        public event RevealingPuzzleAreaMap onRevealingPuzzleAreaMap;

        /// <summary>
        /// Invoked whenever a puzzle area map reveal has completed.
        /// </summary>
        public event RevealedPuzzleAreaMap onRevealedPuzzleAreaMap;

        public Camera Camera;
        public LeanTweenType MapTransitionTweenType;
        public float MapTransitionDuration = 0.5f;
        public LeanTweenType MapRevealTweenType;
        public LeanTweenType MapRevealReturnTweenType;
        public float MapRevealDelay = 2.0f;
        public float MapRevealDuration = 1.5f;
        public float MapRevealHoldDuration = 1.0f;
        public float MapRevealReturnDuration = 1.5f;
        public GameObject CloudsLeft;
        public GameObject CloudsRight;

        /// <summary>
        /// The size of all area maps combined.
        /// </summary>
        public Vector2 FullMapSize { get { return new Vector2(TOTAL_AREA_MAP_WIDTH, TOTAL_AREA_MAP_HEIGHT * _mapItems.Count); } }

        public float UsableAreaMapWidth { get { return USABLE_AREA_MAP_WIDTH; } }

        /// <summary>
        /// The height of each area map.
        /// </summary>
        public float UsableAreaMapHeight { get { return USABLE_AREA_MAP_HEIGHT; } }

        /// <summary>
        /// The total height of each area map.
        /// </summary>
        public float TotalAreaMapHeight { get { return TOTAL_AREA_MAP_HEIGHT; } }

        /// <summary>
        /// All map items in the map scene.
        /// </summary>
        public List<MapItem> MapItems { get { return _mapItems; } }

        public bool IsViewingMap { get { return _state == MapSceneState.ViewingMap; } }

        public int CurrentMapIndex { get { return _currentMapItemIndex; } }

        private const float USABLE_AREA_MAP_WIDTH = 880.0f / 1820.0f;
        private const float USABLE_AREA_MAP_HEIGHT = 1480.0f / 1820.0f;
        private const float TOTAL_AREA_MAP_WIDTH = 2048.0f / 1820.0f;
        private const float TOTAL_AREA_MAP_HEIGHT = 2048.0f / 1820.0f;
        private const float CAMERA_ORTHOGRAPHIC_SIZE = 0.5625f;
        private MapSceneState _state;
        private AudioManager _audioManager;
        private PuzzleManager _puzzleManager;
        private int _currentMapItemIndex = -1;
        private List<MapItem> _mapItems = new List<MapItem>();

        public void GoHome()
        {
            _gameManager.GoHome();
        }

        public void ViewMapItem(MapItem mapItem, bool animated = true)
        {
            if (_state == MapSceneState.RevealingArea) return;

            int mapItemIndex = _mapItems.IndexOf(mapItem);
            if (mapItemIndex == -1 || mapItemIndex == _currentMapItemIndex) return;

            if (_state == MapSceneState.ViewingMap)
            {
                MapItem currentMapItem = _mapItems[_currentMapItemIndex];
                if (onPuzzleAreaMapInactive != null) onPuzzleAreaMapInactive(currentMapItem);
            }
            else if (_state == MapSceneState.Navigating)
            {
                LeanTween.cancel(Camera.gameObject);
            }

            _currentMapItemIndex = mapItemIndex;
            _state = MapSceneState.Navigating;
            _mapItems[_currentMapItemIndex].PuzzleAreaMap.gameObject.SetActive(true);
            if (onNavigatingToMapItem != null) onNavigatingToMapItem(_mapItems[_currentMapItemIndex]);

            float cameraYPosition = _currentMapItemIndex * TOTAL_AREA_MAP_HEIGHT;
            if (animated)
            {
                LeanTween.cancel(Camera.gameObject);
                LeanTween.moveY(Camera.gameObject, cameraYPosition, MapTransitionDuration)
                    .setEase(LeanTweenType.easeOutQuint)
                    .setOnComplete(CompleteNavigation);
            }
            else
            {
                Camera.transform.position = Vector2.up * cameraYPosition;
                CompleteNavigation();
            }
        }

        public void ViewPreviousPuzzleArea()
        {
            if (_currentMapItemIndex == -1) return;

            if (_currentMapItemIndex > 0)
            {
                ViewMapItem(_mapItems[_currentMapItemIndex - 1]);
            }
            else if (_state == MapSceneState.ViewingMap)
            {
                LeanTween.cancel(Camera.gameObject);
                Camera.transform.position = new Vector2(Camera.transform.position.x, GetMapPositionByIndex(_currentMapItemIndex).y);
                LeanTween.moveY(Camera.gameObject, Camera.transform.position.y - 0.0125f, MapTransitionDuration)
                    .setEase(LeanTweenType.punch);
            }
        }

        public void ViewNextPuzzleArea()
        {
            if (_currentMapItemIndex == -1) return;

            if (_currentMapItemIndex + 1 < _mapItems.Count)
            {
                ViewMapItem(_mapItems[_currentMapItemIndex + 1]);
            }
            else if (_state == MapSceneState.ViewingMap)
            {
                LeanTween.cancel(Camera.gameObject);
                Camera.transform.position = new Vector2(Camera.transform.position.x, GetMapPositionByIndex(_currentMapItemIndex).y);
                LeanTween.moveY(Camera.gameObject, Camera.transform.position.y + 0.0125f, MapTransitionDuration)
                    .setEase(LeanTweenType.punch);
            }
        }

        public Vector3 GetMapPositionByIndex(int mapIndex)
        {
            return new Vector3(0.0f, mapIndex * TOTAL_AREA_MAP_HEIGHT, mapIndex);
        }

        protected override void Awake()
        {
            base.Awake();

            _audioManager = _gameManager.Audio;
            _puzzleManager = _gameManager.Puzzles;

            List<PuzzleArea> puzzleAreas = _puzzleManager.PuzzleAreaCollection.List;

            for (int i = 0; i < puzzleAreas.Count; i++)
            {
                PuzzleArea puzzleArea = puzzleAreas[i];
                
                if (_puzzleManager.IsAreaLocked(puzzleArea)) continue;

                PuzzleAreaMap puzzleAreaMap = Instantiate(puzzleArea.PuzzleAreaMap, transform);
                puzzleAreaMap.gameObject.SetActive(false);
                puzzleAreaMap.transform.position = GetMapPositionByIndex(i);
                _mapItems.Add(new MapItem { PuzzleArea = puzzleArea, PuzzleAreaMap = puzzleAreaMap });
            }
        }

        protected override void Start()
        {
            base.Start();

            MapItem currentMapItem = _mapItems.Find(mi => mi.PuzzleArea == _puzzleManager.CurrentArea);
            ViewMapItem(currentMapItem, false);
            _puzzleManager.OneOffPuzzleAreaUnlocked.ConsumeAll(OnPuzzleAreaUnlocked);

            if (_gameManager.PreviousGameScene == GameScene.Home)
            {
                //LeanTween.value(Camera.gameObject, Camera.orthographicSize + 0.15f, Camera.orthographicSize, 1.5f)
                //    .setEase(LeanTweenType.easeOutCubic)
                //    .setOnUpdate((float newOrthoSize) =>
                //    {
                //        Camera.orthographicSize = newOrthoSize;
                //    });

                LeanTween.moveLocalX(CloudsLeft, -0.02f, 2.0f);
                LeanTween.moveLocalX(CloudsRight, 0.02f, 2.0f);
                LeanTween.alpha(CloudsLeft, 0.0f, 2.0f);
                LeanTween.alpha(CloudsRight, 0.0f, 2.0f);
            }
            else
            {
                CloudsLeft.SetActive(false);
                CloudsRight.SetActive(false);
            }
        }

        private void CompleteNavigation()
        {
            MapItem currentMapItem = _mapItems[_currentMapItemIndex];
            PuzzleArea currentPuzzleArea = currentMapItem.PuzzleArea;
            if (onPuzzleAreaMapActive != null) onPuzzleAreaMapActive(currentMapItem);
            _state = MapSceneState.ViewingMap;

            if (_puzzleManager.CurrentArea != currentPuzzleArea)
            {
                _puzzleManager.CurrentArea = currentPuzzleArea;
                List<StoryPuzzle> storyPuzzles = _puzzleManager.GetStoryPuzzles(currentPuzzleArea);
                if (storyPuzzles.Count > 0)
                {
                    _puzzleManager.CurrentStoryPuzzle = storyPuzzles[0];
                }
            }

            AudioLoop backgroundMusic = currentMapItem.PuzzleArea.PuzzleTheme.BackgroundMusic;
            if (_audioManager.BackgroundMusic.CurrentAudioLoop != backgroundMusic)
                _audioManager.PlayBackgroundMusic(backgroundMusic);
        }

        private void CompleteReveal()
        {
            MapItem currentMapItem = _mapItems[_currentMapItemIndex];
            if (onPuzzleAreaMapActive != null) onPuzzleAreaMapActive(currentMapItem);
            if (onRevealedPuzzleAreaMap != null) onRevealedPuzzleAreaMap(currentMapItem);

            _puzzleManager.CurrentArea = _mapItems[_currentMapItemIndex].PuzzleArea;

            AudioLoop backgroundMusic = currentMapItem.PuzzleArea.PuzzleTheme.BackgroundMusic;
            if (_audioManager.BackgroundMusic.CurrentAudioLoop != backgroundMusic)
                _audioManager.PlayBackgroundMusic(backgroundMusic);

            _state = MapSceneState.ViewingMap;
        }

        private void OnPuzzleAreaUnlocked(PuzzleManager.PuzzleAreaUnlockedEvent puzzleAreaUnlockedEvent)
        {
            int mapIndex = _mapItems.FindIndex(mi => mi.PuzzleArea == puzzleAreaUnlockedEvent.PuzzleArea);
            Vector2 mapPosition = GetMapPositionByIndex(mapIndex);
            _currentMapItemIndex = mapIndex;
            _mapItems[_currentMapItemIndex].PuzzleAreaMap.gameObject.SetActive(true);

            LeanTween.moveY(Camera.gameObject, mapPosition.y, MapRevealDuration)
                .setDelay(MapRevealDelay)
                .setEase(MapRevealTweenType)
                .setOnComplete(CompleteReveal);
            //LeanTween.moveY(Camera.gameObject, Camera.transform.position.y, MapRevealReturnDuration)
            //    .setDelay(MapRevealDuration + 1.0f + MapRevealHoldDuration)
            //    .setEase(MapRevealReturnTweenType)
            //    .setOnComplete(CompleteReveal);

            _state = MapSceneState.RevealingArea;
            if (onRevealingPuzzleAreaMap != null) onRevealingPuzzleAreaMap(_mapItems.Find(mi => mi.PuzzleArea == puzzleAreaUnlockedEvent.PuzzleArea));
        }
    }
}