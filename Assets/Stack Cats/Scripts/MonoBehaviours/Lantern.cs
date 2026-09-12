using UnityEngine;

namespace Tofuwu.StackCats
{
    public class Lantern : MonoBehaviour
    {
        public SpriteRenderer LightSprite;
        public Color LightColor;
        public float LightAlphaMin = 0.1f;
        public float LightAlphaMax = 0.25f;
        public float FlickerMultiplier = 5.0f;
        public float SwayRangeInDegrees = 15.0f;
        public float SwayMultiplier = 1.0f;

        private float _flickerOffset;
        private float _swayOffset;

        protected void Awake()
        {
            _flickerOffset = Random.Range(0.0f, 100.0f);
            _swayOffset = Random.Range(0.0f, 1.0f);
        }

        protected void Update()
        {
            float r = LightColor.r;
            float g = LightColor.g;
            float b = LightColor.b;
            float a = LightAlphaMin + (LightAlphaMax - LightAlphaMin) * Mathf.PerlinNoise((_flickerOffset + Time.time) * FlickerMultiplier, 0.0f);
            LightSprite.color = new Color(r, g, b, a);

            float swayRotation = -SwayRangeInDegrees / 2.0f + SwayRangeInDegrees * Mathf.PerlinNoise((_swayOffset + Time.time) * SwayMultiplier, 0.0f);
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, swayRotation);
        }
    }
}