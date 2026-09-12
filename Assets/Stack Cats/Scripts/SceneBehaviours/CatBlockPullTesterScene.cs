using Tofuwu.StackCats.Procedural;
using System.Collections.Generic;
using UnityEngine;
using static Tofuwu.StackCats.Procedural.CatBlockContentsGenerator;

namespace Tofuwu.StackCats
{
    public class CatBlockPullTesterScene : MonoBehaviour
    {
        public delegate void Pulled(List<CatBlockContents> catBlockContentsList);
        public delegate void TotalsChanged(int totalNumPulls, int numCommonCats, int numUncommonCats, int numRareCats, int numSilverPaws);

        public event Pulled onPulled;
        public event TotalsChanged onTotalsChanged;

        [Range(1, 1000)]
        public int NumBoxesToPull = 10;
        [Range(1, 2)]
        public float LuckBonusForArea = 1.0f;
        public PuzzleAreaType PuzzleAreaType;
        public PuzzleMode PuzzleMode;
        public bool IsLuckyPuzzle;
        public PuzzleArea FarmFlavoredPuzzleArea;
        public PuzzleArea JungleFlavoredPuzzleArea;
        public PuzzleArea DesertFlavortedPuzzleArea;
        public PuzzleArea NightFlavoredPuzzleArea;
        public bool AutoPull = true;
        public float AutoPullIntervalInSeconds = 0.5f;

        private float _lastAutoPullTime;
        private int _totalNumPulls;
        private int _totalNumCommonCats;
        private int _totalNumUncommonCats;
        private int _totalNumRareCats;
        private int _totalNumSilverPaws;

        public void Update()
        {
            if(AutoPull && Time.time > _lastAutoPullTime + AutoPullIntervalInSeconds)
            {
                Pull();
                _lastAutoPullTime = Time.time;
            }
        }

        public void Pull()
        {
            List<CatBlockContents> allPulls = new List<CatBlockContents>();
            PuzzleCatBlockContentsInfo info = new PuzzleCatBlockContentsInfo
            {
                PuzzleArea = GetPuzzleAreaForType(PuzzleAreaType),
                PuzzleMode = PuzzleMode,
                IsLuckyPuzzle = IsLuckyPuzzle
            };
            for (int i = 0; i < NumBoxesToPull; i++)
            {
                CatBlockContents contents = CatBlockContentsGenerator.GenerateCatBlockContents(info);
                allPulls.Add(contents);

                if (contents.Cat)
                {
                    info.CatsAlreadyInBlocks.Add(contents.Cat);

                    switch (contents.Cat.Rarity)
                    {
                        case Rarity.Common:
                            ++_totalNumCommonCats;
                            break;
                        case Rarity.Uncommon:
                            ++_totalNumUncommonCats;
                            break;
                        case Rarity.Rare:
                            ++_totalNumRareCats;
                            break;
                    }
                }

                _totalNumSilverPaws += contents.NumSilverPaws;
            }

            _totalNumPulls += NumBoxesToPull;

            if (onPulled != null)
            {
                onPulled(allPulls);
            }

            if(onTotalsChanged != null)
            {
                onTotalsChanged(_totalNumPulls, _totalNumCommonCats, _totalNumUncommonCats, _totalNumRareCats, _totalNumSilverPaws);
            }
        }

        public void ResetTotals()
        {
            _totalNumPulls = 0;
            _totalNumCommonCats = 0;
            _totalNumUncommonCats = 0;
            _totalNumRareCats = 0;
            _totalNumSilverPaws = 0;

            if (onTotalsChanged != null)
            {
                onTotalsChanged(_totalNumPulls, _totalNumCommonCats, _totalNumUncommonCats, _totalNumRareCats, _totalNumSilverPaws);
            }
        }

        private PuzzleArea GetPuzzleAreaForType(PuzzleAreaType puzzleAreaType)
        {
            switch (puzzleAreaType)
            {
                case PuzzleAreaType.Farm: return FarmFlavoredPuzzleArea;
                case PuzzleAreaType.Jungle: return JungleFlavoredPuzzleArea;
                case PuzzleAreaType.Desert: return DesertFlavortedPuzzleArea;
                case PuzzleAreaType.Night: return NightFlavoredPuzzleArea;
            }

            return null;
        }
    }
}