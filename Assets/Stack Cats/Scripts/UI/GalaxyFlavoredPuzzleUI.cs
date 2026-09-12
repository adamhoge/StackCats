using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class GalaxyFlavoredPuzzleUI : MonoBehaviour
    {
        public PuzzleLoader PuzzleLoader;
        public RectTransform RaiseStacksTimerRectTransform;
        public TextMeshProUGUI RaiseStacksTimerText;
        public Button RaiseStacksButton;

        private GalaxyFlavoredPuzzle _puzzle;

        public bool CurtainTimerText { get; private set; }

        protected void Awake()
        {
            PuzzleLoader.onPuzzleBeginLoad += OnPuzzleBeginLoad;
            PuzzleLoader.onPuzzleLoaded += OnPuzzleLoaded;
            PuzzleLoader.onPuzzleUnloaded += OnPuzzleUnloaded;
            PuzzleLoader.onPuzzleRestarted += OnPuzzleRestarted;

            if (RaiseStacksTimerRectTransform) RaiseStacksTimerRectTransform.gameObject.SetActive(false);
        }

        private void UpdateRaiseStacksTimer()
        {
            if (RaiseStacksTimerText) RaiseStacksTimerText.text = _puzzle.RaiseStacksMovesRemaining.ToString();// + "/" + Puzzle.CurtainDropMoveCount;
        }

        private void OnPuzzleBeginLoad(Puzzle puzzle, bool wasRestarted, bool isUndo)
        {
            RaiseStacksButton.interactable = false;

            if (puzzle.GetType() == typeof(GalaxyFlavoredPuzzle))
            {
                _puzzle = (GalaxyFlavoredPuzzle)puzzle;
                _puzzle.onRaiseStacksMovesChanged += OnRaiseStacksMovesChanged;
                RaiseStacksButton.onClick.AddListener(OnRaiseStacksButtonClicked);

                UpdateRaiseStacksTimer();

                if (RaiseStacksTimerRectTransform)
                {
                    RaiseStacksTimerRectTransform.gameObject.SetActive(true);
                    Vector3 originalPosition = RaiseStacksTimerRectTransform.gameObject.transform.position;
                }
            }
        }

        private void OnPuzzleLoaded(Puzzle puzzle)
        {
            RaiseStacksButton.interactable = true;
        }

        private void OnPuzzleUnloaded(Puzzle puzzle)
        {
            if (_puzzle)
            {
                _puzzle.onRaiseStacksMovesChanged -= OnRaiseStacksMovesChanged;
                RaiseStacksButton.onClick.RemoveListener(OnRaiseStacksButtonClicked);
                _puzzle = null;

                if (RaiseStacksTimerRectTransform) RaiseStacksTimerRectTransform.gameObject.SetActive(false);
            }
        }

        private void OnPuzzleRestarted(Puzzle puzzle)
        {
            RaiseStacksButton.interactable = false;

            if (puzzle.GetType() == typeof(GalaxyFlavoredPuzzle))
            {
                if (_puzzle) _puzzle.onRaiseStacksMovesChanged -= OnRaiseStacksMovesChanged;

                _puzzle = (GalaxyFlavoredPuzzle)puzzle;
                _puzzle.onRaiseStacksMovesChanged += OnRaiseStacksMovesChanged;

                if (CurtainTimerText) RaiseStacksTimerText.text = _puzzle.RaiseStacksInterval.ToString();// + "/" + Puzzle.CurtainDropMoveCount;

                if (RaiseStacksTimerRectTransform)
                {
                    LeanTween.scale(RaiseStacksTimerRectTransform.gameObject, Vector3.one * 1.2f, 0.25f).setEase(LeanTweenType.punch);
                }
            }
        }

        private void OnRaiseStacksMovesChanged(int curtainTurnsRemaining)
        {
            UpdateRaiseStacksTimer();
        }

        private void OnRaiseStacksButtonClicked()
        {
            _puzzle.RaiseStacksImmediate();
        }
    }
}