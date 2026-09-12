using UnityEngine;
using TMPro;

namespace Tofuwu.StackCats.UI
{
    public class MoveCounterUI : MonoBehaviour
    {
        public PuzzleLoader PuzzleLoader;
        public RectTransform ContentRectTransform;
        public TextMeshProUGUI MovesMadeText;
        public TextMeshProUGUI BestMovesMadeText;
        public TextMeshProUGUI ThreeStarMovesText;

        private PuzzleManager _puzzles;
        private Puzzle _puzzle;
        private int? _puzzleBestMoveScore;

        protected void Awake()
        {
            _puzzles = GameManager.Instance.Puzzles;

            PuzzleLoader.onPuzzleBeginLoad += OnPuzzleBeginLoad;
            PuzzleLoader.onPuzzleUnloaded += OnPuzzleUnloaded;

            if (ContentRectTransform) ContentRectTransform.gameObject.SetActive(false);
        }

        private void OnPuzzleBeginLoad(Puzzle puzzle, bool wasRestarted, bool isUndo)
        {
            if (_puzzles.CurrentMode == PuzzleMode.Story)
            {
                _puzzle = puzzle;
                _puzzle.onBlockMoved += OnBlockMoved;
                _puzzle.onPuzzleCompleted += OnPuzzleCompleted;
                _puzzleBestMoveScore = _puzzles.GetStoryPuzzleBestMoveScore(_puzzles.CurrentStoryPuzzle);

                if (ContentRectTransform)
                {
                    ContentRectTransform.gameObject.SetActive(true);
                }

                if (MovesMadeText)
                {
                    MovesMadeText.text = _puzzle.NumMovesMade.ToString();
                }

                if (BestMovesMadeText)
                {
                    BestMovesMadeText.text = _puzzleBestMoveScore != null ? _puzzleBestMoveScore.ToString() : "-";
                }

                if (ThreeStarMovesText && _puzzles.CurrentStoryPuzzle.StarMoveRequirements.Count == 3)
                {
                    ThreeStarMovesText.text = _puzzles.CurrentStoryPuzzle.StarMoveRequirements[2].ToString();
                }
            }
        }

        private void OnPuzzleUnloaded(Puzzle puzzle)
        {
            if (_puzzle)
            {
                _puzzle.onBlockMoved -= OnBlockMoved;
                _puzzle.onPuzzleCompleted -= OnPuzzleCompleted;
                _puzzle = null;

                if (ContentRectTransform) ContentRectTransform.gameObject.SetActive(false);
            }
        }

        private void OnBlockMoved(Puzzle puzzle, Stack source, Block block, Stack destination)
        {
            MovesMadeText.text = _puzzle.NumMovesMade.ToString();

            LeanTween.cancel(MovesMadeText.gameObject);
            MovesMadeText.transform.localScale = Vector2.one;
            LeanTween.scale(MovesMadeText.gameObject, Vector3.one * 1.25f, 0.5f).setEase(LeanTweenType.punch);
        }

        private void OnPuzzleCompleted(Puzzle puzzle, PuzzleCompletionType puzzleCompletionType)
        {
            // Store best on load, compare here.
            if (_puzzleBestMoveScore == null || puzzle.NumMovesMade < _puzzleBestMoveScore)
            {
                LeanTween.cancel(BestMovesMadeText.gameObject);
                BestMovesMadeText.text = puzzle.NumMovesMade.ToString();
                LeanTween.scale(BestMovesMadeText.gameObject, Vector3.one * 1.25f, 0.5f).setEase(LeanTweenType.punch);
            }
        }
    }
}