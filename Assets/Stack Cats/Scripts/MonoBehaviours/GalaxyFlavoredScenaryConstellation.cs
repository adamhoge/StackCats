using UnityEngine;
using System.Collections.Generic;

namespace Tofuwu.StackCats
{
    public class GalaxyFlavoredScenaryConstellation : MonoBehaviour
    {
        public float AnimationFrameDuration = 1.0f;

        public List<SpriteRenderer> AnimationFrames = new List<SpriteRenderer>();

        private float _totalAnimationDuration;
        private float _animationStartTime;
        private int _currentAnimationFrameIndex = -1;

        protected void OnEnable()
        {
            _animationStartTime = Time.time;
            foreach(SpriteRenderer frame in AnimationFrames)
            {
                frame.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            }
        }

        protected void OnDisable()
        {
            if (_currentAnimationFrameIndex != -1)
            {
                SpriteRenderer currentFrame = AnimationFrames[_currentAnimationFrameIndex];
                LeanTween.alpha(currentFrame.gameObject, 0.0f, AnimationFrameDuration / 2);
            }
        }

        protected void Start()
        {
            _totalAnimationDuration = AnimationFrameDuration * AnimationFrames.Count;
        }

        protected void Update()
        {
            float animationCycleTimeElapsed = (Time.time - _animationStartTime) % _totalAnimationDuration;
            int updatedAnimationFrameIndex = Mathf.FloorToInt(animationCycleTimeElapsed / AnimationFrameDuration);

            if (_currentAnimationFrameIndex != updatedAnimationFrameIndex)
            {
                if (_currentAnimationFrameIndex != -1)
                {
                    SpriteRenderer currentFrame = AnimationFrames[_currentAnimationFrameIndex];
                    LeanTween.alpha(currentFrame.gameObject, 0.0f, AnimationFrameDuration / 2);
                }

                _currentAnimationFrameIndex = updatedAnimationFrameIndex;
                SpriteRenderer newFrame = AnimationFrames[_currentAnimationFrameIndex];
                LeanTween.alpha(newFrame.gameObject, 1.0f, AnimationFrameDuration / 2);
            }
        }
    }
}