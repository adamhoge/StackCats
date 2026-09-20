using UnityEngine;

namespace Tofuwu.StackCats
{
    public class KeyBlockDestructionEffect : MonoBehaviour
    {
        public GameObject KeyBlock;
        public float BlockShrinkDuration = 0.15f;

        protected void Start()
        {
            LeanTween.moveLocalY(KeyBlock, 0.5f, 0.25f).setEase(LeanTweenType.easeOutSine);
            LeanTween
                .scale(KeyBlock, Vector3.zero, BlockShrinkDuration)
                .setEase(LeanTweenType.easeOutSine);
            LeanTween.alpha(KeyBlock, 0.0f, 0.25f).setEase(LeanTweenType.easeOutQuint);
        }
    }
}
