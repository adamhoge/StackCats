using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tofuwu.StackCats.Procedural
{
    public static class CatBlockContentsGenerator
    {
        public class PuzzleCatBlockContentsInfo
        {
            public PuzzleArea PuzzleArea { get; set; }
            public PuzzleMode PuzzleMode { get; set; }
            public List<Cat> CatsAlreadyInBlocks { get; set; } = new List<Cat>();
            public bool IsLuckyPuzzle { get; set; }
            public float LuckBonusForArea { get; set; }
        }

        public static CatBlockContents GenerateCatBlockContents(PuzzleCatBlockContentsInfo puzzleCatBlockContentsInfo)
        {
            CatBlockContents catBlockContents = new CatBlockContents();

            catBlockContents.Cat = GetCatForCatBlock(puzzleCatBlockContentsInfo);

            if (!catBlockContents.Cat)
            {
                catBlockContents.NumSilverPaws = GetSilverPawsForCatBlock(puzzleCatBlockContentsInfo.PuzzleMode, puzzleCatBlockContentsInfo.IsLuckyPuzzle);
            }

            return catBlockContents;
        }

        private static Cat GetCatForCatBlock(PuzzleCatBlockContentsInfo puzzleCatBlockContentsInfo)
        {
            if (puzzleCatBlockContentsInfo.CatsAlreadyInBlocks.Count == 5) return null;

            PuzzleArea puzzleArea = puzzleCatBlockContentsInfo.PuzzleArea;
            bool isLuckyPuzzle = puzzleCatBlockContentsInfo.IsLuckyPuzzle;

            List<Cat> cats = null;
            switch (puzzleCatBlockContentsInfo.PuzzleMode)
            {
                case PuzzleMode.Story:
                    cats = puzzleArea.ProgressionPuzzleCats.Except(puzzleCatBlockContentsInfo.CatsAlreadyInBlocks).ToList();
                    break;
                case PuzzleMode.Challenge:
                    cats = puzzleArea.GeneratedPuzzleCats.Except(puzzleCatBlockContentsInfo.CatsAlreadyInBlocks).ToList();
                    break;
                case PuzzleMode.Endless:
                    cats = puzzleArea.ProgressionPuzzleCats.Except(puzzleCatBlockContentsInfo.CatsAlreadyInBlocks).ToList();
                    break;
            }

            float rarityValue = UnityEngine.Random.value;
            if (puzzleCatBlockContentsInfo.IsLuckyPuzzle) rarityValue *= 1.25f;

            float rareRequirement = 0.996f;
            float uncommonRequirement = 0.96f;
            float commonRequirement = 0.8f;

            if (rarityValue >= rareRequirement)
            {
                List<Cat> rareCats = cats.Where(c => c.Rarity == Rarity.Rare).ToList();
                if (rareCats.Count > 0)
                {
                    return rareCats[UnityEngine.Random.Range(0, rareCats.Count)];
                }
            }

            if (rarityValue >= uncommonRequirement)
            {
                List<Cat> uncommonCats = cats.Where(c => c.Rarity == Rarity.Uncommon).ToList();
                if (uncommonCats.Count > 0)
                {
                    return uncommonCats[UnityEngine.Random.Range(0, uncommonCats.Count)];
                }
            }

            if (rarityValue >= commonRequirement)
            {
                List<Cat> commonCats = cats.Where(c => c.Rarity == Rarity.Common).ToList();
                if (commonCats.Count > 0)
                {
                    return commonCats[UnityEngine.Random.Range(0, commonCats.Count)];
                }
            }

            return null;
        }

        private static int GetSilverPawsForCatBlock(PuzzleMode puzzleMode, bool isLuckyPuzzle)
        {
            float randomValue = Random.value;
            if (puzzleMode == PuzzleMode.Challenge) randomValue *= 1.25f;
            if (isLuckyPuzzle) randomValue *= 1.25f;

            if (randomValue <= 0.2f) return 0;

            if (randomValue <= 0.8f) return 1;

            if (randomValue <= 1.1f) return 3;

            if (randomValue <= 1.5) return 5;

            return 100;
        }
    }
}
