using UnityEngine;

namespace Tofuwu.StackCats
{
    public class BlockParticleEffect : ParticleEffect
    {
        public float FadeDuration;
        public SpriteRenderer BlockSpriteRenderer;

        private float _effectStartTime;

        protected void OnEnable()
        {
            _effectStartTime = Time.time;
        }

        protected override void Update()
        {
            float alphaTransform = 1.0f - (Time.time - _effectStartTime) / FadeDuration;
            BlockSpriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, alphaTransform);

            base.Update();
        }
    }
}