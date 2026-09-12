using UnityEngine;

namespace Tofuwu.StackCats.UI
{
    public class EndlessAreaSelectionOverlayScreen : OverlayScreen
    {
        public HomeScene HomeScene;
        public RectTransform EndlessAreasRectTransform;
        public EndlessAreaInfoUI EndlessAreaInfoPrefab;

        private PuzzleManager _puzzleManager;

        protected void Awake()
        {
            _puzzleManager = GameManager.Instance.Puzzles;
        }

        protected void Start()
        {
            foreach (PuzzleArea puzzleArea in _puzzleManager.PuzzleAreaCollection.List)
            {
                EndlessAreaInfoUI endlessAreaInfoInstance = Instantiate(EndlessAreaInfoPrefab, EndlessAreasRectTransform);
                endlessAreaInfoInstance.PuzzleArea = puzzleArea;
                endlessAreaInfoInstance.IsLocked = _puzzleManager.IsEndlessModeLocked(puzzleArea);
                endlessAreaInfoInstance.Button.onClick.AddListener(delegate () { HomeScene.PlayEndlessMode(endlessAreaInfoInstance.PuzzleArea); });
            }
        }
    }
}