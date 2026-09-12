 using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    [RequireComponent(typeof(PuzzleCameraAdjuster))]
    public class StoryPuzzleSceneUI : MonoBehaviour
    {
        public StoryPuzzleScene StoryPuzzleScene;
        public Button BackButton;
        public Button UndoButton;
        public Image FooterBackgroundImage;
        public List<Image> UIImages = new List<Image>();
        public OverlayScreenManager OverlayScreenManager;
        public StoryPuzzleSolvedOverlayScreen StoryPuzzleSolvedOverlayScreen;
        public StoryPuzzleFailedOverlayScreen PuzzleFailedOverlayScreen;
        public CatSightingsOverlayScreen CatSightingsOverlayScreen;
        public NewCatOverlayScreen NewCatOverlayScreenPrefab;
        public NewCatsInCollectionOverlayScreen NewCatsInCollectionOverlayScreen;
        public ObjectPooler CurrencyFadeInfoObjectPooler;
        public Canvas ScreenCanvas;

        private PuzzleCameraAdjuster _puzzleCameraAdjuster;
        private Vector2 _screenSize;

        protected void Awake()
        {
            _puzzleCameraAdjuster = GetComponent<PuzzleCameraAdjuster>();
            UndoButton.interactable = false;
        }

        protected void Start()
        {
            PuzzleTheme puzzleTheme = StoryPuzzleScene.PuzzleArea.PuzzleTheme;
            FooterBackgroundImage.color = puzzleTheme.UIColor;
            foreach (Image image in UIImages) image.color = puzzleTheme.UIColor;
        }

        protected void OnEnable()
        {
            StoryPuzzleScene.onPuzzleSceneCompleting += OnPuzzleSceneCompleting;
            StoryPuzzleScene.onStoryPuzzleSceneCompleted += OnStoryPuzzleSceneCompleted;
            StoryPuzzleScene.PuzzleLoader.onPuzzleBeginLoad += OnPuzzleBeginLoad;
            StoryPuzzleScene.PuzzleLoader.onPuzzleLoaded += OnPuzzleLoaded;
            StoryPuzzleScene.PuzzleLoader.onPuzzleRestarted += OnPuzzleRestarted;
            GameManagerUI.Instance.OverlayScreenManager.OnVisible.AddListener(OnOverlayVisible);
            GameManagerUI.Instance.OverlayScreenManager.OnHidden.AddListener(OnOverlayHidden);
        }

        protected void OnDisable()
        {
            StoryPuzzleScene.onPuzzleSceneCompleting += OnPuzzleSceneCompleting;
            StoryPuzzleScene.onStoryPuzzleSceneCompleted -= OnStoryPuzzleSceneCompleted;
            StoryPuzzleScene.PuzzleLoader.onPuzzleBeginLoad -= OnPuzzleBeginLoad;
            StoryPuzzleScene.PuzzleLoader.onPuzzleLoaded -= OnPuzzleLoaded;
            StoryPuzzleScene.PuzzleLoader.onPuzzleRestarted -= OnPuzzleRestarted;
            GameManagerUI.Instance.OverlayScreenManager.OnVisible.AddListener(OnOverlayVisible);
            GameManagerUI.Instance.OverlayScreenManager.OnHidden.AddListener(OnOverlayHidden);
        }

        protected void Update()
        {
            if (Screen.width != _screenSize.x || Screen.height != _screenSize.y)
            {
                _screenSize = new Vector2(Screen.width, Screen.height);
                Puzzle puzzle = StoryPuzzleScene.PuzzleLoader.Puzzle;
                if (puzzle)
                {
                    _puzzleCameraAdjuster.FitCameraAndUIToPuzzleAspect(puzzle);
                }
                else
                {
                    _puzzleCameraAdjuster.FitCameraAndUIToMenu();
                }
            }
        }

        private void OnPuzzleBeginLoad(Puzzle puzzle, bool wasRestarted, bool isUndo)
        {
            _puzzleCameraAdjuster.FitCameraAndUIToPuzzleAspect(puzzle);

            BackButton.interactable = true;
        }

        private void OnPuzzleLoaded(Puzzle puzzle)
        {
            UndoButton.interactable = true;

            puzzle.onCurrencyFound += OnCurrencyFound;
        }

        private void OnPuzzleRestarted(Puzzle puzzle)
        {
            UndoButton.interactable = false;
        }

        private void OnCurrencyFound(CatBlock source, Currency currency, int amount)
        {
            CurrencyFadeInfo currencyFadeInfo = (CurrencyFadeInfo)CurrencyFadeInfoObjectPooler.BorrowInstance();
            currencyFadeInfo.Currency = currency;
            currencyFadeInfo.Amount = amount;
            currencyFadeInfo.transform.position = source.transform.position + Vector3.up * 0.5f;
            currencyFadeInfo.transform.SetParent(ScreenCanvas.transform, true);
            currencyFadeInfo.ParamStart();
        }

        private void OnPuzzleSceneCompleting()
        {
            BackButton.interactable = false;
            UndoButton.interactable = false;
        }

        private void OnStoryPuzzleSceneCompleted(StoryPuzzle storyPuzzle, PuzzleCompletionType completionType, int numMovesMade)
        {
            if (completionType == PuzzleCompletionType.PuzzleSolved)
            {
                List<Cat> catsSeen = StoryPuzzleScene.CatsSeen;
                List<Cat> newCatsSeen = StoryPuzzleScene.NewCatsSeen;
                foreach (Cat cat in newCatsSeen)
                {
                    NewCatOverlayScreen newCatOverlayScreen = Instantiate(NewCatOverlayScreenPrefab, OverlayScreenManager.transform);
                    newCatOverlayScreen.Cat = cat;
                    OverlayScreenManager.EnqueueScreen(newCatOverlayScreen);
                }

                if(newCatsSeen.Count > 0)
                {
                    NewCatsInCollectionOverlayScreen.NewCats = newCatsSeen;
                    OverlayScreenManager.EnqueueScreen(NewCatsInCollectionOverlayScreen);
                }

                if (catsSeen.Count > 0)
                {
                    CatSightingsOverlayScreen.CatsSighted = catsSeen;
                    OverlayScreenManager.EnqueueScreen(CatSightingsOverlayScreen);
                }

                StoryPuzzleSolvedOverlayScreen.StoryPuzzle = storyPuzzle;
                StoryPuzzleSolvedOverlayScreen.NumMovesMade = numMovesMade;
                StoryPuzzleSolvedOverlayScreen.CurrencyEarned = StoryPuzzleScene.CurrencyFound;
                OverlayScreenManager.EnqueueScreen(StoryPuzzleSolvedOverlayScreen);
            }
            else
            {
                OverlayScreenManager.EnqueueScreen(PuzzleFailedOverlayScreen);
            }
        }

        private void OnOverlayVisible()
        {
            StoryPuzzleScene.PuzzleLoader.SetInteractable(false);
        }

        private void OnOverlayHidden()
        {
            StoryPuzzleScene.PuzzleLoader.SetInteractable(true);
        }
    }
}