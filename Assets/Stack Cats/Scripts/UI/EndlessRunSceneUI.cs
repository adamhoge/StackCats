using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Tofuwu.StackCats.Models;

namespace Tofuwu.StackCats.UI
{
    public class EndlessRunSceneUI : MonoBehaviour
    {
        public EndlessRunScene EndlessRunScene;
        public PuzzleLoader PuzzleLoader;
        public PuzzleCameraAdjuster PuzzleCameraAdjuster;
        public CanvasGroup EndlessRunHeaderCanvasGroup;
        public Image FooterBackgroundImage;
        public List<Button> UIButtons;
        public List<Image> UIImages;
        public Button BackButton;
        public Button UndoButton;
        public RectTransform NumUndosRectTransform;
        public TextMeshProUGUI NumUndoesRemainingText;
        public OverlayScreenManager EndlessOverlayScreenManager;
        public OverlayScreenManager MainOverlayScreenManager;
        public LuckyPuzzleOverlayScreen LuckyPuzzleOverlayScreen;
        public EndlessStartOverlayScreen EndlessStartOverlayScreen;
        public EndlessProgressOverlayScreen EndlessProgressOverlayScreen;
        public EndlessEndedOverlayScreen EndlessEndedOverlayScreen;
        public PuzzleSolvedOverlayScreen PuzzleSolvedOverlayScreen;
        public EndlessPuzzleFailedOverlayScreen EndlessPuzzleFailedOverlayScreen;
        public NewCatOverlayScreen NewCatOverlayScreenPrefab;
        public NewCatsInCollectionOverlayScreen NewCatsInCollectionOverlayScreen;
        public CatSightingsOverlayScreen CatSightingsOverlayScreen;
        public ObjectPooler CurrencyFadeInfoObjectPooler;
        public Canvas ScreenCanvas;
        public RectTransform MovesRemainingRectTransform;
        public TextMeshProUGUI MovesRemainingText;
        public Color MovesRemainingCriticalColor;

        private PuzzleCameraAdjuster _puzzleCameraAdjuster;
        private Vector2 _screenSize;

        protected void Awake()
        {
            _puzzleCameraAdjuster = GetComponent<PuzzleCameraAdjuster>();
        }

        protected void OnEnable()
        {
            EndlessRunScene.onStateChanged += OnEndlessRunSceneStateChanged;
            EndlessRunScene.onEndlessPuzzleLoaded += OnEndlessPuzzleLoaded;
            EndlessRunScene.onEndlessPuzzleUnloaded += OnEndlessPuzzleUnloaded;
            EndlessRunScene.onPuzzleSceneCompleting += OnPuzzleSceneCompleting;
            EndlessRunScene.onPuzzleSceneCompleted += OnEndlessPuzzleSceneCompleted;
            EndlessRunScene.onMovesRemainingChanged += OnMovesRemainingChanged;
            EndlessRunScene.onUndoUsed += OnUndoUsed;
            PuzzleLoader.onPuzzleBeginLoad += OnPuzzleBeginLoad;
            PuzzleLoader.onPuzzleLoaded += OnPuzzleLoaded;
            PuzzleLoader.onPuzzleUnloaded += OnPuzzleUnloaded;
            GameManagerUI.Instance.OverlayScreenManager.OnVisible.AddListener(OnOverlayVisible);
            GameManagerUI.Instance.OverlayScreenManager.OnHidden.AddListener(OnOverlayHidden);
        }

        protected void OnDisable()
        {
            EndlessRunScene.onStateChanged -= OnEndlessRunSceneStateChanged;
            EndlessRunScene.onEndlessPuzzleLoaded -= OnEndlessPuzzleLoaded;
            EndlessRunScene.onEndlessPuzzleUnloaded -= OnEndlessPuzzleUnloaded;
            EndlessRunScene.onPuzzleSceneCompleting -= OnPuzzleSceneCompleting;
            EndlessRunScene.onPuzzleSceneCompleted -= OnEndlessPuzzleSceneCompleted;
            EndlessRunScene.onMovesRemainingChanged -= OnMovesRemainingChanged;
            EndlessRunScene.onUndoUsed -= OnUndoUsed;
            PuzzleLoader.onPuzzleBeginLoad -= OnPuzzleBeginLoad;
            PuzzleLoader.onPuzzleLoaded -= OnPuzzleLoaded;
            PuzzleLoader.onPuzzleUnloaded -= OnPuzzleUnloaded;
            GameManagerUI.Instance.OverlayScreenManager.OnVisible.RemoveListener(OnOverlayVisible);
            GameManagerUI.Instance.OverlayScreenManager.OnHidden.RemoveListener(OnOverlayHidden);
        }

        protected void Update()
        {
            if (Screen.width != _screenSize.x || Screen.height != _screenSize.y)
            {
                _screenSize = new Vector2(Screen.width, Screen.height);
                Puzzle puzzle = EndlessRunScene.PuzzleLoader.Puzzle;
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

        private void UpdateNumUndosRemaining(int numUndosRemaining)
        {
            NumUndoesRemainingText.text = numUndosRemaining.ToString();

            LeanTween.cancel(NumUndoesRemainingText.gameObject);
            NumUndoesRemainingText.transform.localScale = Vector2.one;
            LeanTween.scale(NumUndoesRemainingText.gameObject, Vector2.one * 1.25f, 0.5f).setEase(LeanTweenType.punch);

            if (numUndosRemaining == 0) UndoButton.interactable = false;
        }

        private void UpdateMovesRemaining(int movesRemaining)
        {
            MovesRemainingText.text = movesRemaining.ToString();
            MovesRemainingText.color = movesRemaining > 3 ? Color.white : MovesRemainingCriticalColor;

            LeanTween.cancel(MovesRemainingText.gameObject);
            MovesRemainingText.transform.localScale = Vector2.one;
            LeanTween.scale(MovesRemainingText.gameObject, Vector2.one * 1.25f, 0.5f).setEase(LeanTweenType.punch);
        }

        private void OnEndlessRunSceneStateChanged(EndlessRunSceneState state)
        {
            switch (state)
            {
                case EndlessRunSceneState.None:
                    break;
                case EndlessRunSceneState.NotStarted:
                    UpdatePuzzleAreaUI();
                    EndlessOverlayScreenManager.EnqueueScreen(EndlessStartOverlayScreen);
                    break;
                case EndlessRunSceneState.PlayingPuzzle:
                    LeanTween.alphaCanvas(EndlessRunHeaderCanvasGroup, 0.0f, 0.5f).setEase(LeanTweenType.easeOutSine);
                    break;
                case EndlessRunSceneState.ViewingProgress:
                    LeanTween.alphaCanvas(EndlessRunHeaderCanvasGroup, 1.0f, 0.5f).setEase(LeanTweenType.easeOutSine);
                    UpdatePuzzleAreaUI();
                    EndlessOverlayScreenManager.EnqueueScreen(EndlessProgressOverlayScreen);
                    UpdateNumUndosRemaining(EndlessRunScene.CurrentEndlessRun.CurrentPuzzle.UndosRemaining);
                    break;
                case EndlessRunSceneState.RunCompleted:
                    UpdatePuzzleAreaUI();
                    EndlessOverlayScreenManager.EnqueueScreen(EndlessEndedOverlayScreen);
                    break;
            }

            NumUndosRectTransform.gameObject.SetActive(state == EndlessRunSceneState.PlayingPuzzle);
        }

        private void UpdatePuzzleAreaUI()
        {
            PuzzleArea puzzleArea = EndlessRunScene.PuzzleArea;
            Color uiColor = puzzleArea.PuzzleTheme.UIColor;
            FooterBackgroundImage.color = uiColor;
            foreach (Button button in UIButtons) button.image.color = uiColor;
            foreach (Image image in UIImages) image.color = uiColor;
        }

        private void OnPuzzleBeginLoad(Puzzle puzzle, bool wasRestarted, bool isUndo)
        {
            BackButton.interactable = false;
            PuzzleCameraAdjuster.FitCameraAndUIToPuzzleAspect(puzzle);
            if (!isUndo && puzzle.IsSpecial) MainOverlayScreenManager.EnqueueScreen(LuckyPuzzleOverlayScreen);
            UpdateMovesRemaining(EndlessRunScene.CurrentEndlessRun.CurrentPuzzle.MaxMoves - puzzle.NumMovesMade);
            UpdateNumUndosRemaining(EndlessRunScene.CurrentEndlessRun.CurrentPuzzle.UndosRemaining);
        }

        private void OnPuzzleLoaded(Puzzle puzzle)
        {
            BackButton.interactable = true;
            UndoButton.interactable = EndlessRunScene.CurrentEndlessRun.CurrentPuzzle.UndosRemaining > 0;
            puzzle.onCurrencyFound += OnCurrencyFound;
        }

        private void OnPuzzleUnloaded(Puzzle puzzle)
        {
            BackButton.interactable = true;
            PuzzleCameraAdjuster.FitCameraAndUIToMenu();

            MainOverlayScreenManager.OnHidden.RemoveListener(OnPuzzleCompletionInfoHidden);
        }

        private void OnEndlessPuzzleLoaded(Puzzle puzzle, EndlessPuzzleModel endlessPuzzle)
        {
            if (endlessPuzzle.MaxMoves > 0)
            {
                MovesRemainingRectTransform.gameObject.SetActive(true);
                UpdateMovesRemaining(endlessPuzzle.MaxMoves - endlessPuzzle.MovesMade);
            }
        }

        private void OnEndlessPuzzleUnloaded()
        {
            MovesRemainingRectTransform.gameObject.SetActive(false);
            UndoButton.interactable = false;
        }

        private void OnPuzzleSceneCompleting()
        {
            BackButton.interactable = false;
            UndoButton.interactable = false;
        }

        private void OnEndlessPuzzleSceneCompleted(Puzzle puzzle, PuzzleCompletionType puzzleCompletionType)
        {
            MovesRemainingRectTransform.gameObject.SetActive(false);

            if (puzzleCompletionType == PuzzleCompletionType.PuzzleSolved)
            {
                List<Cat> catsSeen = EndlessRunScene.CatsSeen;
                List<Cat> newCatsSeen = EndlessRunScene.NewCatsSeen;

                foreach (Cat cat in newCatsSeen)
                {
                    NewCatOverlayScreen newCatOverlayScreen = Instantiate(NewCatOverlayScreenPrefab, MainOverlayScreenManager.transform);
                    newCatOverlayScreen.Cat = cat;
                    MainOverlayScreenManager.EnqueueScreen(newCatOverlayScreen);
                }

                if (newCatsSeen.Count > 0)
                {
                    NewCatsInCollectionOverlayScreen.NewCats = newCatsSeen;
                    MainOverlayScreenManager.EnqueueScreen(NewCatsInCollectionOverlayScreen);
                }

                if (catsSeen.Count > 0)
                {
                    CatSightingsOverlayScreen.CatsSighted = catsSeen;
                    MainOverlayScreenManager.EnqueueScreen(CatSightingsOverlayScreen);
                }

                MainOverlayScreenManager.EnqueueScreen(PuzzleSolvedOverlayScreen);
            }
            else
            {
                MainOverlayScreenManager.EnqueueScreen(EndlessPuzzleFailedOverlayScreen);
            }

            MainOverlayScreenManager.OnHidden.AddListener(OnPuzzleCompletionInfoHidden);
        }

        private void OnMovesRemainingChanged(int movesRemaining)
        {
            UpdateMovesRemaining(movesRemaining);
        }

        private void OnUndoUsed(int numUndosRemaining)
        {
            UpdateNumUndosRemaining(numUndosRemaining);
        }

        private void OnPuzzleCompletionInfoHidden()
        {
            EndlessRunScene.StopPlayingCurrentPuzzle();
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

        private void OnOverlayVisible()
        {
            EndlessRunScene.PuzzleLoader.SetInteractable(false);
        }

        private void OnOverlayHidden()
        {
            EndlessRunScene.PuzzleLoader.SetInteractable(true);
        }
    }
}