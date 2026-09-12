using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Tofuwu.StackCats
{
    public enum SceneState
    {
        ACTIVE,
        ENTERING,
        EXITING,
        LOADING
    }

    [Serializable]
    public struct SceneTransitionSettings
    {
        /// <summary>
        /// The color used when fading out of a scene.
        /// </summary>
        public Color TransitionColor;

        /// <summary>
        /// The duration of the transition into a scene.
        /// </summary>
        public float TransitionInDuration;

        /// <summary>
        /// The duration of the transition out of a scene.
        /// </summary>
        public float TransitionOutDuration;

        /// <summary>
        /// Whether or not input is enabled while a scene is transitioning.
        /// </summary>
        public bool IsInputEnabledOnTransition;

        /// <summary>
        /// Whether or not the transitione should fade out background music during the transition.
        /// </summary>
        public bool ShouldFadeBackgroundMusic;
    }

    public delegate void SceneStateChanged(SceneState state);

    [RequireComponent(typeof(AudioManager))]
    [RequireComponent(typeof(ScreenOverlay))]
    public class SceneTransitioner : MonoBehaviour
    {
        /// <summary>
        /// Invoked whenever the scene state changes.
        /// </summary>
        public event SceneStateChanged onSceneStateChanged;

        /// <summary>
        /// The settings used for transitions.
        /// </summary>
        public SceneTransitionSettings TransitionSettings;

        /// <summary>
        /// The current state of the scene transitioner.
        /// </summary>
        public SceneState State { get { return _state; } }

        /// <summary>
        /// The position of the scene transitioner.
        /// </summary>
        public float TransitionPosition { get { return _transitionPosition; } }

        private SceneState _state = SceneState.ENTERING;
        private float _stateTimeElapsed;
        private int _targetScene;
        private float _transitionPosition = 1.0f;
        private AudioManager _audioManager;
        private Guid _backgroundMusicVolumeModifierGuid;
        private ScreenOverlay _screenOverlay;
        private SceneTransitionSettings _activeSceneTransitionSettings;

        void Awake()
        {
            _audioManager = GetComponent<AudioManager>();
            _screenOverlay = GetComponent<ScreenOverlay>();

            _backgroundMusicVolumeModifierGuid = _audioManager.AddBackgroundMusicVolumeModifier();
        }

        void Start()
        {
            _activeSceneTransitionSettings = TransitionSettings;
        }

        void Update()
        {
            _stateTimeElapsed += Time.deltaTime;

            if (_state == SceneState.ENTERING)
            {
                if (UpdateTransition(1))
                {
                    ChangeSceneState(SceneState.ACTIVE);
                }
            }
            else if (_state == SceneState.EXITING)
            {
                if (UpdateTransition(-1))
                {
                    ChangeSceneState(SceneState.LOADING);
                    StartCoroutine(ILoadScene(_targetScene));
                }
            }
            else if (_state == SceneState.LOADING)
            {
                if (_stateTimeElapsed > 0.5f)
                {
                }
            }

            if (_state != SceneState.ACTIVE)
            {
                _screenOverlay.SetAlpha(1 - _transitionPosition);
                if (_activeSceneTransitionSettings.ShouldFadeBackgroundMusic)
                {
                    _audioManager.SetBackgroundMusicVolumeModifier(_backgroundMusicVolumeModifierGuid, _transitionPosition);
                }
            }
        }

        public void LoadScene(int sceneBuildIndex)
        {
            _targetScene = sceneBuildIndex;
            _activeSceneTransitionSettings = TransitionSettings;
            _screenOverlay.OverlayColor = _activeSceneTransitionSettings.TransitionColor;
            _screenOverlay.SetAlpha(0.0f);
            ChangeSceneState(SceneState.EXITING);
        }

        private IEnumerator ILoadScene(int sceneBuildIndex)
        {
            AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(sceneBuildIndex);

            while (!asyncLoadScene.isDone)
            {
                yield return null;
            }

            GC.Collect();
            ChangeSceneState(SceneState.ENTERING);
        }

        private void ChangeSceneState(SceneState state)
        {
            if (_state != state)
            {
                switch (state)
                {
                    case SceneState.ACTIVE:
                        _screenOverlay.SetAlpha(0.0f);
                        if (_activeSceneTransitionSettings.ShouldFadeBackgroundMusic)
                        {
                            _audioManager.SetBackgroundMusicVolumeModifier(_backgroundMusicVolumeModifierGuid, 1.0f);
                        }
                        break;
                    case SceneState.ENTERING:
                        break;
                    case SceneState.EXITING:
                        if (EventSystem.current) EventSystem.current.enabled = _activeSceneTransitionSettings.IsInputEnabledOnTransition;
                        break;
                    case SceneState.LOADING:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _state = state;
                _stateTimeElapsed = 0.0f;
                if (onSceneStateChanged != null) onSceneStateChanged(_state);
            }
        }

        private bool UpdateTransition(int direction)
        {
            bool transitionCompleted = false;

            // Update the transition position.
            float transitionDuration = direction > 0 ? _activeSceneTransitionSettings.TransitionInDuration : _activeSceneTransitionSettings.TransitionOutDuration;
            _transitionPosition = transitionDuration > 0 ? _transitionPosition + (Time.unscaledDeltaTime / transitionDuration) * direction : direction;

            // Determine if the transition is complete.
            if ((direction < 0 && _transitionPosition <= -1) || (direction > 0 && _transitionPosition >= 1))
            {
                _transitionPosition = Mathf.Clamp(_transitionPosition, 0, 1);
                transitionCompleted = true;
            }

            return transitionCompleted;
        }
    }
}