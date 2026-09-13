using UnityEngine;

namespace Tofuwu.StackCats
{
    public delegate void SelectPlayMode();
    public delegate void SelectChallengeArea();
    public delegate void SelectEndlessArea();

    [RequireComponent(typeof(HomeVisitors))]
    public class HomeScene : SceneBehaviour
    {
        public event SelectPlayMode onSelectPlayMode;
        public event SelectChallengeArea onSelectChallengeArea;
        public event SelectEndlessArea onSelectEndlessArea;

        public Camera Camera;
        public Transform HomeScenaryTransform;
        private const float CAMERA_SIZE_MAX = 4.65f;
        private const float CAMERA_SIZE_PREFERRED = 3.85f;

        private PuzzleManager _puzzleManager;
        private HomeVisitors _homeVisitors;
        private float _lastAspect;

        public void Play()
        {
            if (_puzzleManager.IsChallengeModeLocked())
            {
                _gameManager.GoToMap();
            }
            else
            {
                if (onSelectPlayMode != null)
                    onSelectPlayMode();
            }
        }

        public void SelectChallengeArea()
        {
            if (onSelectChallengeArea != null)
                onSelectChallengeArea();
        }

        public void SelectEndlessArea()
        {
            if (onSelectEndlessArea != null)
                onSelectEndlessArea();
        }

        public void PlayStoryMode()
        {
            _gameManager.GoToMap();
        }

        public void PlayChallengeMode(PuzzleArea puzzleArea)
        {
            _gameManager.PlayChallengeMode(puzzleArea);
        }

        public void PlayEndlessMode(PuzzleArea puzzleArea)
        {
            _gameManager.PlayEndlessMode(puzzleArea);
        }

        public void ViewCats()
        {
            _gameManager.ViewCats();
        }

        public void Shop()
        {
            _gameManager.Shop();
        }

        public void OpenPresents()
        {
            _gameManager.OpenPresents();
        }

        public void MakePuzzles()
        {
            _gameManager.GoToPuzzleMaker();
        }

        public void ReplayCutScenes()
        {
            _gameManager.ReplayCutScenes();
        }

        public void ReplayTutorial()
        {
            _gameManager.GoToTutorial();
        }

        public void PlayMinigame(MinigameInformation minigame)
        {
            _gameManager.PlayMinigame(minigame, _homeVisitors.GetVisitors());
        }

        protected override void Awake()
        {
            base.Awake();

            _puzzleManager = _gameManager.Puzzles;
            _homeVisitors = GetComponent<HomeVisitors>();
        }

        protected override void Start()
        {
            base.Start();

            LoadWindowScenary();
        }

        protected void Update()
        {
            float aspect = Camera.aspect;
            if (_lastAspect != aspect)
            {
                UpdateCameraAspect(aspect);
                _lastAspect = aspect;
            }
        }

        private void LoadWindowScenary()
        {
            PuzzleArea currentPuzzleArea = _puzzleManager.CurrentArea;
            Instantiate(currentPuzzleArea.PuzzleTheme.HomeScenaryPrefab, HomeScenaryTransform);
        }

        private void UpdateCameraAspect(float aspect)
        {
            float clampedAspect = Mathf.Clamp(
                aspect,
                Constants.MinAspect,
                Constants.PreferredAspect
            );
            float clampedAspectNormal =
                (clampedAspect - Constants.MinAspect)
                / (Constants.PreferredAspect - Constants.MinAspect);
            Camera.orthographicSize =
                CAMERA_SIZE_PREFERRED
                + (CAMERA_SIZE_MAX - CAMERA_SIZE_PREFERRED) * (1 - clampedAspectNormal);
        }
    }
}
