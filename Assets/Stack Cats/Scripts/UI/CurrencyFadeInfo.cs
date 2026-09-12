using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tofuwu.StackCats.UI
{
    public class CurrencyFadeInfo : PoolObject
    {
        public Currency Currency;
        public int Amount;
        public Sprite YarnSprite;
        public Sprite MoniesSprite;
        public float MoveDistance = 16.0f;
        public Color GainColor = Color.green;
        public Color LossColor = Color.red;
        public CanvasGroup CurrencyCanvas;
        public TextMeshProUGUI SignText;
        public TextMeshProUGUI CurrencyText;
        public Image CurrencyImage;

        public override void ParamStart()
        {
            if (Amount == 0 || !YarnSprite || !MoniesSprite || !CurrencyText || !CurrencyImage)
            {
                ReturnToPool();
                return;
            }

            CurrencyText.text = Currency.ToCurrencyString(Amount);

            switch (Currency)
            {
                case Currency.SilverPaw:
                    CurrencyImage.sprite = YarnSprite;
                    break;
                case Currency.GoldPaw:
                    CurrencyImage.sprite = MoniesSprite;
                    break;
            }

            LeanTween.scale(gameObject, Vector3.one * 1.25f, 0.5f).setEase(LeanTweenType.punch);
            float direction = Amount > 0 ? 1 : -1;
            LeanTween.moveLocalY(gameObject, transform.localPosition.y + MoveDistance * direction, 1.25f);
            CurrencyCanvas.alpha = 1.0f;
            LeanTween.alphaCanvas(CurrencyCanvas, 0.0f, 0.25f).setDelay(1.0f).setOnComplete(ReturnToPool);
        }

        private void ReturnToPool()
        {
            LeanTween.cancel(gameObject);
            Parent.ReturnInstance(this);
        }
    }
}