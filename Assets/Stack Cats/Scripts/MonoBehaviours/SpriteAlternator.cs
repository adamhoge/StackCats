using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public enum AlternationDirection
    {
        Forward,
        Reverse,
        PingPong
    }

    public class SpriteAlternator : MonoBehaviour
    {
        /// <summary>
        /// The sprite renderer used for the current sprite.
        /// </summary>
        public SpriteRenderer CurrentSpriteRenderer;

        /// <summary>
        /// The sprite renderer used for the previous sprite.
        /// </summary>
        public SpriteRenderer PreviousSpriteRenderer;

        /// <summary>
        /// The collection of sprites being alternated.
        /// </summary>
        public List<Sprite> Sprites;

        /// <summary>
        /// The direction of alternation.
        /// </summary>
        public AlternationDirection Direction;

        /// <summary>
        /// The duration (in seconds) that each sprite will be individually displayed.
        /// </summary>
        public float DisplayDuration = 0.5f;

        /// <summary>
        /// The duration (in seconds) that each sprite will overlap during alternation.
        /// </summary>
        public float OverlapDuration = 0.0f;

        /// <summary>
        /// The duration (in seconds) of the transition in animation (if specified).
        /// </summary>
        public float TransitionInDuration = 0.0f;

        /// <summary>
        /// The duration (in seconds) of the transition out animation (if specified).
        /// </summary>
        public float TransitionOutDuration = 0.0f;

        /// <summary>
        /// The tween type used (if any) for sprite alpha when transitioning in.
        /// </summary>
        public LeanTweenType TransitionInTween = LeanTweenType.notUsed;

        /// <summary>
        /// The tween type used (if any) for sprite alpha when transitioning out.
        /// </summary>
        public LeanTweenType TransitionOutTween = LeanTweenType.notUsed;

        private int _currentSpriteIndex;
        private float _lastAlternationTime;
        private bool _pingPongForward = true;

        protected void Start()
        {
            CurrentSpriteRenderer.sprite = Sprites[0];
            _lastAlternationTime = Time.time;
        }

        protected void Update()
        {
            if (Time.time > _lastAlternationTime + DisplayDuration + TransitionInDuration)
            {
                TransitionSpriteOut(Sprites[_currentSpriteIndex]);
                _currentSpriteIndex = GetNextSpriteIndex();
                TransitionSpriteIn(Sprites[_currentSpriteIndex]);
                _lastAlternationTime = Time.time;
            }
        }

        private void TransitionSpriteIn(Sprite sprite)
        {
            CurrentSpriteRenderer.sprite = sprite;

            CurrentSpriteRenderer.color = Constants.ClearWhite;
            if (TransitionInDuration == 0 || TransitionInTween == LeanTweenType.notUsed)
            {
                LeanTween.alpha(CurrentSpriteRenderer.gameObject, 1.0f, 0.0f);
            }
            else
            {
                LeanTween.alpha(CurrentSpriteRenderer.gameObject, 1.0f, TransitionInDuration + OverlapDuration).setEase(TransitionInTween);
            }
        }

        private void TransitionSpriteOut(Sprite sprite)
        {
            CurrentSpriteRenderer.sprite = null;
            PreviousSpriteRenderer.sprite = sprite;

            PreviousSpriteRenderer.color = Color.white;
            if (TransitionOutDuration == 0 || TransitionOutTween == LeanTweenType.notUsed)
            {
                LeanTween.alpha(PreviousSpriteRenderer.gameObject, 0.0f, 0.0f)
                    .setDelay(OverlapDuration);
            }
            else
            {
                LeanTween.alpha(PreviousSpriteRenderer.gameObject, 0.0f, TransitionOutDuration)
                    .setEase(TransitionOutTween)
                    .setDelay(OverlapDuration);
            }
        }

        private int GetNextSpriteIndex()
        {
            switch (Direction)
            {
                case AlternationDirection.Forward:
                    return ++_currentSpriteIndex % Sprites.Count;
                case AlternationDirection.Reverse:
                    return (--_currentSpriteIndex + Sprites.Count) % Sprites.Count;
                case AlternationDirection.PingPong:
                    if (_currentSpriteIndex == 0)
                    {
                        _pingPongForward = true;
                        return 1;
                    }
                    else if (_currentSpriteIndex == Sprites.Count - 1)
                    {
                        _pingPongForward = false;
                        return Sprites.Count - 2;
                    }
                    else
                    {
                        return _pingPongForward ? ++_currentSpriteIndex : --_currentSpriteIndex;
                    }
            }

            return -1;
        }
    }
}
