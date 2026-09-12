using UnityEngine;

namespace Tofuwu.StackCats
{
    public class CutSceneManager : MonoBehaviour
    {
        /// <summary>
        /// The scene to load upon cut scene completion.
        /// </summary>
        public GameScene? ReturnScene { get { return _returnScene; } set { _returnScene = value; } }

        public SceneTransitionSettings ReturnSceneTransitionSettings { get { return _returnSceneTransitionSettings; } set { _returnSceneTransitionSettings = value; } }

        /// <summary>
        /// The current cut to be played when Cut Scene scene is loaded.
        /// </summary>
        public CutScene PendingCutScene { get { return _pendingCutScene; } }

        private GameManager _gameManager;
        private GameScene? _returnScene;
        private SceneTransitionSettings _returnSceneTransitionSettings;
        private CutScene _pendingCutScene;

        /// <summary>
        /// Play a cut scene immediately, then return to the current scene.
        /// </summary>
        public void PlayCutScene(CutScene cutScene)
        {
            _returnScene = _gameManager.CurrentGameScene;
            _pendingCutScene = cutScene;
            _gameManager.PlayCutScene();
        }

        /// <summary>
        /// Enqueue a cut scene to be played after the current scene unloads.
        /// </summary>
        public void EnqueueCutScene(CutScene cutScene, GameScene returnScene = GameScene.None)
        {
            _returnScene = returnScene;
            _pendingCutScene = cutScene;
        }

        public void DequeueCutScene()
        {
            _pendingCutScene = null;
        }

        protected void Awake()
        {
            _gameManager = GameManager.Instance;
        }
    }
}