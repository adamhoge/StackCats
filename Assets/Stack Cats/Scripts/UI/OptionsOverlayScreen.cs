using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class OptionsOverlayScreen : OverlayScreen
    {
        public float TransitionYOffset = 512.0f;
        public CanvasGroup HeaderCanvasGroup;
        public VerticalLayoutGroup OptionsLayoutGroup;
        public Toggle MasterEnabledToggle;
        public Toggle BackgroundMusicEnabledToggle;
        public Toggle SoundEffectsEnabledToggle;
        public Slider MasterVolumeSlider;
        public Slider BackgroundMusicVolumeSlider;
        public Slider SoundEffectsVolumeSlider;

        private readonly List<CanvasGroup> _optionsCanvasGroups = new List<CanvasGroup>();
        private bool _isInitialized;
        private AudioManager _audio;

        public override void OnActive()
        {
            foreach (CanvasGroup optionsCanvasGroup in _optionsCanvasGroups)
            {
                optionsCanvasGroup.interactable = true;
            }
        }

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            if (!_isInitialized) Initialize();

            for (int i = 0; i < _optionsCanvasGroups.Count; i++)
            {
                float delay = TransitionInDuration / _optionsCanvasGroups.Count / 2 * (_optionsCanvasGroups.Count - i - 1);
                if (delay < 0.0f) delay = 0.0f;
                LeanTween.moveLocalY(_optionsCanvasGroups[i].gameObject, 0.0f, TransitionInDuration / 2.0f).setDelay(delay).setEase(TransitionInTween);
                LeanTween.alphaCanvas(_optionsCanvasGroups[i], 1.0f, TransitionInDuration / 2.0f).setDelay(delay).setEase(LeanTweenType.easeOutSine);
            }
        }

        public override void OnTransitioningOut()
        {
            base.OnTransitioningOut();

            for (int i = 0; i < _optionsCanvasGroups.Count; i++)
            {
                _optionsCanvasGroups[i].interactable = false;
                float delay = TransitionOutDuration / _optionsCanvasGroups.Count / 2 * i;
                if (delay < 0.0f) delay = 0.0f;
                LeanTween.moveLocalY(_optionsCanvasGroups[i].gameObject, TransitionYOffset, TransitionOutDuration / 2.0f).setDelay(delay).setEase(TransitionOutTween);
                LeanTween.alphaCanvas(_optionsCanvasGroups[i], 0.0f, TransitionOutDuration / 2.0f).setDelay(delay).setEase(LeanTweenType.easeOutSine);
            }
        }

        private void Initialize()
        {
            _audio = GameManager.Instance.Audio;

            if (OptionsLayoutGroup)
            {
                foreach (RectTransform transform in OptionsLayoutGroup.transform)
                {
                    _optionsCanvasGroups.Add(transform.GetComponent<CanvasGroup>());
                }

                OptionsLayoutGroup.GetComponent<RectTransform>().WrapChildren();
            }

            foreach (CanvasGroup canvasGroup in _optionsCanvasGroups)
            {
                canvasGroup.interactable = false;
                canvasGroup.transform.localPosition = new Vector2(0, TransitionYOffset);
                canvasGroup.alpha = 0.0f;
            }

            MasterEnabledToggle.isOn = _audio.IsMasterEnabled;
            BackgroundMusicEnabledToggle.isOn = _audio.IsBackgroundMusicEnabled;
            SoundEffectsEnabledToggle.isOn = _audio.IsSoundEffectsEnabled;
            MasterVolumeSlider.value = _audio.MasterVolume;
            BackgroundMusicVolumeSlider.value = _audio.BackgroundMusicVolume;
            SoundEffectsVolumeSlider.value = _audio.SoundEffectsVolume;

            MasterEnabledToggle.onValueChanged.AddListener(OnMasterToggled);
            BackgroundMusicEnabledToggle.onValueChanged.AddListener(OnBackgroundMusicToggled);
            SoundEffectsEnabledToggle.onValueChanged.AddListener(OnSoundEffectsToggled);
            MasterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            BackgroundMusicVolumeSlider.onValueChanged.AddListener(OnBackgroundMusicVolumeChanged);
            SoundEffectsVolumeSlider.onValueChanged.AddListener(OnSoundEffectsVolumeChanged);

            _isInitialized = true;
        }

        private void OnMasterToggled(bool isEnabled)
        {
            _audio.IsMasterEnabled = isEnabled;
        }

        private void OnBackgroundMusicToggled(bool isEnabled)
        {
            _audio.IsBackgroundMusicEnabled = isEnabled;
        }

        private void OnSoundEffectsToggled(bool isEnabled)
        {
            _audio.IsSoundEffectsEnabled = isEnabled;
        }

        private void OnBackgroundMusicVolumeChanged(float volume)
        {
            _audio.BackgroundMusicVolume = volume;
        }

        private void OnMasterVolumeChanged(float volume)
        {
            _audio.MasterVolume = volume;
        }

        private void OnSoundEffectsVolumeChanged(float volume)
        {
            _audio.SoundEffectsVolume = volume;
        }
    }
}