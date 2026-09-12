using UnityEngine;

namespace Tofuwu.StackCats.UI
{
    public class PuzzleManagerUI : MonoBehaviour
    {
        public PuzzleAreaUnlockedBanner PuzzleAreaUnlockedBanner;
        public ChallengeModePuzzleAreaUnlockedBanner ChallengeModePuzzleAreaUnlockedBanner;
        public EndlessModePuzzleAreaUnlockedBanner EndlessModePuzzleAreaUnlockedBanner;

        private PuzzleManager _puzzleManager;

        protected void Awake()
        {
            _puzzleManager = GameManager.Instance.Puzzles;
        }

        protected void OnEnable()
        {
            _puzzleManager.onPuzzleAreaUnlocked += OnPuzzleAreaUnlocked;
            _puzzleManager.onChallengeModePuzzleAreaUnlocked += OnChallengeModePuzzleAreaUnlocked;
            _puzzleManager.onEndlessModePuzzleAreaUnlocked += OnEndlessModePuzzleAreaUnlocked;
        }

        protected void OnDisable()
        {
            _puzzleManager.onPuzzleAreaUnlocked -= OnPuzzleAreaUnlocked;
            _puzzleManager.onChallengeModePuzzleAreaUnlocked -= OnChallengeModePuzzleAreaUnlocked;
            _puzzleManager.onEndlessModePuzzleAreaUnlocked -= OnEndlessModePuzzleAreaUnlocked;
        }

        private void OnPuzzleAreaUnlocked(PuzzleArea puzzleArea)
        {
            PuzzleAreaUnlockedBanner banner = Instantiate(PuzzleAreaUnlockedBanner);
            banner.PuzzleArea = puzzleArea;
            GameManagerUI.Instance.BannerManagerUI.DisplayBanner(banner);
        }

        private void OnChallengeModePuzzleAreaUnlocked(PuzzleArea puzzleArea)
        {
            ChallengeModePuzzleAreaUnlockedBanner banner = Instantiate(ChallengeModePuzzleAreaUnlockedBanner);
            banner.PuzzleArea = puzzleArea;
            GameManagerUI.Instance.BannerManagerUI.DisplayBanner(banner);
        }

        private void OnEndlessModePuzzleAreaUnlocked(PuzzleArea puzzleArea)
        {
            EndlessModePuzzleAreaUnlockedBanner banner = Instantiate(EndlessModePuzzleAreaUnlockedBanner);
            banner.PuzzleArea = puzzleArea;
            GameManagerUI.Instance.BannerManagerUI.DisplayBanner(banner);
        }
    }
}