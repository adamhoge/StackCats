using TMPro;
using UnityEngine;

namespace Tofuwu.StackCats.UI
{
    public class ChallengeAreaSelectionOverlayScreen : OverlayScreen
    {
        public HomeScene HomeScene;
        public RectTransform ChallengeAreasRectTransform;
        public ChallengeAreaInfoUI ChallengeAreaInfoPrefab;

        private PuzzleManager _puzzleManager;

        protected void Awake()
        {
            _puzzleManager = GameManager.Instance.Puzzles;
        }

        protected void Start()
        {
            foreach (PuzzleArea puzzleArea in _puzzleManager.PuzzleAreaCollection.List)
            {
                ChallengeAreaInfoUI challengeAreaInfoInstance = Instantiate(ChallengeAreaInfoPrefab, ChallengeAreasRectTransform);
                challengeAreaInfoInstance.PuzzleArea = puzzleArea;
                challengeAreaInfoInstance.IsLocked = _puzzleManager.IsChallengeModeLocked(puzzleArea);
                challengeAreaInfoInstance.Button.onClick.AddListener(delegate () { HomeScene.PlayChallengeMode(challengeAreaInfoInstance.PuzzleArea); });
            }
        }
    }
}