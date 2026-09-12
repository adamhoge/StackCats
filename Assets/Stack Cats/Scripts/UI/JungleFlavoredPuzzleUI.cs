using UnityEngine;
using TMPro;

namespace Tofuwu.StackCats.UI
{
    public class JungleFlavoredPuzzleUI : MonoBehaviour
    {
        public PuzzleLoader PuzzleLoader;
        public RectTransform JigsawRectTransform;
        public TextMeshProUGUI JigsawText;

        private JungleFlavoredPuzzle _puzzle;

        protected void Awake()
        {
            PuzzleLoader.onPuzzleBeginLoad += OnPuzzleBeginLoad;
            PuzzleLoader.onPuzzleUnloaded += OnPuzzleUnloaded;
            PuzzleLoader.onPuzzleRestarted += OnPuzzleRestarted;

            if (JigsawRectTransform) JigsawRectTransform.gameObject.SetActive(false);
        }

        private void OnPuzzleBeginLoad(Puzzle puzzle, bool wasRestarted, bool isUndo)
        {
            if (puzzle.GetType() == typeof(JungleFlavoredPuzzle))
            {
                _puzzle = (JungleFlavoredPuzzle)puzzle;
                _puzzle.onJigsawBlockMoved += OnJigsawBlockMoved;

                if (JigsawText)
                {
                    UpdateJigsawsCompletedText();
                }

                if (JigsawRectTransform)
                {
                    JigsawRectTransform.gameObject.SetActive(true);
                }
            }
        }

        private void OnPuzzleUnloaded(Puzzle puzzle)
        {
            if (_puzzle)
            {
                _puzzle.onJigsawBlockMoved -= OnJigsawBlockMoved;
                _puzzle = null;

                if (JigsawRectTransform) JigsawRectTransform.gameObject.SetActive(false);
            }
        }

        private void OnPuzzleRestarted(Puzzle puzzle)
        {
            if (puzzle.GetType() == typeof(JungleFlavoredPuzzle))
            {
                _puzzle = (JungleFlavoredPuzzle)puzzle;
                _puzzle.onJigsawBlockMoved += OnJigsawBlockMoved;

                UpdateJigsawsCompletedText();

                if (JigsawRectTransform)
                {
                    LeanTween.scale(JigsawRectTransform.gameObject, Vector3.one * 1.2f, 0.25f).setEase(LeanTweenType.punch);
                }
            }
        }

        private void UpdateJigsawsCompletedText()
        {
            JigsawText.text = _puzzle.NumCompletedJigsawPuzzles + "/" + _puzzle.NumJigsawPuzzles;
        }

        private void OnJigsawBlockMoved(JigsawBlock jigsawBlock)
        {
            UpdateJigsawsCompletedText();
        }
    }
}