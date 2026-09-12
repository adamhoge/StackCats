using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Tofuwu.StackCats.UI
{
    public enum OverlayScreenManagerState
    {
        Idle,
        Active,
        ScreenTransitioningIn,
        ScreenTransitioningOut,
    }

    public class OverlayScreenManager : MonoBehaviour
    {
        public CanvasGroup Overlay;
        public UnityEvent OnVisible;
        public UnityEvent OnHidden;

        public bool IsDisplayingScreen { get { return _currentScreen != null; } }

        private OverlayScreenManagerState _state;
        private float _stateTimeElapsed;
        private readonly Queue<OverlayScreen> _screenQueue = new Queue<OverlayScreen>();
        private OverlayScreen _currentScreen;
        private bool _dismissOnActive;

        /// <summary>
        /// Enqueue a screen for display.
        /// </summary>
        /// <param name="screen">The screen to display.</param>
        public void EnqueueScreen(OverlayScreen screen)
        {
            EnqueueScreen(screen, false);
        }

        /// <summary>
        /// Enqueue a screen for display.
        /// </summary>
        /// <param name="screen">The screen to display.</param>
        /// <param name="removeOthers">A flag indicating whether or not to remove other queued screens.</param>
        public void EnqueueScreen(OverlayScreen screen, bool removeOthers)
        {
            if (removeOthers)
            {
                _screenQueue.Clear();
                DismissCurrentScreen();
            }

            if (screen && !_screenQueue.Contains(screen))
            {
                _screenQueue.Enqueue(screen);
                if (_state == OverlayScreenManagerState.Idle) LoadNextScreen();
            }
        }

        /// <summary>
        /// Dismiss the currently active screen (if any)
        /// </summary>
        public void DismissCurrentScreen()
        {
            if (_currentScreen)
            {
                if (_state == OverlayScreenManagerState.Active)
                {
                    bool hideOverlay = _screenQueue.Count == 0;

                    _currentScreen.OnTransitioningOut();
                    _state = OverlayScreenManagerState.ScreenTransitioningOut;
                    _stateTimeElapsed = 0.0f;

                    if (Overlay && hideOverlay) LeanTween.alphaCanvas(Overlay, 0.0f, _currentScreen.TransitionOutDuration).setEase(_currentScreen.TransitionOutTween);
                }
                else if (_state == OverlayScreenManagerState.ScreenTransitioningIn)
                {
                    _dismissOnActive = true;
                }
            }
        }

        /// <summary>
        /// Clears the screen queue and dismisses the current screen.
        /// </summary>
        public void DismissAllScreens()
        {
            _screenQueue.Clear();
            DismissCurrentScreen();
        }

        protected void Update()
        {
            _stateTimeElapsed += Time.deltaTime;

            switch (_state)
            {
                case OverlayScreenManagerState.Idle:
                    break;
                case OverlayScreenManagerState.Active:
                    break;
                case OverlayScreenManagerState.ScreenTransitioningIn:
                    if (_stateTimeElapsed >= _currentScreen.TransitionInDuration)
                    {
                        _currentScreen.OnActive();
                        _state = OverlayScreenManagerState.Active;
                        _stateTimeElapsed = 0.0f;

                        if (_dismissOnActive)
                        {
                            DismissCurrentScreen();
                            _dismissOnActive = false;
                        }
                    }
                    break;
                case OverlayScreenManagerState.ScreenTransitioningOut:
                    if (_stateTimeElapsed >= _currentScreen.TransitionOutDuration)
                    {
                        _currentScreen.OnHidden();
                        _currentScreen.DisplayedBy = null;
                        _currentScreen.gameObject.SetActive(false);

                        if (_screenQueue.Count > 0)
                        {
                            LoadNextScreen();
                        }
                        else
                        {
                            _currentScreen = null;
                            _state = OverlayScreenManagerState.Idle;
                            _stateTimeElapsed = 0.0f;

                            if (Overlay)
                            {
                                Overlay.blocksRaycasts = false;
                                Overlay.interactable = false;
                            }

                            OnHidden.Invoke();
                        }
                    }
                    break;
            }
        }

        private void LoadNextScreen()
        {
            bool hasOverlay = _currentScreen;

            _currentScreen = _screenQueue.Dequeue();
            _currentScreen.DisplayedBy = this;
            _currentScreen.gameObject.SetActive(true);
            _currentScreen.OnTransitioningIn();
            _state = OverlayScreenManagerState.ScreenTransitioningIn;
            _stateTimeElapsed = 0.0f;

            if (Overlay)
            {
                if (!hasOverlay)
                {
                    LeanTween.alphaCanvas(Overlay, 1.0f, _currentScreen.TransitionInDuration).setEase(_currentScreen.TransitionInTween);
                    Overlay.GetComponent<Image>().color = _currentScreen.OverlayColor;
                    Overlay.blocksRaycasts = true;
                    Overlay.interactable = true;
                }
                else
                {
                    LeanTween.color(Overlay.GetComponent<RectTransform>(), _currentScreen.OverlayColor, _currentScreen.TransitionInDuration);
                }
            }

            OnVisible.Invoke();
        }
    }
}