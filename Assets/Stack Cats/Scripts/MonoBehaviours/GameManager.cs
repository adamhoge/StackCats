using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Tofuwu.StackCats
{
    public delegate void ConfirmAction(string message, UnityAction action);

    [RequireComponent(typeof(GameSceneManager))]
    [RequireComponent(typeof(DataManager))]
    [RequireComponent(typeof(AudioManager))]
    [RequireComponent(typeof(EventManager))]
    [RequireComponent(typeof(CutSceneManager))]
    [RequireComponent(typeof(TutorialManager))]
    [RequireComponent(typeof(CatManager))]
    [RequireComponent(typeof(PuzzleManager))]
    [RequireComponent(typeof(CurrencyManager))]
    [RequireComponent(typeof(StuffManager))]
    [RequireComponent(typeof(HomeManager))]
    [RequireComponent(typeof(MinigameManager))]
    [RequireComponent(typeof(ChatCat))]
    public class GameManager : MonoBehaviour
    {
        public event ConfirmAction onConfirmAction;

        public SceneTransitionSettings StandardSceneTransitionSettings;

        /// <summary>
        /// An instance of the GameManager.
        /// </summary>
        public static GameManager Instance { get { return _instance; } }

        /// <summary>
        /// The current game scene.
        /// </summary>
        public GameScene CurrentGameScene { get { return _gameSceneManager.CurrentGameScene; } }

        /// <summary>
        /// The previous game scene.
        /// </summary>
        public GameScene PreviousGameScene { get { return _gameSceneManager.PreviousGameScene; } }

        /// <summary>
        /// Manages all saved game data.
        /// </summary>
        public DataManager Data { get { return _data; } }

        /// <summary>
        /// Manages game audio.
        /// </summary>
        public AudioManager Audio { get { return _audio; } }

        /// <summary>
        /// Manages in-game events.
        /// </summary>
        public EventManager Events { get { return _events; } }

        /// <summary>
        /// Manages all game scenes.
        /// </summary>
        public GameSceneManager GameScenes { get { return _gameSceneManager; } }

        /// <summary>
        /// Manages cut scene playback.
        /// </summary>
        public CutSceneManager CutScenes { get { return _cutScenes; } }

        /// <summary>
        /// Manages type-related tutorials.
        /// </summary>
        public TutorialManager TutorialManager { get { return _tutorialManager; } }

        /// <summary>
        /// Manages cat data.
        /// </summary>
        public CatManager Cats { get { return _cats; } }

        /// <summary>
        /// Manages standard puzzle data.
        /// </summary>
        public PuzzleManager Puzzles { get { return _puzzles; } }

        /// <summary>
        /// Manages game currency.
        /// </summary>
        public CurrencyManager Currency { get { return _currency; } }

        /// <summary>
        /// Manages presents, furniture, items, etc.
        /// </summary>
        public StuffManager Stuff { get { return _stuff; } }

        /// <summary>
        /// Manages the player's home (cat visitors, invited cats, decor, etc.)
        /// </summary>
        public HomeManager Home { get { return _home; } }

        /// <summary>
        /// Manages ads and ad-based rewards.
        /// </summary>
        public AdManager Ads { get { return _ads; } }

        /// <summary>
        /// Manages minigames and minigame-related information.
        /// </summary>
        public MinigameManager Minigames { get { return _minigames; } }

        /// <summary>
        /// Chat Cat companion.
        /// </summary>
        public ChatCat ChatCat { get { return _chatCat; } }

        /// <summary>
        /// Get the current time of day.
        /// </summary>
        public TimeOfDay TimeOfDay { get { return GetTimeOfDay(); } }

        private static GameManager _instance;
        private GameSceneManager _gameSceneManager;
        private DataManager _data;
        private AudioManager _audio;
        private EventManager _events;
        private CutSceneManager _cutScenes;
        private TutorialManager _tutorialManager;
        private CatManager _cats;
        private PuzzleManager _puzzles;
        private CurrencyManager _currency;
        private StuffManager _stuff;
        private HomeManager _home;
        private AdManager _ads;
        private MinigameManager _minigames;
        private ChatCat _chatCat;

        /// <summary>
        /// Go to the title screen.
        /// </summary>
        public void GoToTitleScreen()
        {
            _gameSceneManager.GoToScene(GameScene.Title, StandardSceneTransitionSettings);
        }

        /// <summary>
        /// Go to the home screen.
        /// </summary>
        public void GoHome()
        {
            SceneTransitionSettings transitionSettings = StandardSceneTransitionSettings;
            GameScene currentScene = _gameSceneManager.CurrentGameScene;
            if (currentScene == GameScene.Tutorial || currentScene == GameScene.Title)
            {
                transitionSettings.TransitionColor = Color.white;
                transitionSettings.TransitionOutDuration = 1.0f;
                transitionSettings.TransitionInDuration = 0.75f;
            }
            else if (currentScene == GameScene.Cats || currentScene == GameScene.ReplayCutScenes || currentScene == GameScene.OpenPresents)
            {
                transitionSettings.ShouldFadeBackgroundMusic = false;
            }

            _gameSceneManager.GoToScene(GameScene.Home, transitionSettings);
        }

        /// <summary>
        /// Go to the tutorial scene.
        /// </summary>
        public void GoToTutorial()
        {
            SceneTransitionSettings transitionSettings = StandardSceneTransitionSettings;
            if (_gameSceneManager.PreviousGameScene == GameScene.Title)
            {
                transitionSettings.TransitionColor = Color.white;
                transitionSettings.TransitionOutDuration = 1.5f;
                transitionSettings.TransitionInDuration = 1.0f;
            }

            _gameSceneManager.GoToScene(GameScene.Tutorial, transitionSettings);
        }

        /// <summary>
        /// Go to the main map.
        /// </summary>
        public void GoToMap()
        {
            SceneTransitionSettings transitionSettings = StandardSceneTransitionSettings;
            if (_gameSceneManager.CurrentGameScene == GameScene.ChallengeRun || _gameSceneManager.CurrentGameScene == GameScene.StoryPuzzle)
            {
                transitionSettings.ShouldFadeBackgroundMusic = false;
            }

            _gameSceneManager.GoToScene(GameScene.Map, transitionSettings);
        }


        /// <summary>
        /// Play the specified puzzle.
        /// </summary>
        public void PlayStoryPuzzle(PuzzleArea puzzleArea, StoryPuzzle puzzle)
        {
            _puzzles.CurrentMode = PuzzleMode.Story;
            _puzzles.CurrentArea = puzzleArea;
            _puzzles.CurrentStoryPuzzle = puzzle;

            SceneTransitionSettings transitionSettings = StandardSceneTransitionSettings;
            transitionSettings.ShouldFadeBackgroundMusic = false;

            _gameSceneManager.GoToScene(GameScene.StoryPuzzle, transitionSettings);
        }

        /// <summary>
        /// Play challenge puzzles in the specified area.
        /// </summary>
        /// <param name="puzzleArea">The challenge puzzles area.</param>
        public void PlayChallengeMode(PuzzleArea puzzleArea)
        {
            _puzzles.CurrentMode = PuzzleMode.Challenge;
            _puzzles.CurrentArea = puzzleArea;

            SceneTransitionSettings transitionSettings = StandardSceneTransitionSettings;
            transitionSettings.ShouldFadeBackgroundMusic = false;

            _gameSceneManager.GoToScene(GameScene.ChallengeRun, transitionSettings);
        }

        /// <summary>
        /// Play endless puzzles.
        /// </summary>
        public void PlayEndlessMode(PuzzleArea puzzleArea)
        {
            _puzzles.CurrentMode = PuzzleMode.Endless;
            _puzzles.CurrentArea = puzzleArea;

            _gameSceneManager.GoToScene(GameScene.EndlessRun, StandardSceneTransitionSettings);
        }

        /// <summary>
        /// View cats.
        /// </summary>
        public void ViewCats()
        {
            SceneTransitionSettings transitionSettings = StandardSceneTransitionSettings;
            transitionSettings.ShouldFadeBackgroundMusic = false;

            _gameSceneManager.GoToScene(GameScene.Cats, transitionSettings);
        }

        /// <summary>
        /// Shop.
        /// </summary>
        public void Shop()
        {
            _gameSceneManager.GoToScene(GameScene.Shop, StandardSceneTransitionSettings);
        }

        /// <summary>
        /// Open presents.
        /// </summary>
        public void OpenPresents()
        {
            SceneTransitionSettings transitionSettings = StandardSceneTransitionSettings;
            transitionSettings.ShouldFadeBackgroundMusic = false;

            _gameSceneManager.GoToScene(GameScene.OpenPresents, transitionSettings);
        }

        /// <summary>
        /// Play the provided minigame
        /// </summary>
        public void PlayMinigame(MinigameInformation minigame, List<Cat> minigameCats)
        {
            _minigames.CurrentMinigame = minigame;
            _minigames.MinigameCats = minigameCats;

            _gameSceneManager.GoToScene(GameScene.Minigame, StandardSceneTransitionSettings);
        }

        /// <summary>
        /// Go to the puzzle maker.
        /// </summary>
        public void GoToPuzzleMaker()
        {
            _gameSceneManager.GoToScene(GameScene.PuzzleMaker, StandardSceneTransitionSettings);
        }

        /// <summary>
        /// Play the currently enqueued cut scene.
        /// </summary>
        public void PlayCutScene()
        {
            SceneTransitionSettings transitionSettings = StandardSceneTransitionSettings;
            transitionSettings.ShouldFadeBackgroundMusic = false;
            if (_gameSceneManager.PreviousGameScene != GameScene.ReplayCutScenes)
            {
                transitionSettings.TransitionOutDuration = 1.0f;
                transitionSettings.TransitionInDuration = 0.75f;
            }

            _gameSceneManager.GoToScene(GameScene.CutScene, transitionSettings);
        }

        /// <summary>
        /// Browse and replay cut scenes.
        /// </summary>
        public void ReplayCutScenes()
        {
            SceneTransitionSettings transitionSettings = StandardSceneTransitionSettings;
            transitionSettings.ShouldFadeBackgroundMusic = false;

            _gameSceneManager.GoToScene(GameScene.ReplayCutScenes, transitionSettings);
        }

        public void ConfirmAction(string message, UnityAction action = null)
        {
            if (onConfirmAction != null) onConfirmAction(message, action);
        }

        protected void Awake()
        {
            if (!_instance)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                _gameSceneManager = GetComponent<GameSceneManager>();
                _data = GetComponent<DataManager>();
                _audio = GetComponent<AudioManager>();
                _events = GetComponent<EventManager>();
                _cutScenes = GetComponent<CutSceneManager>();
                _tutorialManager = GetComponent<TutorialManager>();
                _cats = GetComponent<CatManager>();
                _puzzles = GetComponent<PuzzleManager>();
                _currency = GetComponent<CurrencyManager>();
                _stuff = GetComponent<StuffManager>();
                _home = GetComponent<HomeManager>();
                _ads = GetComponent<AdManager>();
                _minigames = GetComponent<MinigameManager>();
                _chatCat = GetComponent<ChatCat>();

                Application.targetFrameRate = 60;
#if UNITY_STANDALONE_WIN
                Screen.SetResolution(450, 800, false);
#endif
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private TimeOfDay GetTimeOfDay()
        {
            TimeOfDay timeOfDay;

            int currentHour = DateTime.Now.Hour;
            if (currentHour >= 19 || currentHour < 6)
            {
                timeOfDay = TimeOfDay.Night;
            }
            else if (currentHour >= 17)
            {
                timeOfDay = TimeOfDay.Morning;
            }
            else if (currentHour >= 11)
            {
                timeOfDay = TimeOfDay.Day;
            }
            else
            {
                timeOfDay = TimeOfDay.Evening;
            }

            return timeOfDay;
        }
    }
}