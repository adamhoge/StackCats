using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class PlayModeSelectionOverlayScreen : OverlayScreen
    {
        public float TransitionXOffset = 256.0f;
        public TextMeshProUGUI StoryPuzzlesCompletedText;
        public TextMeshProUGUI StoryPuzzleStarsEarnedText;
        public LayoutGroup PlayModesLayoutGroup;
        public RectTransform PuzzleAreaChallengeCompletionRectTransform;
        public RectTransform PuzzleAreaEndlessCompletionRectTransform;
        public Button EndlessModeButton;
        public PuzzleAreaChallengeCompletionUI PuzzleAreaChallengeCompletionPrefab;
        public PuzzleAreaEndlessCompletionUI PuzzleAreaEndlessCompletionPrefab;

        private PuzzleManager _puzzleManager;
        private bool _isInitialized;
        private readonly List<CanvasGroup> _playModesCanvasGroups = new List<CanvasGroup>();

        protected void Awake()
        {
            _puzzleManager = GameManager.Instance.Puzzles;
        }

        protected void Start()
        {
            StoryPuzzlesCompletedText.text = _puzzleManager.GetNumCompletedStoryPuzzles() + "/" + _puzzleManager.GetNumStoryPuzzlesInAllAreas();
            StoryPuzzleStarsEarnedText.text = _puzzleManager.GetTotalNumStarsEarned() + "/" + _puzzleManager.GetNumStarsInAllAreas();

            foreach (PuzzleArea puzzleArea in _puzzleManager.PuzzleAreaCollection.List)
            {
                bool isAreaLocked = _puzzleManager.IsAreaLocked(puzzleArea);

                PuzzleAreaChallengeCompletionUI challengeCompletion = Instantiate(PuzzleAreaChallengeCompletionPrefab, PuzzleAreaChallengeCompletionRectTransform);
                challengeCompletion.IsLocked = _puzzleManager.IsChallengeModeLocked(puzzleArea);
                challengeCompletion.PuzzleArea = isAreaLocked ? null : puzzleArea;

                PuzzleAreaEndlessCompletionUI endlessCompletion = Instantiate(PuzzleAreaEndlessCompletionPrefab, PuzzleAreaEndlessCompletionRectTransform);
                endlessCompletion.IsLocked = _puzzleManager.IsEndlessModeLocked(puzzleArea);
                endlessCompletion.PuzzleArea = isAreaLocked ? null : puzzleArea;
            }
        }

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            if (!_isInitialized) Initialize();

            for (int i = 0; i < _playModesCanvasGroups.Count; i++)
            {
                float delay = TransitionInDuration / _playModesCanvasGroups.Count / 2 * i;
                if (delay < 0.0f) delay = 0.0f;
                _playModesCanvasGroups[i].transform.localPosition = new Vector2(-TransitionXOffset, 0);
                LeanTween.moveLocalX(_playModesCanvasGroups[i].gameObject, 0.0f, TransitionInDuration).setDelay(delay).setEase(LeanTweenType.easeOutQuart);
                LeanTween.alphaCanvas(_playModesCanvasGroups[i], 1.0f, TransitionInDuration / 2.0f).setDelay(delay).setEase(LeanTweenType.easeOutQuart);
            }
        }

        public override void OnTransitioningOut()
        {
            base.OnTransitioningOut();

            for (int i = _playModesCanvasGroups.Count - 1; i >= 0; i--)
            {
                float delay = TransitionOutDuration / _playModesCanvasGroups.Count / 2 * i;
                if (delay < 0.0f) delay = 0.0f;
                LeanTween.moveLocalX(_playModesCanvasGroups[i].gameObject, TransitionXOffset, TransitionOutDuration / 2.0f).setDelay(delay).setEase(LeanTweenType.easeOutQuart);
                LeanTween.alphaCanvas(_playModesCanvasGroups[i], 0.0f, TransitionOutDuration).setDelay(delay).setEase(LeanTweenType.easeOutQuart);
            }
        }

        private void Initialize()
        {
            if (PlayModesLayoutGroup)
            {
                foreach (RectTransform transform in PlayModesLayoutGroup.transform)
                {
                    _playModesCanvasGroups.Add(transform.GetComponent<CanvasGroup>());
                }

                PlayModesLayoutGroup.GetComponent<RectTransform>().WrapChildren();
            }

            foreach (CanvasGroup canvasGroup in _playModesCanvasGroups)
            {
                canvasGroup.transform.localPosition = new Vector2(TransitionXOffset, 0);
                canvasGroup.alpha = 0.0f;
            }

            _isInitialized = true;
        }

    }
}