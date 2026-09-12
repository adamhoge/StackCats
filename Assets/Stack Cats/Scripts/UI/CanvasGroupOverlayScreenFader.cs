using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupOverlayScreenFader : MonoBehaviour
    {
        public OverlayScreen OverlayScreen;

        private CanvasGroup _canvasGroup;

        protected void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        protected void OnEnable()
        {
            OverlayScreen.onTransitioningIn += OnTransitioningIn;
            OverlayScreen.onTransitioningOut += OnTransitioningOut;
        }

        protected void OnDisable()
        {
            OverlayScreen.onTransitioningIn -= OnTransitioningIn;
            OverlayScreen.onTransitioningOut -= OnTransitioningOut;
        }

        private void OnTransitioningIn()
        {
            _canvasGroup.alpha = 0.0f;
            LeanTween.cancel(_canvasGroup.gameObject);
            LeanTween.alphaCanvas(_canvasGroup, 1.0f, OverlayScreen.TransitionInDuration).setEase(OverlayScreen.TransitionInTween);
        }

        private void OnTransitioningOut()
        {
            LeanTween.cancel(_canvasGroup.gameObject);
            LeanTween.alphaCanvas(_canvasGroup, 0.0f, OverlayScreen.TransitionOutDuration).setEase(OverlayScreen.TransitionOutTween);
        }
    }
}