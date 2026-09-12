using System;
using UnityEngine;
using Tofuwu.StackCats.Models;
using TMPro;

namespace Tofuwu.StackCats
{
    public enum ChallengeRunSceneState
    {
        None,
        NotStarted,
        PlayingPuzzle,
        ViewingProgress,
        RunCompleted
    }

    public delegate void DifficultyChanged(ChallengeRunDifficulty difficulty);
    public delegate void ChallengePuzzleLoaded(Puzzle puzzle, ChallengePuzzleModel challengePuzzle);
    public delegate void ChallengePuzzleUnloaded();
    public delegate void ChallengePuzzleCompleted(Puzzle puzzle, PuzzleCompletionType completionType);
    public delegate void MovesRemainingChanged(int movesRemaining);
    public delegate void UndoUsed(int numUndosRemaining);
    public delegate void LuckPotionUsed();

    [RequireComponent(typeof(PuzzleUndoer))]
    public class ChallengeRunScene : PuzzleScene
    {
        public delegate void StateChanged(ChallengeRunSceneState state);

        public event StateChanged onStateChanged;
        public event DifficultyChanged onDifficultyChanged;
        public event ChallengePuzzleLoaded onChallengePuzzleLoaded;
        public event ChallengePuzzleUnloaded onChallengePuzzleUnloaded;
        public event ChallengePuzzleCompleted onChallengePuzzleCompleted;
        public event MovesRemainingChanged onMovesRemainingChanged;
        public event UndoUsed onUndoUsed;
        public event LuckPotionUsed onLuckPotionUsed;

        public Item LuckPotionItem;

        /// <summary>
        /// The current challenge run (if any).
        /// </summary>
        public ChallengeRunModel CurrentChallengeRun { get { return _currentChallengeRun; } }

        /// <summary>
        /// Whether or not the challenge run can be played.
        /// </summary>
        public bool CanPlay { get { return _currencyManager.GetCurrencyHeld(Currency.SilverPaw) >= _yarnCost; } }

        /// <summary>
        /// Indicates whether or not a luck potion can be used.
        /// </summary>
        public bool CanUseLuckPotion
        {
            get
            {
                return _currentChallengeRun != null &&
                    _currentChallengeRun.CurrentPuzzle != null &&
                    !_currentChallengeRun.CurrentPuzzle.WasLuckPotionUsed &&
                    !_currentChallengeRun.CurrentPuzzle.WasStarted && 
                    _stuffManager.HasItem(LuckPotionItem);
            }
        }

        /// <summary>
        /// The selected challenge run difficulty.
        /// </summary>
        public ChallengeRunDifficulty SelectedDifficulty { get { return _puzzleManager.DefaultChallengeRunDifficulty; } }

        /// <summary>
        /// The number of automatic special puzzles remaining.
        /// </summary>
        public int LuckPotionsRemaining { get { return _puzzleManager.SpecialPuzzlesRemaining; } }

        private ChallengeRunSceneState _state;
        private CurrencyManager _currencyManager;
        private StuffManager _stuffManager;
        private int _yarnCost;
        private Puzzle _puzzle;
        private PuzzleUndoer _puzzleUndoer;
        private ChallengeRunModel _currentChallengeRun;

        public void UndoMove()
        {
            if (_currentChallengeRun.CurrentPuzzle.UndosRemaining > 0)
            {
                if (_puzzleUndoer.UndoMove())
                {
                    int undosRemaining = _currentChallengeRun.CurrentPuzzle.UndosRemaining - 1;
                    _puzzleManager.UpdateCurrentChallengePuzzle(_puzzleArea, PuzzleBuilder.GetPuzzleJsonData(_puzzle), _puzzle.NumMovesMade, CatsSeen, NewCatsSeen, undosRemaining);
                    _currentChallengeRun = _puzzleManager.GetCurrentChallengeRun(_puzzleManager.CurrentArea);
                    if (onUndoUsed != null) { onUndoUsed(undosRemaining); }
                }
            }
        }

        public void StopPlaying()
        {
            _gameManager.GoHome();
        }

        public void Back()
        {
            if (_state == ChallengeRunSceneState.PlayingPuzzle)
            {
                StopPlayingCurrentPuzzle();
            }
            else
            {
                StopPlaying();
            }
        }

        public void StartChallengeRun()
        {
            if (_state == ChallengeRunSceneState.None || _state == ChallengeRunSceneState.NotStarted)
            {
                _currencyManager.ChangeCurrency(Currency.SilverPaw, -_yarnCost);
                _currentChallengeRun = _puzzleManager.StartChallengeRun(_puzzleArea, _puzzleManager.DefaultChallengeRunDifficulty);
                ChangeState(ChallengeRunSceneState.ViewingProgress);
            }
        }

        public void LowerDifficulty()
        {
            if (_puzzleManager.DefaultChallengeRunDifficulty == ChallengeRunDifficulty.VeryEasy) return;

            --_puzzleManager.DefaultChallengeRunDifficulty;
            if (onDifficultyChanged != null) onDifficultyChanged(_puzzleManager.DefaultChallengeRunDifficulty);
        }

        public void RaiseDifficulty()
        {
            if (_puzzleManager.DefaultChallengeRunDifficulty == ChallengeRunDifficulty.VeryHard) return;

            ++_puzzleManager.DefaultChallengeRunDifficulty;
            if (onDifficultyChanged != null) onDifficultyChanged(_puzzleManager.DefaultChallengeRunDifficulty);
        }

        public void UseLuckPotion()
        {
            if (CanUseLuckPotion)
            {
                string puzzleJsonData = _currentChallengeRun.CurrentPuzzle.PuzzleCurrentStateJsonData;
                Puzzle puzzle = PuzzleBuilder.BuildFromModel(puzzleJsonData, _puzzleArea, _catManager);
                puzzle.IsSpecial = true;
                _puzzleManager.AddStuffToCatBlocks(puzzle, _puzzleArea, PuzzleMode.Challenge, false);
                string updatedPuzzleJsonData = PuzzleBuilder.GetPuzzleJsonData(puzzle);
                Destroy(puzzle.gameObject);

                if (_puzzleManager.UseLuckPotion(_puzzleArea, updatedPuzzleJsonData))
                {
                    _currentChallengeRun = _puzzleManager.GetCurrentChallengeRun(_puzzleArea);
                    _stuffManager.RemoveItem(LuckPotionItem, 1);
                    if(onLuckPotionUsed != null) onLuckPotionUsed();
                }
            }
        }

        public void PlayCurrentPuzzle()
        {
            ChangeState(ChallengeRunSceneState.PlayingPuzzle);
        }

        public void StopPlayingCurrentPuzzle()
        {
            if (_state == ChallengeRunSceneState.PlayingPuzzle)
            {
                if (_currentChallengeRun.CurrentPuzzle == null)
                {
                    _puzzleManager.EndChallengeRun(_puzzleArea);
                    ChangeState(ChallengeRunSceneState.RunCompleted);
                }
                else
                {
                    ChangeState(ChallengeRunSceneState.ViewingProgress);
                }
            }
        }

        public void EndChallengeRun()
        {
            if (_state == ChallengeRunSceneState.ViewingProgress)
            {
                _gameManager.ConfirmAction("Are you sure you want to end the current challenge run?", ConfirmEndChallengeRun);
            }
            else if (_state == ChallengeRunSceneState.RunCompleted)
            {
                ChangeState(ChallengeRunSceneState.NotStarted);
            }
        }

        public void ConfirmEndChallengeRun()
        {
            _puzzleManager.EndChallengeRun(_puzzleArea);
            ChangeState(ChallengeRunSceneState.RunCompleted);
        }

        public void Restart()
        {
            if (_state == ChallengeRunSceneState.RunCompleted)
            {
                ChangeState(ChallengeRunSceneState.NotStarted);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _puzzleArea = _puzzleManager.CurrentArea;
            _currencyManager = _gameManager.Currency;
            _stuffManager = _gameManager.Stuff;
            _puzzleUndoer = GetComponent<PuzzleUndoer>();
            _yarnCost = _puzzleArea.ChallengeRunCost;

            BackgroundMusic = _puzzleArea.PuzzleTheme.BackgroundMusic;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _puzzleLoader.onPuzzleBeginLoad += OnPuzzleBeginLoad;
            _puzzleLoader.onPuzzleLoaded += OnPuzzleLoaded;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _puzzleLoader.onPuzzleBeginLoad -= OnPuzzleBeginLoad;
            _puzzleLoader.onPuzzleLoaded -= OnPuzzleLoaded;
        }

        protected override void Start()
        {
            base.Start();

            LoadPuzzleTheme(_puzzleArea.PuzzleTheme);

            _currentChallengeRun = _puzzleManager.GetCurrentChallengeRun(_puzzleArea);
            if (_currentChallengeRun == null)
            {
                ChangeState(ChallengeRunSceneState.NotStarted);
            }
            else if (_currentChallengeRun.CurrentPuzzle == null)
            {
                ChangeState(ChallengeRunSceneState.RunCompleted);
            }
            else
            {
                ChangeState(ChallengeRunSceneState.ViewingProgress);
            }
        }

        protected void LoadPuzzleTheme(PuzzleTheme puzzleTheme)
        {
            Camera.backgroundColor = puzzleTheme.BackgroundColor;
            if (puzzleTheme.PuzzleScenaryPrefab)
            {
                Instantiate(puzzleTheme.PuzzleScenaryPrefab, transform).name = "Puzzle Scenary";
            }

            if (puzzleTheme.TopBoundarySprite != null)
            {
                TopBoundarySprite.sprite = puzzleTheme.TopBoundarySprite;
                TopBoundarySprite.transform.SetParent(transform);
            }
            TopBoundarySprite.transform.localPosition = Vector2.up * 6.25f;
        }

        protected override void OnPuzzleCompleted(Puzzle puzzle, PuzzleCompletionType completionType)
        {
            base.OnPuzzleCompleted(puzzle, completionType);

            CompletePuzzle(puzzle, completionType);
        }

        private void ChangeState(ChallengeRunSceneState state)
        {
            if (_state == state) return;

            switch (_state)
            {
                case ChallengeRunSceneState.PlayingPuzzle:
                    _puzzleLoader.UnloadPuzzle();
                    HidePuzzleBackdrop();
                    if (onChallengePuzzleUnloaded != null) onChallengePuzzleUnloaded();
                    break;
                default:
                    break;
            }

            _state = state;

            switch (_state)
            {
                case ChallengeRunSceneState.PlayingPuzzle:
                    _puzzleLoader.LoadPuzzle(_currentChallengeRun.CurrentPuzzle.PuzzleCurrentStateJsonData, _puzzleArea);
                    ShowPuzzleBackdrop(_puzzleLoader.Puzzle);
                    _puzzleLoader.Puzzle.NumMovesMade = _currentChallengeRun.CurrentPuzzle.MovesMade;
                    CatsSeen = _currentChallengeRun.CurrentPuzzle.CatsSeen;
                    NewCatsSeen = _currentChallengeRun.CurrentPuzzle.NewCatsSeen;
                    if (onChallengePuzzleLoaded != null) onChallengePuzzleLoaded(_puzzleLoader.Puzzle, _currentChallengeRun.CurrentPuzzle);
                    break;
                case ChallengeRunSceneState.ViewingProgress:
                    _currentChallengeRun = _puzzleManager.GetCurrentChallengeRun(_puzzleArea);
                    break;
                default:
                    break;
            }

            if (onStateChanged != null) onStateChanged(_state);
        }

        private void CompletePuzzle(Puzzle puzzle, PuzzleCompletionType completionType)
        {
            _puzzleManager.CompleteCurrentChallengePuzzle(_puzzleArea, completionType, puzzle.NumMovesMade);
            _currentChallengeRun = _puzzleManager.GetCurrentChallengeRun(_puzzleArea);

            if (completionType == PuzzleCompletionType.PuzzleSolved)
            {
                foreach (Cat cat in CatsSeen)
                {
                    CatReward reward = _catManager.AddCatSighting(cat);
                    if (reward != null)
                    {
                        _gameManager.Stuff.AddPresent(cat, reward.Currency, reward.Items);
                    }
                }
            }

            foreach (PuzzleController controller in puzzle.GetComponents<PuzzleController>())
            {
                controller.enabled = false;
            }

            if (onChallengePuzzleCompleted != null) onChallengePuzzleCompleted(puzzle, completionType);
        }

        private void UpdateChallengePuzzleData(Puzzle puzzle)
        {
            int undosRemaining = _currentChallengeRun.CurrentPuzzle.UndosRemaining;
            _puzzleManager.UpdateCurrentChallengePuzzle(_puzzleArea, PuzzleBuilder.GetPuzzleJsonData(puzzle), puzzle.NumMovesMade, CatsSeen, NewCatsSeen, undosRemaining);
            ChallengePuzzleModel challengePuzzle = _currentChallengeRun.CurrentPuzzle;

            int maxMoves = challengePuzzle.MaxMoves;
            if (maxMoves > 0)
            {
                if (puzzle.NumMovesMade >= maxMoves)
                {
                    puzzle.CompletePuzzle(PuzzleCompletionType.PuzzleFailed);
                }

                if (onMovesRemainingChanged != null) onMovesRemainingChanged(challengePuzzle.MaxMoves - puzzle.NumMovesMade);
            }
        }

        private void OnPuzzleBeginLoad(Puzzle puzzle, bool wasRestarted, bool isUndo)
        {
            if (isUndo)
            {
                UpdateChallengePuzzleData(puzzle);
            }
        }

        private void OnPuzzleLoaded(Puzzle puzzle)
        {
            puzzle.onBlockMoveResolved += OnBlockMoveResolved;
            _puzzle = puzzle;
        }

        private void OnBlockMoveResolved(Puzzle puzzle, Stack source, Block block, Stack destination)
        {
            if (!puzzle.IsCompleted)
            {
                // TODO: This might cause issues relaying puzzle completion info.
                UpdateChallengePuzzleData(puzzle);
            }
        }
    }
}