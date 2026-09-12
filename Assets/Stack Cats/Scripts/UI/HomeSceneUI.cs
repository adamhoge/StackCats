using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

namespace Tofuwu.StackCats.UI
{
    [RequireComponent(typeof(HomeDecorUI))]
    public class HomeSceneUI : MonoBehaviour
    {
        public HomeScene HomeScene;
        public Button PresentButton;
        public Button PuzzleMakerButton;
        public TextMeshProUGUI CatsButtonText;
        public RectTransform PresentCountRectTransform;
        public TextMeshProUGUI PresentCountText;
        public CanvasGroup HomeMenuLayoutCanvasGroup;
        public List<Button> DrawerButtons;
        public OverlayScreenManager OverlayScreenManager;
        public PlayModeSelectionOverlayScreen PlayModeSelectionOverlayScreen;
        public ChallengeAreaSelectionOverlayScreen ChallengeAreaSelectionOverlayScreen;
        public EndlessAreaSelectionOverlayScreen EndlessAreaSelectionOverlayScreen;
        public ItemReceivedOverlayScreen ItemReceivedOverlayScreen;

        private GameManager _gameManager;
        private CatManager _catManager;
        private StuffManager _stuffManager;
        private HomeDecorUI _homeDecor;

        protected void Awake()
        {
            _gameManager = GameManager.Instance;
            _catManager = _gameManager.Cats;
            _stuffManager = _gameManager.Stuff;
            _homeDecor = GetComponent<HomeDecorUI>();
        }

        protected void OnEnable()
        {
            HomeScene.onSelectPlayMode += OnSelectPlayMode;
            HomeScene.onSelectChallengeArea += OnSelectChallengeArea;
            HomeScene.onSelectEndlessArea += OnSelectEndlessArea;
            _stuffManager.onPresentAdded += OnPresentAdded;
            _stuffManager.onPresentAdded += OnPresentRemoved;
            _stuffManager.onItemAdded += OnItemAdded;
            _homeDecor.onChangingDecor += OnChangingDecor;
            _homeDecor.onDoneChangingDecor += OnDoneChangingDecor;
        }

        protected void OnDisable()
        {
            HomeScene.onSelectPlayMode -= OnSelectPlayMode;
            HomeScene.onSelectChallengeArea -= OnSelectChallengeArea;
            HomeScene.onSelectEndlessArea -= OnSelectEndlessArea;
            _stuffManager.onPresentAdded -= OnPresentAdded;
            _stuffManager.onPresentAdded -= OnPresentRemoved;
            _stuffManager.onItemAdded -= OnItemAdded;
            _homeDecor.onChangingDecor -= OnChangingDecor;
            _homeDecor.onDoneChangingDecor -= OnDoneChangingDecor;
        }

        protected void Start()
        {
            var cats = GameManager.Instance.Cats.CatCollection.List;
            CatsButtonText.text = $"{ cats.Count(c => _catManager.IsBonded(c)) }";
            UpdatePresentsButton();
            PuzzleMakerButton.gameObject.SetActive(Debug.isDebugBuild);
        }

        private void UpdatePresentsButton()
        {
            int presentCount = _gameManager.Stuff.GetPresents().Count;
            if (presentCount > 0)
            {
                PresentCountText.text = presentCount.ToString();
                PresentCountRectTransform.gameObject.SetActive(true);
                PresentButton.interactable = true;
            }
            else
            {
                PresentCountRectTransform.gameObject.SetActive(false);
                PresentButton.interactable = false;
            }
        }

        private void OnSelectPlayMode()
        {
            OverlayScreenManager.EnqueueScreen(PlayModeSelectionOverlayScreen);
        }

        private void OnSelectChallengeArea()
        {
            OverlayScreenManager.EnqueueScreen(ChallengeAreaSelectionOverlayScreen, true);
        }

        private void OnSelectEndlessArea()
        {
            OverlayScreenManager.EnqueueScreen(EndlessAreaSelectionOverlayScreen, true);
        }

        private void OnPresentAdded(PresentInfo presentInfo)
        {
            UpdatePresentsButton();
        }

        private void OnPresentRemoved(PresentInfo presentInfo)
        {
            UpdatePresentsButton();
        }

        private void OnChangingDecor()
        {
            LeanTween.alphaCanvas(HomeMenuLayoutCanvasGroup, 0.0f, 0.0f).setEase(LeanTweenType.easeOutSine);
            foreach (Button button in DrawerButtons)
            {
                foreach(Transform child in button.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        private void OnDoneChangingDecor()
        {
            LeanTween.alphaCanvas(HomeMenuLayoutCanvasGroup, 1.0f, 0.0f).setEase(LeanTweenType.easeOutSine);
            foreach (Button button in DrawerButtons)
            {
                foreach (Transform child in button.transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
        }

        private void OnItemAdded(Item item, int amount)
        {
            ItemReceivedOverlayScreen.Item = item;
            ItemReceivedOverlayScreen.Amount = amount;
            OverlayScreenManager.EnqueueScreen(ItemReceivedOverlayScreen);
        }
    }
}