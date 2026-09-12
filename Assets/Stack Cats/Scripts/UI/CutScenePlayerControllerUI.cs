using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tofuwu.StackCats
{
    public class CutScenePlayerControllerUI : MonoBehaviour
    {
        public CutScenePlayerController CutScenePlayerController;
        public GameObject SkipInfo;
        public CanvasGroup SkipCanvasGroup;
        public Image SkipMaskImage;

        protected void OnEnable()
        {
            CutScenePlayerController.onBeginHoldToStop += OnBeginHoldToSkip;
            CutScenePlayerController.onHoldingToStop += OnHoldingToSkip;
            CutScenePlayerController.onCancelHoldToStop += OnCancelHoldToSkip;
            CutScenePlayerController.onCompleteHoldToStop += OnCompleteHoldToSkip;
        }

        protected void OnDisable()
        {
            CutScenePlayerController.onBeginHoldToStop -= OnBeginHoldToSkip;
            CutScenePlayerController.onHoldingToStop -= OnHoldingToSkip;
            CutScenePlayerController.onCancelHoldToStop -= OnCancelHoldToSkip;
            CutScenePlayerController.onCompleteHoldToStop -= OnCompleteHoldToSkip;
        }

        private void OnBeginHoldToSkip()
        {
            LeanTween.cancel(SkipInfo);
            SkipInfo.transform.localScale = Vector3.one;
            LeanTween.scale(SkipInfo, Vector3.one * 0.9f, 0.5f).setEase(LeanTweenType.punch);

            LeanTween.cancel(SkipCanvasGroup.gameObject);
            SkipCanvasGroup.alpha = 0.0f;
            LeanTween.alphaCanvas(SkipCanvasGroup, 0.5f, 1.5f).setEase(LeanTweenType.easeOutQuint);
        }

        private void OnHoldingToSkip(float progress)
        {
            SkipMaskImage.fillAmount = progress;
        }

        private void OnCancelHoldToSkip()
        {
            SkipMaskImage.fillAmount = 0.0f;

            LeanTween.cancel(SkipCanvasGroup.gameObject);
            LeanTween.alphaCanvas(SkipCanvasGroup, 0.0f, 0.5f).setEase(LeanTweenType.easeOutQuint);
        }

        private void OnCompleteHoldToSkip()
        {
            SkipMaskImage.fillAmount = 0.0f;

            LeanTween.cancel(SkipCanvasGroup.gameObject);
            LeanTween.alphaCanvas(SkipCanvasGroup, 0.0f, 0.5f).setEase(LeanTweenType.easeOutSine);
        }
    }
}