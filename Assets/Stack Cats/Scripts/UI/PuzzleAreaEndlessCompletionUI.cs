using TMPro;
using Tofuwu.StackCats.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats
{
    public class PuzzleAreaEndlessCompletionUI : MonoBehaviour
    {
        public PuzzleArea PuzzleArea;
        public bool IsLocked;
        public Image LockedImage;
        public Image BannerImage;
        public Image MedalPlaceholderImage;
        public MedalUI Medal;
        public Image PuzzleAreaIconImage;

        private PuzzleManager _puzzleManager;

        protected void Awake()
        {
            _puzzleManager = GameManager.Instance.Puzzles;
        }

        public void Start()
        {
            InitializeAreaInfo();
            InitializeMedalInfo();
        }

        private void InitializeAreaInfo()
        {
            if (!PuzzleArea || IsLocked)
            {
                BannerImage.color = Color.gray;
                PuzzleAreaIconImage.gameObject.SetActive(false);
                return;
            }

            BannerImage.gameObject.SetActive(true);
            BannerImage.color = PuzzleArea.PuzzleTheme.UIColor;
            PuzzleAreaIconImage.gameObject.SetActive(true);
            PuzzleAreaIconImage.sprite = PuzzleArea.PuzzleAreaIconSprite;
        }

        private void InitializeMedalInfo()
        {
            if (IsLocked)
            {
                BannerImage.color = new Color(BannerImage.color.r, BannerImage.color.g, BannerImage.color.b, 0.25f);
                MedalPlaceholderImage.gameObject.SetActive(false);
                Medal.gameObject.SetActive(false);
                LockedImage.gameObject.SetActive(true);
                return;
            }

            LockedImage.gameObject.SetActive(false);

            int mostPuzzlesCompleted = _puzzleManager.GetEndlessRunMostPuzzlesCompleted(PuzzleArea);
            if (mostPuzzlesCompleted == 0)
            {
                MedalPlaceholderImage.gameObject.SetActive(true);
                Medal.gameObject.SetActive(false);
                return;
            }

            MedalPlaceholderImage.gameObject.SetActive(false);
            Medal.gameObject.SetActive(true);
            Medal.NumPuzzlesCompleted = mostPuzzlesCompleted;
        }
    }
}