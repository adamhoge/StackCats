using UnityEngine;
using TMPro;

namespace Tofuwu.StackCats.UI
{
    public class FarmFlavoredPuzzleUI : MonoBehaviour
    {
        public PuzzleLoader PuzzleLoader;
        public RectTransform CatBlockRectTransform;
        public TextMeshProUGUI CatBlocksRemovedText;

        private FarmFlavoredPuzzle _puzzle;
        private int _totalCatBlocks;

        protected void Awake()
        {
            PuzzleLoader.onPuzzleBeginLoad += OnPuzzleBeginLoad;
            PuzzleLoader.onPuzzleUnloaded += OnPuzzleUnloaded;
            PuzzleLoader.onPuzzleRestarted += OnPuzzleRestarted;

            if (CatBlockRectTransform) CatBlockRectTransform.gameObject.SetActive(false);
        }

        private void OnPuzzleBeginLoad(Puzzle puzzle, bool wasRestarted, bool isUndo)
        {
            _totalCatBlocks = puzzle.GetBlockComponentCount<CatBlock>();

            if (puzzle.GetType() == typeof(FarmFlavoredPuzzle))
            {
                _puzzle = (FarmFlavoredPuzzle)puzzle;
                _puzzle.onBlockMoveResolved += OnBlockMoveResolved;

                if (CatBlocksRemovedText)
                {
                    UpdateCatBlocksRemovedText();
                }

                if (CatBlockRectTransform)
                {
                    CatBlockRectTransform.gameObject.SetActive(true);
                }
            }
        }

        private void OnPuzzleUnloaded(Puzzle puzzle)
        {
            if (_puzzle)
            {
                _puzzle.onBlockMoveResolved -= OnBlockMoveResolved;
                _puzzle = null;

                if (CatBlockRectTransform) CatBlockRectTransform.gameObject.SetActive(false);
            }
        }

        private void OnPuzzleRestarted(Puzzle puzzle)
        {
            if (puzzle.GetType() == typeof(FarmFlavoredPuzzle))
            {
                _puzzle = (FarmFlavoredPuzzle)puzzle;
                _puzzle.onBlockMoveResolved += OnBlockMoveResolved;

                UpdateCatBlocksRemovedText();

                if (CatBlockRectTransform)
                {
                    LeanTween.scale(CatBlockRectTransform.gameObject, Vector3.one * 1.2f, 0.25f).setEase(LeanTweenType.punch);
                }
            }
        }

        private void UpdateCatBlocksRemovedText()
        {
            CatBlocksRemovedText.text = _puzzle.GetBlockComponentCount<CatBlock>().ToString();
        }

        private void OnBlockMoveResolved(Puzzle puzzle, Stack source, Block block, Stack destination)
        {
            UpdateCatBlocksRemovedText();
        }
    }
}