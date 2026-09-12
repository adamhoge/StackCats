using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace Tofuwu.StackCats.UI
{
    public class ConfirmationOverlayScreen : OverlayScreen
    {
        public UnityAction ConfirmationAction;
        public CanvasGroup ConfirmationCanvasGroup;
        public Button ConfirmButton;
        public Button CancelButton;
        public Button ScreenButton;
        public TextMeshProUGUI MessageText;
        public TextMeshProUGUI ConfirmationText;
        public TextMeshProUGUI CancelText;

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            ConfirmButton.onClick.RemoveAllListeners();
            ConfirmButton.onClick.AddListener(Dismiss);
            if (ConfirmationAction != null)
            {
                ConfirmButton.onClick.AddListener(ConfirmationAction);
            }

            ConfirmationCanvasGroup.alpha = 0.0f;
            ConfirmationCanvasGroup.transform.localScale = Vector2.one;
            //ConfirmationCanvasGroup.transform.localPosition = Vector2.down * 25.0f;
            LeanTween.cancel(ConfirmationCanvasGroup.gameObject);
            LeanTween.alphaCanvas(ConfirmationCanvasGroup, 1.0f, TransitionInDuration).setEase(TransitionInTween);
            LeanTween.scale(ConfirmationCanvasGroup.gameObject, Vector2.one * 1.05f, 0.5f)
                .setEase(LeanTweenType.punch);
            //LeanTween.moveLocalY(ConfirmationCanvasGroup.gameObject, 0.0f, TransitionInDuration).setEase(TransitionInTween);
        }

        public override void OnTransitioningOut()
        {
            base.OnTransitioningOut();

            ConfirmationCanvasGroup.alpha = 1.0f;
            ConfirmationCanvasGroup.transform.localScale = Vector2.one;
            //ConfirmationCanvasGroup.transform.localPosition = Vector2.zero;
            LeanTween.cancel(ConfirmationCanvasGroup.gameObject);
            LeanTween.alphaCanvas(ConfirmationCanvasGroup, 0.0f, TransitionOutDuration).setEase(TransitionInTween);
            LeanTween.scale(ConfirmationCanvasGroup.gameObject, Vector2.one * 0.9f, TransitionOutDuration).setEase(TransitionOutTween);
            //LeanTween.moveLocalY(ConfirmationCanvasGroup.gameObject, -25.0f, TransitionInDuration).setEase(TransitionInTween);
        }

        protected void Awake()
        {
            CancelButton.onClick.AddListener(Dismiss);
            ScreenButton.onClick.AddListener(Dismiss);
        }
    }
}