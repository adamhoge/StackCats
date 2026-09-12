using UnityEngine;

namespace Tofuwu.StackCats.UI
{
    public class FlashEffectUI : MonoBehaviour
    {
        public CanvasGroup FlashCanvasGroup;
        public float FlashFadeDuration = 1.0f;
        public LeanTweenType FlashFadeEasing = LeanTweenType.easeInCubic;
        public CanvasGroup FlashRippleCanvasGroup;
        public float RippleMaxScale = 2.0f;
        public float RippleDuration = 1.0f;
        public LeanTweenType RippleFadeEasing = LeanTweenType.easeInCubic;
        public LeanTweenType RippleEasing = LeanTweenType.easeOutQuint;
        public ParticleSystem FlashParticleSystem;

        public void Flash()
        {
            if (FlashCanvasGroup)
            {
                FlashCanvasGroup.alpha = 1.0f;
                LeanTween.alphaCanvas(FlashCanvasGroup, 0.0f, FlashFadeDuration)
                    .setEase(FlashFadeEasing);
            }

            if (FlashRippleCanvasGroup)
            {
                FlashRippleCanvasGroup.alpha = 1.0f;
                FlashRippleCanvasGroup.transform.localScale = Vector3.one;
                LeanTween.scale(FlashRippleCanvasGroup.gameObject, Vector3.one * RippleMaxScale, RippleDuration)
                    .setEase(RippleEasing);
                LeanTween.alphaCanvas(FlashRippleCanvasGroup, 0.0f, RippleDuration)
                    .setEase(RippleFadeEasing);
            }

            if (FlashParticleSystem)
            {
                FlashParticleSystem.Play();
            }
        }
    }
}