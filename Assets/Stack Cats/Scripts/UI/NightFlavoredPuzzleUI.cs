using UnityEngine;
using TMPro;

namespace Tofuwu.StackCats.UI
{
    public class NightFlavoredPuzzleUI : MonoBehaviour
    {
        public PuzzleLoader PuzzleLoader;
        public RectTransform CurtainTimerRectTransform;
        public TextMeshProUGUI CurtainTimerText;

        private NightFlavoredPuzzle _puzzle;

        protected void Awake()
        {
            PuzzleLoader.onPuzzleBeginLoad += OnPuzzleBeginLoad;
            PuzzleLoader.onPuzzleUnloaded += OnPuzzleUnloaded;
            PuzzleLoader.onPuzzleRestarted += OnPuzzleRestarted;

            if (CurtainTimerRectTransform) CurtainTimerRectTransform.gameObject.SetActive(false);
        }

        private void UpdateCurtainTimer()
        {
            if (CurtainTimerText) CurtainTimerText.text = _puzzle.CurtainTurnsRemaining.ToString();// + "/" + Puzzle.CurtainDropMoveCount;
        }

        private void OnPuzzleBeginLoad(Puzzle puzzle, bool wasRestarted, bool isUndo)
        {
            if (puzzle.GetType() == typeof(NightFlavoredPuzzle))
            {
                _puzzle = (NightFlavoredPuzzle)puzzle;
                _puzzle.onCurtainTurnsChanged += OnCurtainTurnsChanged;

                UpdateCurtainTimer();

                if (CurtainTimerRectTransform)
                {
                    CurtainTimerRectTransform.gameObject.SetActive(true);
                }
            }
        }

        private void OnPuzzleUnloaded(Puzzle puzzle)
        {
            if (_puzzle)
            {
                _puzzle.onCurtainTurnsChanged -= OnCurtainTurnsChanged;
                _puzzle = null;

                if (CurtainTimerRectTransform) CurtainTimerRectTransform.gameObject.SetActive(false);
            }
        }

        private void OnPuzzleRestarted(Puzzle puzzle)
        {
            if (puzzle.GetType() == typeof(NightFlavoredPuzzle))
            {
                if (_puzzle) _puzzle.onCurtainTurnsChanged -= OnCurtainTurnsChanged;

                _puzzle = (NightFlavoredPuzzle)puzzle;
                _puzzle.onCurtainTurnsChanged += OnCurtainTurnsChanged;

                if (CurtainTimerText) CurtainTimerText.text = _puzzle.CurtainDropInterval.ToString();// + "/" + Puzzle.CurtainDropMoveCount;

                if (CurtainTimerRectTransform)
                {
                    LeanTween.scale(CurtainTimerRectTransform.gameObject, Vector3.one * 1.2f, 0.25f).setEase(LeanTweenType.punch);
                }
            }
        }

        private void OnCurtainTurnsChanged(int curtainTurnsRemaining)
        {
            UpdateCurtainTimer();
        }
    }
}