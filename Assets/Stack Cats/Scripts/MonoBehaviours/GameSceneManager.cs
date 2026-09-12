using UnityEngine;

namespace Tofuwu.StackCats
{
    public enum GameScene
    {
        None = -1,
        Title = 0,
        CutScene = 1,
        Tutorial = 2,
        Home = 3,
        Cats = 4,
        Shop = 5,
        OpenPresents = 6,
        Map = 7,
        StoryPuzzle = 8,
        ChallengeRun = 9,
        EndlessRun = 10,
        Minigame = 11,
        PuzzleMaker = 12,
        ReplayCutScenes = 13
    }

    // TODO: Create GameSceneData scriptable object
    // TODO: Use GameSceneData instead of enums to manage scenes
    // TODO: Require AudioManager
    // TODO: Handle background music fade in/out based on GameSceneData to load
    public class GameSceneManager : MonoBehaviour
    {
        public SceneTransitioner SceneTransitioner;

        public GameScene CurrentGameScene { get { return _currentGameScene; } }

        public GameScene PreviousGameScene { get { return _previousGameScene; } }

        private CutSceneManager _cutSceneManager;
        private GameScene _currentGameScene;
        private GameScene _previousGameScene;

        /// <summary>
        /// Go to a game scene.
        /// </summary>
        /// <param name="scene">The destination game scene.</param>
        public void GoToScene(GameScene scene, SceneTransitionSettings transitionSettings)
        {
            if (scene != GameScene.CutScene && _cutSceneManager.PendingCutScene)
            {
                if(_cutSceneManager.ReturnScene == GameScene.None)
                {
                    _cutSceneManager.ReturnScene = scene;
                    _cutSceneManager.ReturnSceneTransitionSettings = transitionSettings;
                }
                GoToScene(GameScene.CutScene, transitionSettings);
                return;
            }

            _previousGameScene = _currentGameScene;
            _currentGameScene = scene;

            SceneTransitioner.TransitionSettings = transitionSettings;
            SceneTransitioner.LoadScene((int)_currentGameScene);
        }

        protected void Awake()
        {
            SceneTransitioner = GetComponent<SceneTransitioner>();
            _cutSceneManager = GameManager.Instance.CutScenes;
        }
    }
}