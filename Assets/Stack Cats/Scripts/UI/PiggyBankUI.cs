using UnityEngine;
using Random = UnityEngine.Random;

namespace Tofuwu.StackCats.UI
{
    public class PiggyBankUI : MonoBehaviour
    {
        public GameObject PiggyBank;
        public CanvasGroup CurrencyInfoCanvasGroup;
        public AudioEvent ShakeAudioEvent;

        private CurrencyManager _currency;

        /// <summary>
        /// Shake the piggy bank.
        /// </summary>
        public void Shake()
        {
            LeanTween.cancel(CurrencyInfoCanvasGroup.gameObject);
            CurrencyInfoCanvasGroup.transform.localScale = Vector2.one;
            LeanTween.scale(CurrencyInfoCanvasGroup.gameObject, Vector2.one * 1.05f, 0.5f).setEase(LeanTweenType.punch);
            LeanTween.alphaCanvas(CurrencyInfoCanvasGroup, 1.0f, 0.05f);
            LeanTween.alphaCanvas(CurrencyInfoCanvasGroup, 0.0f, 0.05f).setDelay(3.0f);

            LeanTween.cancel(PiggyBank);
            PiggyBank.transform.localPosition = Vector2.zero;
            if (Random.value >= 0.5) LeanTween.moveLocalX(PiggyBank, 10.0f, 0.2f).setEase(LeanTweenType.easeShake);
            else LeanTween.moveLocalY(PiggyBank, 10.0f, 0.2f).setEase(LeanTweenType.easeShake);

            if (_currency.GetCurrencyHeld(Currency.SilverPaw) > 0 || _currency.GetCurrencyHeld(Currency.GoldPaw) > 0)
            {
                GameManager.Instance.Audio.PlaySoundEffect(ShakeAudioEvent);
            }
        }

        protected void Awake()
        {
            _currency = GameManager.Instance.Currency;
        }

        protected void Start()
        {
            CurrencyInfoCanvasGroup.alpha = 0.0f;
        }
    }
}