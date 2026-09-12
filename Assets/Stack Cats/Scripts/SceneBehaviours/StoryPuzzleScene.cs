using UnityEngine;

namespace Tofuwu.StackCats
{
    public delegate void StoryPuzzleSceneCompleted(StoryPuzzle storyPuzzle, PuzzleCompletionType completionType, int numMovesMade);

    [RequireComponent(typeof(PuzzleUndoer))]
    public class StoryPuzzleScene : PuzzleScene
    {
        public event StoryPuzzleSceneCompleted onStoryPuzzleSceneCompleted;

        private PuzzleUndoer _puzzleUndoer;

        /// <summary>
        /// Undo the last move made.
        /// </summary>
        public void UndoMove()
        {
            _puzzleUndoer.UndoMove();
        }

        /// <summary>
        /// Stop playing the puzzle.
        /// </summary>
        public void StopPlaying()
        {
            Puzzle puzzle = _puzzleLoader.Puzzle;
            if (!puzzle || puzzle.IsCompleted || puzzle.NumMovesMade == 0)
            {
                _gameManager.GoToMap();
            }
            else
            {
                _gameManager.ConfirmAction("Are you sure you want to quit the current puzzle?", _gameManager.GoToMap);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _puzzleUndoer = GetComponent<PuzzleUndoer>();

            _puzzleArea = _puzzleManager.CurrentArea;
            BackgroundMusic = _puzzleArea.PuzzleTheme.BackgroundMusic;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _puzzleLoader.onPuzzleBeginLoad += OnPuzzleBeginLoad;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _puzzleLoader.onPuzzleBeginLoad -= OnPuzzleBeginLoad;
        }

        protected override void Start()
        {
            base.Start();

            LoadPuzzleTheme(_puzzleArea.PuzzleTheme);

            PuzzleLoader.LoadPuzzle(_puzzleManager.CurrentStoryPuzzle.JsonData, _puzzleArea);
            ShowPuzzleBackdrop(_puzzleLoader.Puzzle);
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

        protected override void OnPuzzleCompleted(Puzzle puzzle, PuzzleCompletionType puzzleCompletionType)
        {
            base.OnPuzzleCompleted(puzzle, puzzleCompletionType);

            CompletePuzzle(puzzle, puzzleCompletionType);

            foreach (PuzzleController controller in puzzle.GetComponents<PuzzleController>())
            {
                controller.enabled = false;
            }
        }

        protected override void OnPuzzleSceneCompleted(Puzzle puzzle, PuzzleCompletionType puzzleCompletionType)
        {
            base.OnPuzzleSceneCompleted(puzzle, puzzleCompletionType);

            if (onStoryPuzzleSceneCompleted != null) onStoryPuzzleSceneCompleted(_puzzleManager.CurrentStoryPuzzle, puzzleCompletionType, puzzle.NumMovesMade);
        }

        public void RestartPuzzle()
        {
            if (_puzzleLoader.Puzzle.IsCompleted)
            {
                _puzzleLoader.ReloadPuzzle();
            }
            else
            {
                _gameManager.ConfirmAction("Restart the current puzzle?", _puzzleLoader.ReloadPuzzle);
            }
        }

        private void CompletePuzzle(Puzzle puzzle, PuzzleCompletionType completionType)
        {
            if (completionType == PuzzleCompletionType.PuzzleSolved)
            {
                _puzzleManager.CompleteStoryPuzzle(_puzzleManager.CurrentStoryPuzzle, puzzle.TimeElapsed, puzzle.NumMovesMade);
                _puzzleManager.UnlockStoryPuzzle(_puzzleManager.GetNextStoryPuzzle(_puzzleManager.CurrentStoryPuzzle));

                foreach (Cat cat in CatsSeen)
                {
                    CatReward reward = _catManager.AddCatSighting(cat);
                    if (reward != null)
                    {
                        _gameManager.Stuff.AddPresent(cat, reward.Currency, reward.Items);
                    }
                }
            }
        }

        private void OnPuzzleBeginLoad(Puzzle puzzle, bool wasRestarted, bool isUndo)
        {
            if (!isUndo)
            {
                _puzzleManager.AddStuffToCatBlocks(puzzle, _puzzleArea, PuzzleMode.Story, wasRestarted);
            }
        }
    }
}