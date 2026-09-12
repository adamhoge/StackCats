using System;
using UnityEngine;

namespace Tofuwu.StackCats
{
    // TODO: Move background music logic out of behaviour.
    // TODO: Possibly remove this base implementation altogether.
    public class SceneBehaviour : MonoBehaviour
    {
        public AudioLoop BackgroundMusic;
        public bool InheritBackgroundMusic;

        /// <summary>
        /// The current state of the scene.
        /// </summary>
        public SceneState SceneState { get { return _sceneState; } }

        protected GameManager _gameManager;
        protected SceneState _sceneState;

        private Guid _backgroundVolumeModifierGuid;

        protected virtual void Awake()
        {
            _gameManager = GameManager.Instance;
        }

        protected virtual void OnEnable()
        {
            _gameManager.GameScenes.SceneTransitioner.onSceneStateChanged += OnSceneStateChanged;
        }

        protected virtual void OnDisable()
        {
            _gameManager.GameScenes.SceneTransitioner.onSceneStateChanged -= OnSceneStateChanged;
        }

        protected virtual void Start()
        {
            if (!_gameManager)
            {
                Debug.Log("Game manager required to use a scene behaviour. Removing this component.");
                Destroy(this);
                return;
            }
            else
            {
                OnTransitioningIn();
            }
        }

        protected virtual void OnTransitioningIn()
        {
            if (_gameManager.Audio.BackgroundMusic.CurrentAudioLoop != BackgroundMusic && !InheritBackgroundMusic)
            {
                _gameManager.Audio.PlayBackgroundMusic(BackgroundMusic);
            }
        }

        protected virtual void OnTransitioningOut() { }

        private void OnSceneStateChanged(SceneState state)
        {
            _sceneState = state;

            switch (_sceneState)
            {
                case SceneState.ACTIVE:
                    break;
                case SceneState.ENTERING:
                    _backgroundVolumeModifierGuid = _gameManager.Audio.AddBackgroundMusicVolumeModifier();
                    break;
                case SceneState.EXITING:
                    OnTransitioningOut();
                    break;
                case SceneState.LOADING:
                    _gameManager.Audio.RemoveBackgroundMusicVolumeModifier(_backgroundVolumeModifierGuid);
                    break;
            }
        }
    }
}