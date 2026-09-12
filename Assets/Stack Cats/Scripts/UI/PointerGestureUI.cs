using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class PointerGestureUI : MonoBehaviour
    {
        public CanvasGroup PointerCanvasGroup;
        public Image PointerImage;
        public Sprite PointerUpSprite;
        public Sprite PointerDownSprite;
        public Color PressedColor = Color.gray;
        public float PressedAnimationDuration = 0.5f;
        public Vector3 UnpressedScale = Vector3.one;
        public Vector3 PressedScale = Vector3.one * 0.8f;
        public float VisibleAnimationDuration = 0.5f;
        public float PositionAnimationDuration = 0.5f;

        private List<int> _pressedTweens = new List<int>();
        private List<int> _visibleTweens = new List<int>();
        private List<int> _positionTweens = new List<int>();

        public void SetPressed(bool isPressed, bool isAnimated = true)
        {
            CancelTweens(_pressedTweens);

            if (isAnimated)
            {
                AnimateIsPressed(isPressed);
            }
            else
            {
                PointerCanvasGroup.transform.localScale = isPressed ? PressedScale : Vector3.one;
                PointerImage.color = isPressed ? PressedColor : Color.white;
                SetPressedSprite(isPressed);
            }
        }

        public void SetVisible(bool isVisible, bool isAnimated = true)
        {
            CancelTweens(_visibleTweens);

            float alpha = isVisible ? 1.0f : 0.0f;
            if (isAnimated)
            {
                _visibleTweens.Add(LeanTween
                    .alphaCanvas(PointerCanvasGroup, alpha, VisibleAnimationDuration)
                    .setEase(LeanTweenType.easeInOutSine)
                    .id);
            }
            else
            {
                PointerCanvasGroup.alpha = alpha;
            }
        }

        public void SetPosition(Vector3 position, bool isAnimated = true)
        {
            CancelTweens(_positionTweens);

            if (isAnimated)
            {
                _positionTweens.Add(LeanTween
                    .move(PointerCanvasGroup.gameObject, position, PositionAnimationDuration)
                    .setEase(LeanTweenType.easeOutSine)
                    .id);
            }
            else
            {
                PointerCanvasGroup.transform.position = position;
            }
        }

        private void AnimateIsPressed(bool isPressed)
        {
            if (isPressed)
            {
                float outDuration = PressedAnimationDuration * 0.9f;
                float inDuration = PressedAnimationDuration * 0.1f;
                _pressedTweens.Add(LeanTween
                    .scale(PointerCanvasGroup.gameObject, PressedScale * 0.9f, outDuration)
                    .setEase(LeanTweenType.easeOutSine)
                    .id);
                _pressedTweens.Add(LeanTween
                    .scale(PointerCanvasGroup.gameObject, PressedScale, inDuration)
                    .setEase(LeanTweenType.easeInSine)
                    .setDelay(outDuration)
                    .setOnComplete(delegate () { SetPressedSprite(true); })
                    .id);
            }
            else
            {
                SetPressedSprite(false);
                _pressedTweens.Add(LeanTween
                    .scale(PointerCanvasGroup.gameObject, Vector3.one, PressedAnimationDuration)
                    .setEase(LeanTweenType.easeInSine)
                    .id);
            }

            _pressedTweens.Add(LeanTween
                .color(PointerImage.gameObject, isPressed ? PressedColor : Color.white, PressedAnimationDuration)
                .setEase(LeanTweenType.easeOutSine)
                .id);
        }

        private void SetPressedSprite(bool isDown)
        {
            PointerImage.sprite = isDown ? PointerDownSprite : PointerUpSprite;
        }

        private void CancelTweens(List<int> tweenIds)
        {
            foreach (int tweenId in tweenIds)
            {
                LeanTween.cancel(tweenId);
            }
            tweenIds.Clear();
        }
    }
}