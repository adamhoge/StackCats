using UnityEngine;

namespace Tofuwu.StackCats
{
    public class CatBlockDestructionEffect : MonoBehaviour
    {
        public GameObject CatBlock;
        public SpriteRenderer BlockSpriteRenderer;
        public SpriteRenderer CatPawSpriteRenderer;
        public float BlockShrinkDuration = 0.15f;

        protected void Start()
        {
            LeanTween.moveLocalY(CatBlock, 0.5f, 0.25f).setEase(LeanTweenType.easeOutSine);
            LeanTween.scale(CatBlock, Vector3.zero, BlockShrinkDuration).setEase(LeanTweenType.easeOutSine);
            LeanTween.alpha(CatBlock, 0.0f, 0.25f).setEase(LeanTweenType.easeOutQuint);
        }
    }
}