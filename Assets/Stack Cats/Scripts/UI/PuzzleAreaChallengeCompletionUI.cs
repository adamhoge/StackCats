using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using RotaryHeart.Lib.SerializableDictionary;
using System;

namespace Tofuwu.StackCats
{
    [Serializable]
    public class TrophySpriteDictionary : SerializableDictionaryBase<ChallengeRunDifficulty, Sprite> { }

    public class PuzzleAreaChallengeCompletionUI : MonoBehaviour
    {
        public PuzzleArea PuzzleArea;
        public bool IsLocked;
        public Image LockedImage;
        public Image PuzzleAreaIconImage;
        public Image PuzzleAreaTrophyImage;
        public TrophySpriteDictionary TrophySprites;
        public Sprite TrophyPlaceholderSprite;

        private PuzzleManager _puzzleManager;

        protected void Awake()
        {
            _puzzleManager = GameManager.Instance.Puzzles;
        }

        public void Start()
        {
            InitializeAreaInfo();

            InitializeTrophyInfo();
        }

        private void InitializeAreaInfo()
        {
            if (!PuzzleArea || IsLocked)
            {
                PuzzleAreaIconImage.gameObject.SetActive(false);
                return;
            }

            PuzzleAreaIconImage.gameObject.SetActive(true);
            PuzzleAreaIconImage.sprite = PuzzleArea.PuzzleAreaGemSprite;
        }

        private void InitializeTrophyInfo()
        {
            if (IsLocked)
            {
                LockedImage.gameObject.SetActive(true);
                PuzzleAreaTrophyImage.gameObject.SetActive(false);
                return;
            }

            LockedImage.gameObject.SetActive(false);

            PuzzleAreaTrophyImage.gameObject.SetActive(true);
            ChallengeRunDifficulty? highestCompletionDifficulty = _puzzleManager.GetChallengeRunHighestDifficultyCompletion(PuzzleArea);
            if (highestCompletionDifficulty != null && TrophySprites.ContainsKey(highestCompletionDifficulty.Value))
            {
                PuzzleAreaTrophyImage.sprite = TrophySprites[highestCompletionDifficulty.Value];
            }
            else
            {
                PuzzleAreaTrophyImage.sprite = TrophyPlaceholderSprite;
            }
        }
    }
}