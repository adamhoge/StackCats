using UnityEngine;

namespace Tofuwu.StackCats
{
    public class TerrainBlockDestructionEffect : MonoBehaviour
    {
        public GameObject TerrainBlock;
        public float BlockShrinkDuration = 0.15f;

        protected void Start()
        {
            LeanTween.moveLocalY(TerrainBlock, 0.5f, 0.25f).setEase(LeanTweenType.easeOutSine);
            LeanTween.scale(TerrainBlock, Vector3.zero, BlockShrinkDuration).setEase(LeanTweenType.easeOutSine);
            LeanTween.alpha(TerrainBlock, 0.0f, 0.25f).setEase(LeanTweenType.easeOutQuint);
        }
    }
}