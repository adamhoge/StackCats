using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class ItemReceivedOverlayScreen : OverlayScreen
    {
        public Item Item;
        public int Amount;
        public Image ItemIcon;
        public TextMeshProUGUI ItemLabel;
        public CanvasGroup PanelCanvasGroup;

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            ItemIcon.sprite = Item.Icon;
            ItemIcon.SetMaterialForItem(Item.ColorShift); ;
            ItemLabel.text = Item.Name;
            if (Amount > 1) ItemLabel.text += " x" + Amount;

            PanelCanvasGroup.alpha = 0.0f;
            PanelCanvasGroup.transform.localScale = Vector2.one * 0.9f;
            LeanTween.cancel(PanelCanvasGroup.gameObject);
            LeanTween.alphaCanvas(PanelCanvasGroup, 1.0f, TransitionInDuration).setEase(TransitionInTween);
            LeanTween.scale(PanelCanvasGroup.gameObject, Vector2.one, TransitionInDuration).setEase(TransitionInTween);
        }

        public override void OnTransitioningOut()
        {
            base.OnTransitioningOut();

            PanelCanvasGroup.alpha = 1.0f;
            PanelCanvasGroup.transform.localScale = Vector2.one;
            LeanTween.cancel(PanelCanvasGroup.gameObject);
            LeanTween.alphaCanvas(PanelCanvasGroup, 0.0f, TransitionOutDuration).setEase(TransitionInTween);
            LeanTween.scale(PanelCanvasGroup.gameObject, Vector2.one * 0.9f, TransitionOutDuration).setEase(TransitionOutTween);
        }
    }
}