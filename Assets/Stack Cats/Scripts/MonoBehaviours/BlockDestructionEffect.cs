using UnityEngine;

namespace Tofuwu.StackCats
{
    [RequireComponent(typeof(Lifespan))]
    public class BlockDestructionEffect : MonoBehaviour
    {
        public SpriteRenderer PopRing;

        protected virtual void Start()
        {
            PopRing.transform.localScale = Vector3.one * 0.1f;
            LeanTween.scale(PopRing.gameObject, Vector3.one * 1.25f, 0.5f).setEase(LeanTweenType.easeOutSine);
            LeanTween.alpha(PopRing.gameObject, 0.0f, 0.5f).setEase(LeanTweenType.easeInSine);
        }
    }
}