using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class PlayMenuOverlayScreen : OverlayScreen
    {
        public float TransitionYOffset = 512.0f;
        public CanvasGroup HeaderCanvasGroup;
        public VerticalLayoutGroup MenuLayoutGroup;

        private List<Button> _menuButtons = new List<Button>();
        private List<CanvasGroup> _menuItemWrapperCanvasGroups = new List<CanvasGroup>();
        private bool _isInitialized;

        public override void OnActive()
        {
            foreach(Button menuButton in _menuButtons)
            {
                menuButton.interactable = true;
            }
        }

        public override void OnHidden() { }

        public override void OnTransitioningIn()
        {
            if (!_isInitialized) Initialize();

            for (int i = 0; i < _menuButtons.Count; i++)
            {
                float delay = TransitionInDuration / _menuButtons.Count / 2 * (_menuButtons.Count - i);
                if (delay < 0.0f) delay = 0.0f;
                LeanTween.moveLocalY(_menuButtons[i].gameObject, 0.0f, TransitionInDuration / 2.0f).setDelay(delay).setEase(TransitionInTween);
                LeanTween.alphaCanvas(_menuItemWrapperCanvasGroups[i], 1.0f, TransitionInDuration / 2.0f).setDelay(delay).setEase(LeanTweenType.easeOutSine);
            }
        }

        public override void OnTransitioningOut()
        {
            for (int i = 0; i < _menuButtons.Count; i++)
            {
                _menuButtons[i].interactable = false;
                float delay = TransitionOutDuration / _menuButtons.Count / 2 * i;
                if (delay < 0.0f) delay = 0.0f;
                LeanTween.moveLocalY(_menuButtons[i].gameObject, TransitionYOffset, TransitionOutDuration / 2.0f).setDelay(delay).setEase(TransitionOutTween);
                LeanTween.alphaCanvas(_menuItemWrapperCanvasGroups[i], 0.0f, TransitionOutDuration / 2.0f).setDelay(delay).setEase(LeanTweenType.easeOutSine);
            }
        }

        private void Initialize()
        {
            if (MenuLayoutGroup)
            {
                foreach (RectTransform transform in MenuLayoutGroup.transform) { _menuButtons.Add(transform.GetComponent<Button>()); }

                MenuLayoutGroup.GetComponent<RectTransform>().WrapChildren();
                foreach (RectTransform transform in MenuLayoutGroup.transform) { _menuItemWrapperCanvasGroups.Add(transform.gameObject.AddComponent<CanvasGroup>()); }
            }

            for (int i = 0; i < _menuButtons.Count; i++)
            {
                _menuButtons[i].interactable = false;
                _menuButtons[i].transform.localPosition = new Vector2(0, TransitionYOffset);
                _menuItemWrapperCanvasGroups[i].alpha = 0.0f;
            }

            _isInitialized = true;
        }
    }
}