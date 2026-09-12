using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Tofuwu.StackCats.UI
{
    public class LuckyPuzzleOverlayScreen : OverlayScreen
    {
        public LayoutGroup LuckyLettersLayoutGroup;
        public LayoutGroup PuzzleLettersLayoutGroup;
        public CanvasGroup MainCanvasGroup;

        private bool _isInitialized;
        private List<CanvasGroup> _luckyPuzzleLetterCanvasGroups = new List<CanvasGroup>();

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            if (!_isInitialized) Initialize();

            LeanTween.cancel(MainCanvasGroup.gameObject);
            MainCanvasGroup.transform.localScale = Vector3.one;
            MainCanvasGroup.alpha = 0.0f;
            LeanTween.alphaCanvas(MainCanvasGroup, 1.0f, TransitionInDuration).setEase(TransitionInTween);

            for (int i = 0; i < _luckyPuzzleLetterCanvasGroups.Count; i++)
            {
                CanvasGroup puzzleLetterCanvasGroup = _luckyPuzzleLetterCanvasGroups[i];
                LeanTween.cancel(puzzleLetterCanvasGroup.gameObject);
                puzzleLetterCanvasGroup.transform.Translate(Vector2.down * 1.0f);
                puzzleLetterCanvasGroup.alpha = 0.0f;
                LeanTween.moveLocalY(puzzleLetterCanvasGroup.gameObject, 0.0f, 2.0f).setEase(LeanTweenType.easeOutElastic).setDelay(0.5f + 0.04f * i);
                LeanTween.alphaCanvas(puzzleLetterCanvasGroup, 1.0f, 0.25f).setEase(LeanTweenType.easeOutQuint).setDelay(0.5f + 0.04f * i);
            }
        }

        public override void OnTransitioningOut()
        {
            base.OnTransitioningOut();

            LeanTween.cancel(MainCanvasGroup.gameObject);
            LeanTween.alphaCanvas(MainCanvasGroup, 0.0f, TransitionOutDuration).setEase(TransitionOutTween);
            LeanTween.scale(MainCanvasGroup.gameObject, Vector3.one * 1.25f, TransitionOutDuration).setEase(TransitionOutTween);
        }

        protected override void Update()
        {
            base.Update();

            if ((_isActive && _activeTimeElapsed > 1.25f) || Input.GetMouseButtonDown(0))
            {
                Dismiss();
            }
        }

        private void Initialize()
        {
            foreach (RectTransform transform in LuckyLettersLayoutGroup.transform) { _luckyPuzzleLetterCanvasGroups.Add(transform.GetComponent<CanvasGroup>()); }
            foreach (RectTransform transform in PuzzleLettersLayoutGroup.transform) { _luckyPuzzleLetterCanvasGroups.Add(transform.GetComponent<CanvasGroup>()); }

            LuckyLettersLayoutGroup.GetComponent<RectTransform>().WrapChildren();
            PuzzleLettersLayoutGroup.GetComponent<RectTransform>().WrapChildren();

            _isInitialized = true;
        }
    }
}