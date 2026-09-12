using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class MapSceneUI : MonoBehaviour
    {
        public MapScene MapScene;
        public Canvas WorldCanvas;
        public RectTransform TargetMapIndicatorRectTransform;
        public RectTransform StarsEarnedRectTransform;
        public List<CanvasGroup> MapInformationCanvasGroups;
        public TextMeshProUGUI CatsSeenText;
        public Image StarsEarnedIcon;
        public CanvasGroup StarsEarnedIconFlashCanvasGroup;
        public DynamicAudioEvent StarsEarnedSoundEffect;
        public DynamicAudioEvent IncrementStarsSoundEffect;
        public TextMeshProUGUI StarsEarnedText;
        public RectTransform MapNavigationRectTransform;
        public Button PreviousMapButton;
        public Button NextMapButton;
        public List<RectTransform> ThemedImages = new List<RectTransform>();
        public Sprite StarEarnedSprite;
        public PuzzleButton PuzzleButtonPrefab;
        public GameObject NavigationDotPrefab;

        private AudioManager _audioManager;
        private CatManager _catManager;
        private PuzzleManager _puzzleManager;
        private readonly Dictionary<MapScene.MapItem, PuzzleAreaMapUI> _puzzleAreaMaps = new Dictionary<MapScene.MapItem, PuzzleAreaMapUI>();
        private readonly Dictionary<MapScene.MapItem, GameObject> _navigationDots = new Dictionary<MapScene.MapItem, GameObject>();
        private MapScene.MapItem _currentMapItem;
        private int _currentStarsEarnedDisplayed;
        private int _totalStarsInArea;
        private List<Image> _starImages = new List<Image>();
        private bool _isMapNavigable;

        protected void Awake()
        {
            _audioManager = GameManager.Instance.Audio;
            _catManager = GameManager.Instance.Cats;
            _puzzleManager = GameManager.Instance.Puzzles;

            _isMapNavigable = MapScene.MapItems.Count > 1;

            foreach (MapScene.MapItem mapItem in MapScene.MapItems)
            {
                PuzzleAreaMapUI newPuzzleAreaMap = new GameObject(mapItem.PuzzleAreaMap.name + " UI").AddComponent<PuzzleAreaMapUI>();
                newPuzzleAreaMap.transform.SetParent(WorldCanvas.transform);
                newPuzzleAreaMap.PuzzleButtonPrefab = PuzzleButtonPrefab;
                newPuzzleAreaMap.Initialize(mapItem.PuzzleArea, mapItem.PuzzleAreaMap);
                newPuzzleAreaMap.onStarsEarned += OnStarsEarned;
                newPuzzleAreaMap.onButtonStarsAdded += OnStarsAdded;
                _puzzleAreaMaps.Add(mapItem, newPuzzleAreaMap);

                if (_isMapNavigable)
                {
                    GameObject navigationDot = Instantiate(NavigationDotPrefab);
                    navigationDot.transform.SetParent(TargetMapIndicatorRectTransform);
                    navigationDot.transform.SetAsFirstSibling();
                    navigationDot.transform.localScale = Vector3.one;
                    _navigationDots.Add(mapItem, navigationDot);
                }
            }

            MapNavigationRectTransform.gameObject.SetActive(_isMapNavigable);
        }

        protected void OnEnable()
        {
            MapScene.onPuzzleAreaMapInactive += OnPuzzleAreaMapInactive;
            MapScene.onPuzzleAreaMapActive += OnPuzzleAreaMapActive;
            MapScene.onNavigatingToMapItem += OnNavigatingToMapItem;
            MapScene.onRevealingPuzzleAreaMap += OnRevealingPuzzleAreaMap;
            MapScene.onRevealedPuzzleAreaMap += OnRevealedPuzzleAreaMap;
        }

        protected void OnDisable()
        {
            MapScene.onPuzzleAreaMapInactive -= OnPuzzleAreaMapInactive;
            MapScene.onPuzzleAreaMapActive -= OnPuzzleAreaMapActive;
            MapScene.onNavigatingToMapItem -= OnNavigatingToMapItem;
            MapScene.onRevealingPuzzleAreaMap -= OnRevealingPuzzleAreaMap;
            MapScene.onRevealedPuzzleAreaMap -= OnRevealedPuzzleAreaMap;
        }

        private void IncrementStars(object itemImage)
        {
            Image image = (Image)itemImage;

            ++_currentStarsEarnedDisplayed;
            StarsEarnedText.text = _currentStarsEarnedDisplayed + "/" + _totalStarsInArea;

            LeanTween.cancel(StarsEarnedIcon.gameObject);
            StarsEarnedIcon.transform.localScale = Vector3.one;
            LeanTween.scaleX(StarsEarnedIcon.gameObject, Random.Range(1.15f, 1.35f), 0.35f).setEase(LeanTweenType.punch);
            LeanTween.scaleY(StarsEarnedIcon.gameObject, Random.Range(1.15f, 1.35f), 0.35f).setEase(LeanTweenType.punch);

            LeanTween.cancel(StarsEarnedIconFlashCanvasGroup.gameObject);
            StarsEarnedIconFlashCanvasGroup.alpha = 0.9f;
            LeanTween.alphaCanvas(StarsEarnedIconFlashCanvasGroup, 0.0f, 0.35f).setEase(LeanTweenType.easeInSine);

            _audioManager.PlaySoundEffect(IncrementStarsSoundEffect);

            _starImages.Remove(image);
            Destroy(image.gameObject);
        }

        private void OnPuzzleAreaMapInactive(MapScene.MapItem mapItem)
        {
            //_puzzleAreaMaps[mapItem].SetActive(false);

            foreach (Image image in _starImages)
            {
                LeanTween.cancel(image.gameObject);
                Destroy(image.gameObject);
            }
            _starImages.Clear();
            foreach (CanvasGroup canvasGroup in MapInformationCanvasGroups)
            {
                LeanTween.alphaCanvas(canvasGroup, 0.0f, 0.25f).setEase(LeanTweenType.easeOutQuint);
            }
        }

        private void OnPuzzleAreaMapActive(MapScene.MapItem mapItem)
        {
            PreviousMapButton.interactable = true;
            NextMapButton.interactable = true;

            PuzzleArea puzzleArea = mapItem.PuzzleArea;

            List<StoryPuzzle> storyPuzzles = _puzzleManager.GetStoryPuzzles(puzzleArea);
            _totalStarsInArea = _puzzleManager.GetNumStarsInArea(puzzleArea);
            _currentStarsEarnedDisplayed = _puzzleManager.GetTotalNumStarsEarned(puzzleArea);

            List<Cat> CatsInArea = puzzleArea.ProgressionPuzzleCats;
            int totalCatsInArea = CatsInArea.Count;
            int catsSeenInArea = CatsInArea.Count(c => _catManager.WasCatSeen(c));

            StarsEarnedText.text = _currentStarsEarnedDisplayed + "/" + _totalStarsInArea;
            CatsSeenText.text = catsSeenInArea + "/" + totalCatsInArea;
            foreach (CanvasGroup canvasGroup in MapInformationCanvasGroups)
            {
                LeanTween.alphaCanvas(canvasGroup, 1.0f, 0.25f).setEase(LeanTweenType.easeOutQuint);
            }

            _puzzleAreaMaps[_currentMapItem].SetActive(true);
        }

        private void OnNavigatingToMapItem(MapScene.MapItem mapItem)
        {
            if (_currentMapItem != null)
            {
                _puzzleAreaMaps[_currentMapItem].SetActive(false);

                GameObject previousNavigationDot = _navigationDots[_currentMapItem];
                LeanTween.cancel(previousNavigationDot);
                LeanTween.scale(previousNavigationDot, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutQuint);

                foreach (RectTransform themedImage in ThemedImages)
                {
                    LeanTween.cancel(themedImage);
                    LeanTween.color(themedImage, mapItem.PuzzleArea.PuzzleTheme.UIColor, 0.5f).setEase(LeanTweenType.easeOutQuint).setRecursive(false);
                }
            }
            else
            {
                foreach (RectTransform themedImage in ThemedImages)
                {
                    themedImage.GetComponent<Image>().color = mapItem.PuzzleArea.PuzzleTheme.UIColor;
                }
            }

            _currentMapItem = mapItem;
            _puzzleAreaMaps[_currentMapItem].SetActive(false);

            if (_isMapNavigable)
            {
                GameObject currentNavigationDot = _navigationDots[_currentMapItem];
                LeanTween.cancel(currentNavigationDot);
                LeanTween.scale(currentNavigationDot, Vector3.one * 1.75f, 0.5f).setEase(LeanTweenType.easeOutQuint);
            }
        }

        private void OnStarsEarned(int numStarsEarned)
        {
            _currentStarsEarnedDisplayed -= numStarsEarned;
            StarsEarnedText.text = _currentStarsEarnedDisplayed + "/" + _totalStarsInArea;
        }

        private void OnStarsAdded(Vector3 worldSpaceLocation, int numStarsEarned)
        {
            _audioManager.PlaySoundEffect(StarsEarnedSoundEffect);

            Vector2 screenSpaceLocation = MapScene.Camera.WorldToScreenPoint(worldSpaceLocation);

            Vector3 starsEarnedIconPosition = StarsEarnedIcon.transform.position;
            for (int i = 0; i < numStarsEarned; i++)
            {
                Image starImage = new GameObject().AddComponent<Image>();
                starImage.transform.SetParent(StarsEarnedRectTransform);
                starImage.rectTransform.localScale = Vector3.one;
                starImage.rectTransform.sizeDelta = new Vector2(48.0f, 48.0f);
                starImage.rectTransform.position = screenSpaceLocation;
                starImage.sprite = StarEarnedSprite;
                _starImages.Add(starImage);

                float expandX = (-numStarsEarned / 2.0f + i + 0.5f) * 48.0f;
                Vector3 xShiftPosition = new Vector3(Random.Range(-64.0f, 64.0f), Random.Range(32.0f, 64.0f), 0.0f);
                float expandY = 48.0f;
                float moveDelay = 0.4f + i * 0.15f;
                Vector3 expandPosition = starImage.transform.position + new Vector3(expandX, expandY, 0.0f);

                LeanTween.move(starImage.gameObject, expandPosition, moveDelay)
                    .setEase(LeanTweenType.easeOutCubic);
                LTBezierPath starPath = new LTBezierPath(new Vector3[]{
                    expandPosition,
                    expandPosition + xShiftPosition,
                    expandPosition + xShiftPosition,
                    starsEarnedIconPosition
                });
                LeanTween.move(starImage.gameObject, starPath, 0.4f)
                    .setDelay(moveDelay)
                    .setEase(LeanTweenType.easeInSine)
                    .setOnComplete(IncrementStars, starImage);
            }
        }

        private void OnRevealingPuzzleAreaMap(MapScene.MapItem mapItem)
        {
            PreviousMapButton.interactable = false;
            NextMapButton.interactable = false;

            int mapIndex = MapScene.MapItems.IndexOf(mapItem);
            PuzzleAreaMapUI revealedMap = _puzzleAreaMaps[MapScene.MapItems[mapIndex]];

            foreach (KeyValuePair<MapScene.MapItem, PuzzleAreaMapUI> puzzleAreaMap in _puzzleAreaMaps)
            {
                puzzleAreaMap.Value.SetActive(false);
            }

            revealedMap.RevealArea();
        }

        private void OnRevealedPuzzleAreaMap(MapScene.MapItem mapItem)
        {
            foreach (RectTransform themedImage in ThemedImages)
            {
                LeanTween.cancel(themedImage);
                LeanTween.color(themedImage, mapItem.PuzzleArea.PuzzleTheme.UIColor, 0.5f).setEase(LeanTweenType.easeOutQuint).setRecursive(false);
            }

            foreach (KeyValuePair<MapScene.MapItem, PuzzleAreaMapUI> puzzleAreaMap in _puzzleAreaMaps)
            {
                puzzleAreaMap.Value.SetActive(true);
            }
        }
    }
}