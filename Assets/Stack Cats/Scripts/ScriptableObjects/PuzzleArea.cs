using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

namespace Tofuwu.StackCats
{
    [Serializable]
    public class ChallengePuzzleReward
    {
        public ChallengeRunDifficulty AtDifficulty;
        public int AtPuzzleIndex;
        public CurrencyAmountDictionary CurrencyReward;
    }

    [Serializable]
    public class DifficultyTrophyDictionary : SerializableDictionaryBase<ChallengeRunDifficulty, TrophyItem> { }

    [CreateAssetMenu(fileName = "Puzzle Area", menuName = "Stack Cats/Puzzle Area")]
    public class PuzzleArea : ScriptableObject, IIdentifiable
    {
        public string AreaTitle;

        public PuzzleTheme PuzzleTheme;

        public Sprite PuzzleAreaIconSprite;

        public Sprite PuzzleAreaGemSprite;

        public Puzzle PuzzlePrefab;

        public PuzzleAreaMap PuzzleAreaMap;

        public List<Cat> ProgressionPuzzleCats;

        public List<Cat> GeneratedPuzzleCats;

        public int ChallengeRunCost;

        public List<ChallengePuzzleReward> ChallengeRunRewards;

        public DifficultyTrophyDictionary ChallengeRunTrophies;

        [SerializeField]
        [HideInInspector]
        private string _id;

        public string GetId()
        {
            if (string.IsNullOrEmpty(_id)) { _id = Guid.NewGuid().ToString(); }

            return _id;
        }
    }
}