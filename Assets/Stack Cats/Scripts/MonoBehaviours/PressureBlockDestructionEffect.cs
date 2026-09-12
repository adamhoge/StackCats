using UnityEngine;
using TMPro;

namespace Tofuwu.StackCats
{
    public class PressureBlockDestructionEffect : MonoBehaviour
    {
        public GameObject PressureBlock;
        public TextMeshPro BreakingPointText;
        public TextMeshPro BlocksAboveText;
        public float BlockShrinkDuration = 0.15f;

        protected void Start()
        {
            LeanTween.moveLocalY(PressureBlock, 0.5f, 0.25f).setEase(LeanTweenType.easeOutSine);
            LeanTween.scale(PressureBlock, Vector3.zero, BlockShrinkDuration).setEase(LeanTweenType.easeOutSine);
            LeanTween.alpha(PressureBlock, 0.0f, 0.25f).setEase(LeanTweenType.easeOutQuint);
        }
    }
}