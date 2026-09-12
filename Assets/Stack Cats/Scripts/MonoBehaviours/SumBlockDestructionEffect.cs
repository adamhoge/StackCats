using UnityEngine;

namespace Tofuwu.StackCats
{
    public class SumBlockDestructionEffect : MonoBehaviour
    {
        public SumBlock SumBlock;
        public GameObject PlusBurst;
        public GameObject MinusBurst;
        public float BlockShrinkDuration = 0.15f;

        protected void Start()
        {
            LeanTween.moveLocalY(SumBlock.gameObject, 0.5f, 0.25f).setEase(LeanTweenType.easeOutSine);
            LeanTween.scale(SumBlock.gameObject, Vector3.zero, BlockShrinkDuration).setEase(LeanTweenType.easeOutSine);
            LeanTween.alpha(SumBlock.gameObject, 0.0f, 0.25f).setEase(LeanTweenType.easeOutQuint);

            PlusBurst.SetActive(SumBlock.SumValue > 0);
            MinusBurst.SetActive(SumBlock.SumValue < 0);
        }
    }
}