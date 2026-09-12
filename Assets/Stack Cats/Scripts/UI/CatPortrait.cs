using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats
{
    public class CatPortrait : MonoBehaviour
    {
        public Cat Cat;
        public bool WasSeen;
        public bool IsBonded;
        public Image CatPortraitImage;
        public Sprite NotSeenSprite;
        public Image CatPortraitMaskImage;
        public CanvasGroup OverlayColorCanvasGroup;
        public Color NotBondedColor = new(0, 0, 0, 0.1f);
        public Color NotSeenColor = new(0, 0, 0, 0.05f);
        public float RevealFlashDuration = 1.0f;

        public void SetWasSeen()
        {
            CatPortraitImage.sprite = Cat.Portrait;
            CatPortraitImage.color = IsBonded ? Color.white : NotBondedColor;
            OverlayColorCanvasGroup.alpha = 1.0f;
            LeanTween.alphaCanvas(OverlayColorCanvasGroup, 0.0f, RevealFlashDuration).setEase(LeanTweenType.easeInSine);
        }

        public void SkipAnimation()
        {
            LeanTween.cancel(gameObject);
            OverlayColorCanvasGroup.alpha = 0.0f;
        }

        protected void Start()
        {
            CatPortraitImage.sprite = WasSeen ? Cat.Portrait : NotSeenSprite;
            CatPortraitMaskImage.sprite = Cat.Portrait;
            if (!WasSeen)
            {
                CatPortraitImage.color = NotSeenColor;
            }
            else if (!IsBonded)
            {
                CatPortraitImage.color = NotBondedColor;
            }
        }
    }
}