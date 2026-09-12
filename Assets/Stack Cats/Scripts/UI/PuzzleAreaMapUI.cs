using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats.UI
{
    public delegate void StarsEarned(int numStarsEarned);
    public delegate void ButtonStarsAdded(Vector3 worldSpaceLocation, int numStarsEarned);

    public class PuzzleAreaMapUI : MonoBehaviour
    {
        /// <summary>
        /// Invoked whenever stars are earned within puzzle area map.
        /// </summary>
        public event StarsEarned onStarsEarned;

        /// <summary>
        /// Invoked whenever the earned stars are "added" (displayed) in the UI button.
        /// </summary>
        public event ButtonStarsAdded onButtonStarsAdded;

        public PuzzleButton PuzzleButtonPrefab;

        private GameManager _gameManager;
        private PuzzleManager _puzzleManager;
        private PuzzleArea _puzzleArea;
        private PuzzleAreaMap _puzzleAreaMap;
        private bool _isActive = true;
        private readonly Dictionary<StoryPuzzle, PuzzleButton> _puzzleButtons = new Dictionary<StoryPuzzle, PuzzleButton>();

        public void Initialize(PuzzleArea puzzleArea, PuzzleAreaMap puzzleAreaMap)
        {
            _puzzleArea = puzzleArea;
            _puzzleAreaMap = puzzleAreaMap;

            if (_puzzleManager.IsAreaLocked(_puzzleArea)) return;

            transform.position = _puzzleAreaMap.transform.position;

            for (int i = 0; i < _puzzleAreaMap.ProgressionPuzzleRoute.Count; i++)
            {
                ProgressionPuzzleRouteItem routeItem = _puzzleAreaMap.ProgressionPuzzleRoute[i];
                StoryPuzzle puzzle = routeItem.NormalPuzzle;
                PuzzleButton puzzleSelectButton = Instantiate(PuzzleButtonPrefab, transform);
                puzzleSelectButton.transform.localPosition = routeItem.Coordinates;
                puzzleSelectButton.IsSelected = puzzle == _puzzleManager.CurrentStoryPuzzle;
                puzzleSelectButton.IsLocked = _puzzleManager.IsStoryPuzzleLocked(puzzle);
                if (!puzzleSelectButton.IsLocked) puzzleSelectButton.NumStarsEarned = _puzzleManager.GetStoryPuzzleNumStarsEarned(puzzle);
                puzzleSelectButton.IsComplete = _puzzleManager.IsStoryPuzzleCompleted(puzzle);
                puzzleSelectButton.Button.image.color = _puzzleArea.PuzzleTheme.BackgroundColor;
                puzzleSelectButton.PuzzleLabelText.text = (i + 1).ToString();
                if (puzzle)
                {
                    puzzleSelectButton.Button.onClick.AddListener(delegate { SelectPuzzle(puzzle); });
                    puzzleSelectButton.UnlocksArea = i == _puzzleAreaMap.RouteIndexToNextArea && !_puzzleManager.IsStoryPuzzleCompleted(puzzle);
                    puzzleSelectButton.HasCutScene = _gameManager.Events.StoryFirstCompletionEvents.Exists(e => e.StoryPuzzle == puzzle && e.EventResponses.Exists(er => er.PlayCutScene != null));
                    puzzleSelectButton.onStarsAdded += OnStarsAdded;
                    _puzzleButtons.Add(puzzle, puzzleSelectButton);
                }
            }
        }

        protected void Awake()
        {
            _gameManager = GameManager.Instance;
            _puzzleManager = _gameManager.Puzzles;
        }

        protected void Start()
        {
            if (_puzzleManager.IsAreaLocked(_puzzleArea)) return;

            if (_puzzleArea == _puzzleManager.CurrentArea)
            {
                _puzzleManager.OneOffStoryPuzzleCompleted.ConsumeAll(OnPuzzleCompleted);
                _puzzleManager.OneOffProgressionPuzzleUnlocked.ConsumeAll(OnPuzzleUnlocked);
            }
        }

        public void SetActive(bool isActive)
        {
            if (isActive == _isActive) return;

            _isActive = isActive;

            if (_puzzleManager.IsAreaLocked(_puzzleArea)) return;

            foreach (KeyValuePair<StoryPuzzle, PuzzleButton> puzzleButton in _puzzleButtons)
            {
                puzzleButton.Value.IsEnabled = isActive;
            }
        }

        public void RevealArea()
        {
            for (int i = 0; i < _puzzleAreaMap.ProgressionPuzzleRoute.Count; i++)
            {
                ProgressionPuzzleRouteItem routeItem = _puzzleAreaMap.ProgressionPuzzleRoute[i];
                if (routeItem.NormalPuzzle)
                {
                    PuzzleButton button = _puzzleButtons[routeItem.NormalPuzzle];
                    CanvasGroup buttonCanvas = button.GetComponent<CanvasGroup>();
                    buttonCanvas.alpha = 0.0f;
                    LeanTween.alphaCanvas(buttonCanvas, 1.0f, 0.25f).setDelay(3.0f + 0.1f * i);
                    LeanTween.scale(button.gameObject, Vector2.one * 1.5f, 0.5f).setEase(LeanTweenType.punch).setDelay(3.0f + 0.1f * i);
                }
            }
        }

        private void SelectPuzzle(StoryPuzzle puzzle)
        {
            _puzzleAreaMap.PlayStoryPuzzle(puzzle);
        }

        private void OnChallengeModeButtonClicked()
        {
            _gameManager.PlayChallengeMode(_puzzleArea);
        }

        private void OnStarsAdded(PuzzleButton sender, int numStars)
        {
            Vector3 buttonPosition = sender.transform.position;
            if (onButtonStarsAdded != null) onButtonStarsAdded(buttonPosition, numStars);
        }

        private void OnPuzzleUnlocked(StoryPuzzle puzzle)
        {
            if (!_puzzleButtons.ContainsKey(puzzle)) return;

            PuzzleButton puzzleButton = _puzzleButtons[puzzle];
            puzzleButton.Unlock();
        }

        private void OnPuzzleCompleted(PuzzleManager.StoryPuzzleCompletionEvent puzzleCompletionEvent)
        {
            if (!_puzzleButtons.ContainsKey(puzzleCompletionEvent.Puzzle)) return;

            PuzzleButton puzzleButton = _puzzleButtons[puzzleCompletionEvent.Puzzle];
            if (!puzzleCompletionEvent.WasPreviouslyCompleted) puzzleButton.Complete();
            puzzleButton.ChangeStars(puzzleCompletionEvent.PreviousNumStarsEarned, puzzleCompletionEvent.CurrentNumStarsEarned, puzzleCompletionEvent.WasPreviouslyCompleted);
            if (onStarsEarned != null) onStarsEarned(puzzleCompletionEvent.CurrentNumStarsEarned - puzzleCompletionEvent.PreviousNumStarsEarned);
        }
    }
}