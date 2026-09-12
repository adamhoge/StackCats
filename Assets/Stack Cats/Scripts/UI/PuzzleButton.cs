using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Tofuwu.StackCats.UI
{
    public delegate void StarsAdded(PuzzleButton sender, int numStars);

    [RequireComponent(typeof(Button))]
    public class PuzzleButton : MonoBehaviour
    {
        [Serializable]
        public class PuzzleStar
        {
            public Image StarImage;
            public CanvasGroup StarFlashCanvasGroup;
        }

        /// <summary>
        /// Invoked when star images are added to the button.
        /// </summary>
        public event StarsAdded onStarsAdded;

        public CanvasGroup ButtonFlashCanvasGroup;
        public TextMeshProUGUI PuzzleLabelText;
        public RectTransform PuzzleUnlocksAreaIcon;
        public RectTransform PuzzleHasCutSceneIcon;
        public Image IsSelectedImage;
        public Image PuzzleCompleteImage;
        public Mask ButtonFrameMask;
        public CanvasGroup PuzzleCompleteCanvasGroup;
        public DynamicAudioEvent PuzzleCompleteSoundEffect;
        public CanvasGroup PuzzleStarsCanvasGroup;
        public List<PuzzleStar> PuzzleStars;
        public Color HasntStarColor;
        public Color HasStarColor;
        public bool IsSelected;
        public bool IsComplete;
        public bool UnlocksArea;
        public bool HasCutScene;
        public int NumStarsEarned;

        public bool IsEnabled { get { return _isEnabled; } set { SetIsEnabled(value); } }

        public bool IsLocked { get { return _isLocked; } set { SetIsLocked(value); } }

        public Button Button { get { return _button; } }

        private AudioManager _audioManager;
        private Button _button;
        private bool _isEnabled = true;
        private bool _isLocked;

        public void Complete()
        {
            PuzzleCompleteCanvasGroup.alpha = 0.0f;
            PuzzleCompleteCanvasGroup.transform.localScale = Vector2.one * 3.0f;
            LeanTween.alphaCanvas(PuzzleCompleteCanvasGroup, 1.0f, 0.5f)
                .setDelay(0.25f)
                .setEase(LeanTweenType.easeInQuint);
            LeanTween.scale(PuzzleCompleteCanvasGroup.gameObject, Vector2.one, 0.5f)
                .setDelay(0.25f)
                .setEase(LeanTweenType.easeInQuint)
                .setOnComplete(delegate ()
                {
                    _audioManager.PlaySoundEffect(PuzzleCompleteSoundEffect);
                });
            LeanTween.scale(gameObject, Vector3.one * 0.75f, 0.5f)
                .setDelay(0.75f)
                .setEase(LeanTweenType.punch);
        }

        public void ChangeStars(int prevAmount, int curAmount, bool wasPreviouslyCompleted)
        {
            if (prevAmount == curAmount) return;

            NumStarsEarned = prevAmount;
            for (int i = prevAmount; i < PuzzleStars.Count; i++)
            {
                PuzzleStars[i].StarImage.color = HasntStarColor;
            }

            IEnumerator addStarsCoroutine = AddStars(prevAmount, curAmount, 0.75f);
            StartCoroutine(addStarsCoroutine);

            if (curAmount == PuzzleStars.Count)
            {
                LeanTween.alphaCanvas(ButtonFlashCanvasGroup, 1.0f, 0.1f).setDelay(0.75f).setOnComplete(AddFullCompletion, wasPreviouslyCompleted);
            }
        }

        public void Unlock()
        {
            IsLocked = true;
            PuzzleStarsCanvasGroup.gameObject.SetActive(false);
            PuzzleLabelText.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);

            LeanTween.rotateAround(gameObject, Vector3.up, 180.0f, 0.3f)
                .setDelay(1.0f)
                .setEase(LeanTweenType.easeInQuint)
                .setOnComplete(EnableButton);
        }

        protected void Awake()
        {
            _audioManager = GameManager.Instance.Audio;
            _button = GetComponent<Button>();
        }

        protected void Start()
        {
            _button.interactable = IsEnabled && !IsLocked;
            IsSelectedImage.gameObject.SetActive(IsSelected);
            PuzzleCompleteCanvasGroup.gameObject.SetActive(IsComplete);
            PuzzleUnlocksAreaIcon.gameObject.SetActive(!IsComplete && UnlocksArea);
            PuzzleHasCutSceneIcon.gameObject.SetActive(!IsComplete && HasCutScene);
            PuzzleStarsCanvasGroup.gameObject.SetActive(!IsLocked);
            PuzzleLabelText.color = IsLocked ? new Color(1.0f, 1.0f, 1.0f, 0.5f) : Color.white;
            PuzzleLabelText.rectTransform.anchorMin = IsLocked && !UnlocksArea ? new Vector2(0.0f, 0.0f) : new Vector2(0.0f, 0.25f);
            for (int i = 0; i < NumStarsEarned && i < PuzzleStars.Count; i++)
            {
                PuzzleStars[i].StarImage.color = HasStarColor;
            }
            if (NumStarsEarned == PuzzleStars.Count) ButtonFrameMask.showMaskGraphic = true;
        }

        private void EnableButton()
        {
            IsLocked = false;
            PuzzleStarsCanvasGroup.gameObject.SetActive(true);
            PuzzleLabelText.color = Color.white;
            PuzzleLabelText.rectTransform.anchorMin = new Vector2(0.0f, 0.25f);
            LeanTween.rotateAround(gameObject, Vector3.up, 180.0f, 1.0f).setEase(LeanTweenType.easeOutElastic);
        }

        private IEnumerator AddStars(int prevAmount, int curAmount, float delay)
        {
            yield return new WaitForSeconds(delay);

            for (int i = prevAmount; i < curAmount && i < PuzzleStars.Count; i++)
            {
                PuzzleStar puzzleStar = PuzzleStars[i];
                LeanTween.alphaCanvas(puzzleStar.StarFlashCanvasGroup, 1.0f, 0.1f).setOnComplete(AddStar, puzzleStar);
            }

            if (onStarsAdded != null) onStarsAdded(this, curAmount - prevAmount);

            yield return null;
        }

        private void AddStar(object puzzleStarObj)
        {
            PuzzleStar puzzleStar = (PuzzleStar)puzzleStarObj;
            puzzleStar.StarImage.color = HasStarColor;
            LeanTween.scale(puzzleStar.StarImage.gameObject, Vector3.one * 1.25f, 0.25f).setEase(LeanTweenType.punch);
            LeanTween.alphaCanvas(puzzleStar.StarFlashCanvasGroup, 0.0f, 1.0f).setEase(LeanTweenType.easeInSine);
        }

        private void AddFullCompletion(object wasPreviouslyCompleted)
        {
            ButtonFrameMask.showMaskGraphic = true;
            LeanTween.alphaCanvas(ButtonFlashCanvasGroup, 0.0f, 1.0f).setEase(LeanTweenType.easeInSine);
            //if((bool)wasPreviouslyCompleted == true) LeanTween.scale(gameObject, Vector3.one * 1.05f, 0.5f).setEase(LeanTweenType.punch);
        }

        private void SetIsEnabled(bool value)
        {
            if (_isEnabled == value) return;

            _isEnabled = value;
            _button.interactable = !_isLocked && _isEnabled;
        }

        private void SetIsLocked(bool value)
        {
            if (_isLocked == value) return;

            _isLocked = value;
            _button.interactable = !_isLocked && _isEnabled;
        }
    }
}