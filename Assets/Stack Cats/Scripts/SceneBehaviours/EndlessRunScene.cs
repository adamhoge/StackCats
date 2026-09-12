using Tofuwu.StackCats.Models;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public enum EndlessRunSceneState
    {
        None,
        NotStarted,
        PlayingPuzzle,
        ViewingProgress,
        RunCompleted
    }

    public delegate void StateChanged(EndlessRunSceneState state);
    public delegate void EndlessPuzzleLoaded(Puzzle puzzle, EndlessPuzzleModel endlessPuzzle);
    public delegate void EndlessPuzzleUnloaded();
    public delegate void EndlessPuzzleCompleted(Puzzle puzzle, PuzzleCompletionType completionType);
    public delegate void EndlessMovesRemainingChanged(int movesRemaining);
    public delegate void EndlessUndoUsed(int numUndosRemaining);

    [RequireComponent(typeof(PuzzleUndoer))]
    public class EndlessRunScene : PuzzleScene
    {
        public event StateChanged onStateChanged;
        public event EndlessPuzzleLoaded onEndlessPuzzleLoaded;
        public event EndlessPuzzleUnloaded onEndlessPuzzleUnloaded;
        public event EndlessPuzzleCompleted onEndlessPuzzleCompleted;
        public event EndlessMovesRemainingChanged onMovesRemainingChanged;
        public event UndoUsed onUndoUsed;

        /// <summary>
        /// The current endless run (if any).
        /// </summary>
        public EndlessRunModel CurrentEndlessRun { get { return _currentEndlessRun; } }

        private Puzzle _puzzle;
        private PuzzleUndoer _puzzleUndoer;
        private EndlessRunModel _currentEndlessRun;
        private EndlessRunSceneState _state;

        public void UndoMove()
        {
            if (_currentEndlessRun.CurrentPuzzle.UndosRemaining > 0)
            {
                if (_puzzleUndoer.UndoMove())
                {
                    int undosRemaining = _currentEndlessRun.CurrentPuzzle.UndosRemaining - 1;
                    _puzzleManager.UpdateCurrentEndlessPuzzle(_puzzleArea, PuzzleBuilder.GetPuzzleJsonData(_puzzle), _puzzle.NumMovesMade, CatsSeen, NewCatsSeen, undosRemaining);
                    _currentEndlessRun = _puzzleManager.GetCurrentEndlessRun(_puzzleArea);
                    if (onUndoUsed != null) { onUndoUsed(undosRemaining); }
                }
            }
        }

        public void Back()
        {
            if (_state == EndlessRunSceneState.PlayingPuzzle)
            {
                StopPlayingCurrentPuzzle();
            }
            else
            {
                StopPlaying();
            }
        }

        public void StopPlaying()
        {
            _gameManager.GoHome();
        }

        public void PlayCurrentPuzzle()
        {
            ChangeState(EndlessRunSceneState.PlayingPuzzle);
        }

        public void StopPlayingCurrentPuzzle()
        {
            if (_state != EndlessRunSceneState.PlayingPuzzle) return;

            if (_currentEndlessRun.CurrentPuzzle == null)
            {
                _puzzleManager.EndEndlessRun(_puzzleArea);
                ChangeState(EndlessRunSceneState.RunCompleted);
            }
            else
            {
                ChangeState(EndlessRunSceneState.ViewingProgress);
            }
        }

        public void StartEndlessRun()
        {
            if (_state == EndlessRunSceneState.None || _state == EndlessRunSceneState.NotStarted)
            {
                _currentEndlessRun = _puzzleManager.StartEndlessRun(_puzzleArea);
                ChangeState(EndlessRunSceneState.ViewingProgress);
            }
        }

        public void EndEndlessRun()
        {
            if (_state == EndlessRunSceneState.ViewingProgress)
            {
                _gameManager.ConfirmAction("Are you sure you want to end the current endless run?", ConfirmEndEndlessRun);
            }
            else if (_state == EndlessRunSceneState.RunCompleted)
            {
                ChangeState(EndlessRunSceneState.NotStarted);
            }
        }

        public void ConfirmEndEndlessRun()
        {
            _puzzleManager.EndEndlessRun(_puzzleArea);
            ChangeState(EndlessRunSceneState.RunCompleted);
        }

        public void Restart()
        {
            if (_state == EndlessRunSceneState.RunCompleted)
            {
                ChangeState(EndlessRunSceneState.NotStarted);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _puzzleArea = _puzzleManager.CurrentArea;
            _puzzleUndoer = GetComponent<PuzzleUndoer>();

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

            GetCurrentEndlessRun();
            if (_currentEndlessRun == null)
            {
                ChangeState(EndlessRunSceneState.NotStarted);
            }
            else if (_currentEndlessRun.IsComplete)
            {
                _puzzleManager.EndEndlessRun(_puzzleArea);
                ChangeState(EndlessRunSceneState.RunCompleted);
            }
            else
            {
                ChangeState(EndlessRunSceneState.ViewingProgress);
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

        private void ChangeState(EndlessRunSceneState state)
        {
            if (_state == state) return;

            switch (_state)
            {
                case EndlessRunSceneState.PlayingPuzzle:
                    _puzzleLoader.UnloadPuzzle();
                    HidePuzzleBackdrop();
                    if (onEndlessPuzzleUnloaded != null) onEndlessPuzzleUnloaded();
                    break;
                default:
                    break;
            }

            _state = state;

            switch (_state)
            {
                case EndlessRunSceneState.NotStarted:
                    break;
                case EndlessRunSceneState.PlayingPuzzle:
                    _puzzleLoader.LoadPuzzle(_currentEndlessRun.CurrentPuzzle.PuzzleCurrentStateJsonData, _puzzleArea);
                    _puzzleLoader.Puzzle.NumMovesMade = _currentEndlessRun.CurrentPuzzle.MovesMade;
                    ShowPuzzleBackdrop(_puzzleLoader.Puzzle);
                    CatsSeen = _currentEndlessRun.CurrentPuzzle.CatsSeen;
                    NewCatsSeen = _currentEndlessRun.CurrentPuzzle.NewCatsSeen;
                    if (onEndlessPuzzleLoaded != null) onEndlessPuzzleLoaded(_puzzleLoader.Puzzle, _currentEndlessRun.CurrentPuzzle);
                    break;
                case EndlessRunSceneState.ViewingProgress:
                    GetCurrentEndlessRun();
                    break;
                case EndlessRunSceneState.RunCompleted:
                    break;
                default:
                    break;
            }

            if (onStateChanged != null) onStateChanged(_state);
        }

        private void CompletePuzzle(Puzzle puzzle, PuzzleCompletionType completionType)
        {
            _puzzleManager.CompleteCurrentEndlessPuzzle(_puzzleArea, completionType, puzzle.NumMovesMade);
            GetCurrentEndlessRun();

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

            if (onEndlessPuzzleCompleted != null) onEndlessPuzzleCompleted(puzzle, completionType);
        }

        private void UpdateEndlessPuzzleData(Puzzle puzzle)
        {
            int undosRemaining = _currentEndlessRun.CurrentPuzzle.UndosRemaining;
            _puzzleManager.UpdateCurrentEndlessPuzzle(_puzzleArea, PuzzleBuilder.GetPuzzleJsonData(puzzle), puzzle.NumMovesMade, CatsSeen, NewCatsSeen, undosRemaining);
            EndlessPuzzleModel endlessPuzzle = _currentEndlessRun.CurrentPuzzle;

            int maxMoves = endlessPuzzle.MaxMoves;
            if (maxMoves > 0)
            {
                if (puzzle.NumMovesMade >= maxMoves)
                {
                    puzzle.CompletePuzzle(PuzzleCompletionType.PuzzleFailed);
                }

                if (onMovesRemainingChanged != null) onMovesRemainingChanged(endlessPuzzle.MaxMoves - puzzle.NumMovesMade);
            }
        }

        private void GetCurrentEndlessRun()
        {
            _currentEndlessRun = _puzzleManager.GetCurrentEndlessRun(_puzzleArea);
        }

        private void OnPuzzleBeginLoad(Puzzle puzzle, bool wasRestarted, bool isUndo)
        {
            if (isUndo)
            {
                UpdateEndlessPuzzleData(puzzle);
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
                UpdateEndlessPuzzleData(puzzle);
            }
        }
    }
}
