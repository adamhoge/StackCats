using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats
{
    public class PriceTagUI : MonoBehaviour
    {
        public Currency Currency;
        public int Amount;
        public Sprite YarnSprite;
        public Sprite MoniesSprite;
        public Text CurrencyText;
        public Image CurrencyImage;

        protected void Start()
        {
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
        }
    }
}