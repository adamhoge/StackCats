using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Tofuwu.StackCats.UI
{
    public class PuzzleSolvedOverlayScreen : OverlayScreen
    {
        public CanvasGroup BannerCanvasGroup;
        public LayoutGroup PuzzleLettersLayoutGroup;
        public LayoutGroup CompleteLettersLayoutGroup;
        public CanvasGroup MainBackground;
        public CanvasGroup BannerBackground;
        public CanvasGroup BannerPortrait;

        private bool _isInitialized;
        private List<CanvasGroup> _puzzleCompleteLetterCanvasGroups = new List<CanvasGroup>();

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            MainBackground.transform.localScale = Vector3.zero;
            LeanTween.scale(MainBackground.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutQuint).setDelay(0.25f);

            BannerBackground.transform.localScale = Vector3.zero;
            LeanTween.scale(BannerBackground.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutQuint).setDelay(0.25f);

            BannerPortrait.transform.localPosition = Vector2.down * 256.0f;
            BannerPortrait.alpha = 0.0f;
            LeanTween.alphaCanvas(BannerPortrait, 1.0f, 0.5f).setEase(LeanTweenType.easeOutQuint).setDelay(0.75f);
            LeanTween.moveLocalY(BannerPortrait.gameObject, 8.0f, 0.75f).setEase(LeanTweenType.easeOutQuad).setDelay(0.4f);
            LeanTween.moveLocalY(BannerPortrait.gameObject, 0.0f, 0.75f).setEase(LeanTweenType.easeInOutQuad).setDelay(1.15f);

            if (!_isInitialized) Initialize();

            BannerCanvasGroup.alpha = 0.0f;
            LeanTween.alphaCanvas(BannerCanvasGroup, 1.0f, TransitionInDuration).setEase(TransitionInTween);
            for (int i = 0; i < _puzzleCompleteLetterCanvasGroups.Count; i++)
            {
                CanvasGroup puzzleLetterCanvasGroup = _puzzleCompleteLetterCanvasGroups[i];
                LeanTween.cancel(puzzleLetterCanvasGroup.gameObject);
                puzzleLetterCanvasGroup.transform.Translate(Vector2.down * 0.5f);
                puzzleLetterCanvasGroup.alpha = 0.0f;
                LeanTween.moveLocalY(puzzleLetterCanvasGroup.gameObject, 0.0f, 2.0f).setEase(LeanTweenType.easeOutElastic).setDelay(0.5f + 0.04f * i);
                LeanTween.alphaCanvas(puzzleLetterCanvasGroup, 1.0f, 0.25f).setEase(LeanTweenType.easeOutQuint).setDelay(0.5f + 0.04f * i);
            }
        }

        public override void OnTransitioningOut()
        {
            base.OnTransitioningOut();

            //LeanTween.scale(BannerBackground.gameObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeOutQuint);
            LeanTween.alphaCanvas(BannerPortrait, 0.0f, 0.1f);
            LeanTween.alphaCanvas(BannerCanvasGroup, 0.0f, 0.25f);
        }

        protected void OnEnable()
        {
            BannerCanvasGroup.alpha = 0.0f;
            BannerCanvasGroup.transform.localScale = Vector3.one;
            BannerCanvasGroup.transform.rotation = Quaternion.identity;
        }

        protected new void Update()
        {
            base.Update();

            BannerBackground.transform.Rotate(Vector3.forward, Time.deltaTime * -15.0f);
        }

        private void Initialize()
        {
            foreach (RectTransform transform in PuzzleLettersLayoutGroup.transform) { _puzzleCompleteLetterCanvasGroups.Add(transform.GetComponent<CanvasGroup>()); }
            foreach (RectTransform transform in CompleteLettersLayoutGroup.transform) { _puzzleCompleteLetterCanvasGroups.Add(transform.GetComponent<CanvasGroup>()); }

            PuzzleLettersLayoutGroup.GetComponent<RectTransform>().WrapChildren();
            CompleteLettersLayoutGroup.GetComponent<RectTransform>().WrapChildren();

            _isInitialized = true;
        }
    }
}