using System.Collections.Generic;
using Tofuwu.StackCats.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public delegate void PuzzleCompleted(int puzzleIndex);

    public class ChallengePuzzleCompletionUI : MonoBehaviour
    {
        public event PuzzleCompleted onPuzzleCompleted;

        public int PuzzleIndex { get; set; }
        public bool IsComplete { get; set; }
        public Image BackgroundImage;
        public RectTransform RewardsRectTransform;
        public CanvasGroup CompletionImage;
        public CanvasGroup CompletionEffectImage;
        public DynamicAudioEvent CompletionSound;
        public float CompletionScaleDuration = 0.75f;
        public Color CompletedColor = Color.white;

        private AudioManager _audioManager;
        private Color _notCompletedColor = new Color(1.0f, 1.0f, 1.0f, 0.25f);
        private bool _isCompleting;

        public void Complete()
        {
            // TODO: Set text to puzzle number

            BackgroundImage.color = _notCompletedColor;
            CompletionImage.alpha = 0.0f;
            CompletionImage.transform.localScale = Vector3.one * 3.0f;
            LeanTween.alphaCanvas(CompletionImage, 1.0f, 0.75f)
                .setDelay(0.25f)
                .setEase(LeanTweenType.easeInQuint);
            LeanTween.scale(CompletionImage.gameObject, Vector3.one, 0.75f)
                .setDelay(0.25f)
                .setEase(LeanTweenType.easeInQuint)
                .setOnComplete(OnCompleteEffect);
            LeanTween.scale(BackgroundImage.gameObject, Vector3.one * 0.75f, CompletionScaleDuration)
                .setEase(LeanTweenType.punch)
                .setDelay(1.0f);

            _isCompleting = true;
        }

        protected void Awake()
        {
            _audioManager = GameManager.Instance.Audio;
            CompletionImage.alpha = 0.0f;
            CompletionEffectImage.alpha = 0.0f;
        }

        protected void Start()
        {
            if (!IsComplete)
            {
                BackgroundImage.color = _notCompletedColor;
                CompletionImage.alpha = 0.0f;

                // TODO: Set text to puzzle number
            }
            else if (!_isCompleting)
            {
                BackgroundImage.color = CompletedColor;
                CompletionImage.alpha = 1.0f;
            }
        }

        private void OnCompleteEffect()
        {
            _audioManager.PlaySoundEffect(CompletionSound);
            BackgroundImage.color = CompletedColor;
            CompletionEffectImage.alpha = 1.0f;
            LeanTween.alphaCanvas(CompletionEffectImage, 0.0f, 0.25f)
                .setEase(LeanTweenType.easeOutCubic);
            LeanTween.scale(CompletionEffectImage.gameObject, Vector3.one * 3.0f, 0.25f)
                .setEase(LeanTweenType.easeOutCubic);

            // TODO?: Clear puzzle number

            if (onPuzzleCompleted != null) onPuzzleCompleted(PuzzleIndex);
        }
    }
}
