using UnityEngine;

namespace Tofuwu.StackCats
{
    public class LockBlockDestructionEffect : MonoBehaviour
    {
        public GameObject LockBlock;
        public float BlockShrinkDuration = 0.15f;

        protected void Start()
        {
            LeanTween.moveLocalY(LockBlock, 0.5f, 0.25f).setEase(LeanTweenType.easeOutSine);
            LeanTween
                .scale(LockBlock, Vector3.zero, BlockShrinkDuration)
                .setEase(LeanTweenType.easeOutSine);
            LeanTween.alpha(LockBlock, 0.0f, 0.25f).setEase(LeanTweenType.easeOutQuint);
        }
    }
}
