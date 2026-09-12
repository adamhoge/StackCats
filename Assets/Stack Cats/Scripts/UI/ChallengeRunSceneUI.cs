using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Tofuwu.StackCats.Models;

namespace Tofuwu.StackCats.UI
{
    public class ChallengeRunSceneUI : MonoBehaviour
    {
        public ChallengeRunScene ChallengeRunScene;
        public PuzzleLoader PuzzleLoader;
        public PuzzleCameraAdjuster PuzzleCameraAdjuster;
        public CanvasGroup ChallengeRunHeaderCanvasGroup;
        public Image FooterBackgroundImage;
        public List<Button> UIButtons;
        public List<Image> UIImages;
        public Button BackButton;
        public Button UndoButton;
        public RectTransform NumUndosRectTransform;
        public TextMeshProUGUI NumUndoesRemainingText;
        public OverlayScreenManager ChallengeOverlayScreenManager;
        public OverlayScreenManager MainOverlayScreenManager;
        public LuckyPuzzleOverlayScreen LuckyPuzzleOverlayScreen;
        public ChallengeStartOverlayScreen ChallengeStartOverlayScreen;
        public ChallengeProgressOverlayScreen ChallengeProgressOverlayScreen;
        public ChallengeEndedOverlayScreen ChallengeEndedOverlayScreen;
        public PuzzleSolvedOverlayScreen PuzzleSolvedOverlayScreen;
        public ChallengePuzzleFailedOverlayScreen ChallengePuzzleFailedOverlayScreen;
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
            ChallengeRunScene.onStateChanged += OnChallengeRunSceneStateChanged;
            ChallengeRunScene.onChallengePuzzleLoaded += OnChallengePuzzleLoaded;
            ChallengeRunScene.onChallengePuzzleUnloaded += OnChallengePuzzleUnloaded;
            ChallengeRunScene.onPuzzleSceneCompleting += OnPuzzleSceneCompleting;
            ChallengeRunScene.onPuzzleSceneCompleted += OnChallengePuzzleSceneCompleted;
            ChallengeRunScene.onMovesRemainingChanged += OnMovesRemainingChanged;
            ChallengeRunScene.onUndoUsed += OnUndoUsed;
            PuzzleLoader.onPuzzleBeginLoad += OnPuzzleBeginLoad;
            PuzzleLoader.onPuzzleLoaded += OnPuzzleLoaded;
            PuzzleLoader.onPuzzleUnloaded += OnPuzzleUnloaded;
            GameManagerUI.Instance.OverlayScreenManager.OnVisible.AddListener(OnOverlayVisible);
            GameManagerUI.Instance.OverlayScreenManager.OnHidden.AddListener(OnOverlayHidden);
        }

        protected void OnDisable()
        {
            ChallengeRunScene.onStateChanged -= OnChallengeRunSceneStateChanged;
            GameManagerUI.Instance.OverlayScreenManager.OnVisible.RemoveListener(OnOverlayVisible);
            GameManagerUI.Instance.OverlayScreenManager.OnHidden.RemoveListener(OnOverlayHidden);
        }

        protected void Start()
        {
            PuzzleTheme puzzleTheme = ChallengeRunScene.PuzzleArea.PuzzleTheme;
            FooterBackgroundImage.color = puzzleTheme.UIColor;
            foreach (Button button in UIButtons) button.image.color = puzzleTheme.UIColor;
            foreach (Image image in UIImages) image.color = puzzleTheme.UIColor;
        }

        protected void Update()
        {
            if (Screen.width != _screenSize.x || Screen.height != _screenSize.y)
            {
                _screenSize = new Vector2(Screen.width, Screen.height);
                Puzzle puzzle = ChallengeRunScene.PuzzleLoader.Puzzle;
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

        private void OnChallengeRunSceneStateChanged(ChallengeRunSceneState state)
        {
            switch (state)
            {
                case ChallengeRunSceneState.None:
                    break;
                case ChallengeRunSceneState.NotStarted:
                    ChallengeOverlayScreenManager.EnqueueScreen(ChallengeStartOverlayScreen);
                    break;
                case ChallengeRunSceneState.PlayingPuzzle:
                    LeanTween.alphaCanvas(ChallengeRunHeaderCanvasGroup, 0.0f, 0.5f).setEase(LeanTweenType.easeOutSine);
                    break;
                case ChallengeRunSceneState.ViewingProgress:
                    LeanTween.alphaCanvas(ChallengeRunHeaderCanvasGroup, 1.0f, 0.5f).setEase(LeanTweenType.easeOutSine);
                    ChallengeOverlayScreenManager.EnqueueScreen(ChallengeProgressOverlayScreen);
                    UpdateNumUndosRemaining(ChallengeRunScene.CurrentChallengeRun.CurrentPuzzle.UndosRemaining);
                    break;
                case ChallengeRunSceneState.RunCompleted:
                    LeanTween.alphaCanvas(ChallengeRunHeaderCanvasGroup, 1.0f, 0.5f).setEase(LeanTweenType.easeOutSine);
                    ChallengeOverlayScreenManager.EnqueueScreen(ChallengeEndedOverlayScreen);
                    break;
            }

            NumUndosRectTransform.gameObject.SetActive(state == ChallengeRunSceneState.PlayingPuzzle);
        }

        private void OnPuzzleBeginLoad(Puzzle puzzle, bool wasRestarted, bool isUndo)
        {
            BackButton.interactable = false;
            PuzzleCameraAdjuster.FitCameraAndUIToPuzzleAspect(puzzle);
            if (!isUndo && puzzle.IsSpecial) MainOverlayScreenManager.EnqueueScreen(LuckyPuzzleOverlayScreen);
            UpdateMovesRemaining(ChallengeRunScene.CurrentChallengeRun.CurrentPuzzle.MaxMoves - puzzle.NumMovesMade);
            UpdateNumUndosRemaining(ChallengeRunScene.CurrentChallengeRun.CurrentPuzzle.UndosRemaining);
        }

        private void OnPuzzleLoaded(Puzzle puzzle)
        {
            BackButton.interactable = true;
            UndoButton.interactable = ChallengeRunScene.CurrentChallengeRun.CurrentPuzzle.UndosRemaining > 0;
            puzzle.onCurrencyFound += OnCurrencyFound;
        }

        private void OnPuzzleUnloaded(Puzzle puzzle)
        {
            BackButton.interactable = true;
            PuzzleCameraAdjuster.FitCameraAndUIToMenu();

            MainOverlayScreenManager.OnHidden.RemoveListener(OnPuzzleCompletionInfoHidden);
        }

        private void OnChallengePuzzleLoaded(Puzzle puzzle, ChallengePuzzleModel challengePuzzle)
        {
            if (challengePuzzle.MaxMoves > 0)
            {
                MovesRemainingRectTransform.gameObject.SetActive(true);
                UpdateMovesRemaining(challengePuzzle.MaxMoves - challengePuzzle.MovesMade);
            }
        }

        private void OnChallengePuzzleUnloaded()
        {
            MovesRemainingRectTransform.gameObject.SetActive(false);
            UndoButton.interactable = false;
        }

        private void OnPuzzleSceneCompleting()
        {
            BackButton.interactable = false;
            UndoButton.interactable = false;
        }

        private void OnChallengePuzzleSceneCompleted(Puzzle puzzle, PuzzleCompletionType puzzleCompletionType)
        {
            MovesRemainingRectTransform.gameObject.SetActive(false);

            if (puzzleCompletionType == PuzzleCompletionType.PuzzleSolved)
            {
                List<Cat> catsSeen = ChallengeRunScene.CatsSeen;
                List<Cat> newCatsSeen = ChallengeRunScene.NewCatsSeen;

                foreach (Cat cat in newCatsSeen)
                {
                    NewCatOverlayScreen newCatOverlayScreen = Instantiate(NewCatOverlayScreenPrefab, MainOverlayScreenManager.transform);
                    newCatOverlayScreen.Cat = cat;
                    MainOverlayScreenManager.EnqueueScreen(newCatOverlayScreen);
                }

                if(newCatsSeen.Count > 0)
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
                MainOverlayScreenManager.EnqueueScreen(ChallengePuzzleFailedOverlayScreen);
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
            ChallengeRunScene.StopPlayingCurrentPuzzle();
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
            ChallengeRunScene.PuzzleLoader.SetInteractable(false);
        }

        private void OnOverlayHidden()
        {
            ChallengeRunScene.PuzzleLoader.SetInteractable(true);
        }
    }
}