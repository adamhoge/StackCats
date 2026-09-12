using UnityEngine;

namespace Tofuwu.StackCats
{
    public class RemovalBlockDestructionEffect : MonoBehaviour
    {
        public GameObject RemovalBlock;
        public float BlockShrinkDuration = 0.15f;

        protected void Start()
        {
            LeanTween.moveLocalY(RemovalBlock, 0.5f, 0.25f).setEase(LeanTweenType.easeOutSine);
            LeanTween.scale(RemovalBlock, Vector3.zero, BlockShrinkDuration).setEase(LeanTweenType.easeOutSine);
            LeanTween.alpha(RemovalBlock, 0.0f, 0.25f).setEase(LeanTweenType.easeOutQuint);
        }
    }
}