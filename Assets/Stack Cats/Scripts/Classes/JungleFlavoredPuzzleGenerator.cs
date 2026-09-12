using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using Tofuwu.StackCats.Models;

namespace Tofuwu.StackCats.Procedural
{
    public class JungleFlavoredPuzzleGenerator : PuzzleGenerator<JungleFlavoredPuzzleArea, JungleFlavoredPuzzle>
    {
        public JungleFlavoredPuzzleGenerator(JungleFlavoredPuzzleArea puzzleArea) : base(puzzleArea) { }

        private IMoveStrategy<JungleFlavoredPuzzleArea, JungleFlavoredPuzzle> _moveOrCoverJigsawPieceStrategy;

        public override Stack GetPreferredSourceStack(JungleFlavoredPuzzle puzzle, List<Stack> usableStacks)
        {
            throw new System.NotImplementedException();
        }

        public override Stack GetPreferredDestinationStack(JungleFlavoredPuzzle puzzle, List<Stack> usableStacks)
        {
            List<Stack> excludeStacks = new List<Stack>();
            foreach (Stack stack in usableStacks)
            {
                foreach (Block block in stack.Blocks)
                {
                    JigsawBlock jigsawBlock = block.GetComponent<JigsawBlock>();
                    if (jigsawBlock && jigsawBlock.IsComplete)
                    {
                        excludeStacks.Add(stack);
                        break;
                    }
                }
            }

            // TODO: Come up with a pattern to ensure this doesn't happen.
            if (excludeStacks.Count == usableStacks.Count) return usableStacks.SelectRandom();

            usableStacks = usableStacks.Except(excludeStacks).ToList();

            int minMovableBlocks = usableStacks.Min(s => puzzle.GetNumMovableBlocksInStack(s));
            return usableStacks.First(s => puzzle.GetNumMovableBlocksInStack(s) == minMovableBlocks);
        }

        protected override IMoveStrategy<JungleFlavoredPuzzleArea, JungleFlavoredPuzzle> GetMoveStrategy(
            Dictionary<IMoveStrategy<JungleFlavoredPuzzleArea, JungleFlavoredPuzzle>, float> moveStrategies,
            GeneratedPuzzleInfo<JungleFlavoredPuzzleArea, JungleFlavoredPuzzle> generatedPuzzleInfo)
        {
            if (generatedPuzzleInfo.NumMovesMade < 2)
            {
                return _moveOrCoverJigsawPieceStrategy;
            }
            else
            {
                return base.GetMoveStrategy(moveStrategies, generatedPuzzleInfo);
            }
        }

        protected override Dictionary<IMoveStrategy<JungleFlavoredPuzzleArea, JungleFlavoredPuzzle>, float> GetMoveStrategies(int difficulty)
        {
            Dictionary<IMoveStrategy<JungleFlavoredPuzzleArea, JungleFlavoredPuzzle>, float> moveStrategies = new Dictionary<IMoveStrategy<JungleFlavoredPuzzleArea, JungleFlavoredPuzzle>, float>();

            _moveOrCoverJigsawPieceStrategy = new MoveOrCoverJigsawPieceStrategy<JungleFlavoredPuzzleArea, JungleFlavoredPuzzle>();
            moveStrategies.Add(_moveOrCoverJigsawPieceStrategy, 0.0f);

            var moveBlocksStrategy = new MoveBlocksStrategy<JungleFlavoredPuzzleArea, JungleFlavoredPuzzle>();
            moveStrategies.Add(moveBlocksStrategy, 0.0f);

            int maxSumBlocks = Mathf.CeilToInt((difficulty - 25.0f) / (MAX_DIFFICULTY - 25.0f) * 4);
            var addSumBlockStrategy = new AddSumBlockStrategy<JungleFlavoredPuzzleArea, JungleFlavoredPuzzle>(maxSumBlocks);
            moveStrategies.Add(addSumBlockStrategy, 0.0f);

            int maxWildBlocks = Mathf.CeilToInt((difficulty - 50.0f) / (MAX_DIFFICULTY - 50.0f) * 4);
            var addWildBlockStategy = new AddWildBlockStrategy<JungleFlavoredPuzzleArea, JungleFlavoredPuzzle>(maxWildBlocks);

            moveStrategies.Add(addWildBlockStategy, 0.0f); float strategyPercentageRemaining = 1.0f;

            if (difficulty > 80)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[addSumBlockStrategy] += usedPercent / 2;
                moveStrategies[addWildBlockStategy] += usedPercent / 2;
            }

            if (difficulty > 70)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[moveBlocksStrategy] += usedPercent / 8;
                moveStrategies[_moveOrCoverJigsawPieceStrategy] += usedPercent / 8;
                moveStrategies[addSumBlockStrategy] += usedPercent / 2;
                moveStrategies[addWildBlockStategy] += usedPercent / 4;
            }

            if (difficulty > 60)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[moveBlocksStrategy] += usedPercent / 4;
                moveStrategies[_moveOrCoverJigsawPieceStrategy] += usedPercent / 4;
                moveStrategies[addSumBlockStrategy] += usedPercent / 4;
                moveStrategies[addWildBlockStategy] += usedPercent / 4;
            }

            if (difficulty > 50)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[addSumBlockStrategy] += usedPercent / 2;
                moveStrategies[addWildBlockStategy] += usedPercent / 2;
            }

            if (difficulty > 40)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[moveBlocksStrategy] += usedPercent / 4;
                moveStrategies[_moveOrCoverJigsawPieceStrategy] += usedPercent / 4;
                moveStrategies[addSumBlockStrategy] += usedPercent / 2;
            }

            if (difficulty > 30)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[moveBlocksStrategy] += usedPercent / 2;
                moveStrategies[_moveOrCoverJigsawPieceStrategy] += usedPercent / 2;
            }

            moveStrategies[moveBlocksStrategy] += strategyPercentageRemaining;

            return moveStrategies;
        }

        protected override void CreateFinishedPuzzle(GeneratedPuzzleInfo<JungleFlavoredPuzzleArea, JungleFlavoredPuzzle> generatedPuzzleInfo)
        {
            #region LoadFromString
            //if (generatedPuzzleInfo.ObjectiveDifficulty >= 70)
            //{
            //    generatedPuzzleInfo.NumStacks = 6;
            //    generatedPuzzleInfo.MaxBlocks = 10;
            //}
            //else
            //{
            //    generatedPuzzleInfo.NumStacks = 5;
            //    generatedPuzzleInfo.MaxBlocks = 8;
            //}
            //string puzzleJsonData = "{ \"_stacks\":[{\"_blocks\":[{\"_hasPuzzleBlock\":false,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":true,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":0,\"_jigsawPuzzleObjectId\":\"5c46792d-18c4-48aa-80a2-8b26f7e69397\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":false,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":true,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":1,\"_jigsawPuzzleObjectId\":\"5c46792d-18c4-48aa-80a2-8b26f7e69397\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":false,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":true,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":2,\"_jigsawPuzzleObjectId\":\"5c46792d-18c4-48aa-80a2-8b26f7e69397\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":false,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":true,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":0,\"_jigsawPuzzleObjectId\":\"15415dba-dfd9-4da3-9a99-0a7dad77d225\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":false,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":true,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":1,\"_jigsawPuzzleObjectId\":\"15415dba-dfd9-4da3-9a99-0a7dad77d225\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":false,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":true,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":0,\"_jigsawPuzzleObjectId\":\"e10741ae-168a-4cf7-a544-528eb692ff4d\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":false,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":true,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":1,\"_jigsawPuzzleObjectId\":\"e10741ae-168a-4cf7-a544-528eb692ff4d\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":false,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":true,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":2,\"_jigsawPuzzleObjectId\":\"e10741ae-168a-4cf7-a544-528eb692ff4d\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}}]},{\"_blocks\":[{\"_hasPuzzleBlock\":false,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":true,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":0,\"_jigsawPuzzleObjectId\":\"f7833c3a-d4ec-4fa3-868b-e1fadb48222b\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":false,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":true,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":1,\"_jigsawPuzzleObjectId\":\"f7833c3a-d4ec-4fa3-868b-e1fadb48222b\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":false,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":true,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":2,\"_jigsawPuzzleObjectId\":\"f7833c3a-d4ec-4fa3-868b-e1fadb48222b\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":false,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":true,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":3,\"_jigsawPuzzleObjectId\":\"f7833c3a-d4ec-4fa3-868b-e1fadb48222b\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":false,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":true,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":0,\"_jigsawPuzzleObjectId\":\"935ddb84-8658-40a1-948e-aa9062cb50a9\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":false,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":true,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":1,\"_jigsawPuzzleObjectId\":\"935ddb84-8658-40a1-948e-aa9062cb50a9\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":false,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":true,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":2,\"_jigsawPuzzleObjectId\":\"935ddb84-8658-40a1-948e-aa9062cb50a9\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":false,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":true,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":3,\"_jigsawPuzzleObjectId\":\"935ddb84-8658-40a1-948e-aa9062cb50a9\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}}]},{\"_blocks\":[{\"_hasPuzzleBlock\":true,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":false,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":2,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":0,\"_jigsawPuzzleObjectId\":\"\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}}]},{\"_blocks\":[{\"_hasPuzzleBlock\":true,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":false,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":8,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":0,\"_jigsawPuzzleObjectId\":\"\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":true,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":false,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":9,\"_secondaryNumber\":1},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":0,\"_jigsawPuzzleObjectId\":\"\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":true,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":false,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":8,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":0,\"_jigsawPuzzleObjectId\":\"\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":true,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":false,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":9,\"_secondaryNumber\":1},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":0,\"_jigsawPuzzleObjectId\":\"\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":true,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":false,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":8,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":0,\"_jigsawPuzzleObjectId\":\"\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":true,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":false,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":9,\"_secondaryNumber\":1},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":0,\"_jigsawPuzzleObjectId\":\"\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":true,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":false,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":8,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":0,\"_jigsawPuzzleObjectId\":\"\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":true,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":false,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":7,\"_secondaryNumber\":1},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":0,\"_jigsawPuzzleObjectId\":\"\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}}]},{\"_blocks\":[{\"_hasPuzzleBlock\":true,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":false,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":8,\"_secondaryNumber\":1},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":0,\"_jigsawPuzzleObjectId\":\"\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":false,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":true,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":0,\"_jigsawPuzzleObjectId\":\"f7833c3a-d4ec-4fa3-868b-e1fadb48222b\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":false,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":true,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":1,\"_jigsawPuzzleObjectId\":\"f7833c3a-d4ec-4fa3-868b-e1fadb48222b\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":false,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":true,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":2,\"_jigsawPuzzleObjectId\":\"f7833c3a-d4ec-4fa3-868b-e1fadb48222b\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":false,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":true,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":3,\"_jigsawPuzzleObjectId\":\"f7833c3a-d4ec-4fa3-868b-e1fadb48222b\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":true,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":false,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":8,\"_secondaryNumber\":1},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":0,\"_jigsawPuzzleObjectId\":\"\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":true,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":false,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":9,\"_secondaryNumber\":0},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":0,\"_jigsawPuzzleObjectId\":\"\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}},{\"_hasPuzzleBlock\":true,\"_hasCatBlock\":false,\"_hasSumBlock\":false,\"_hasJigsawBlock\":false,\"_hasGalaxyBlock\":false,\"_puzzleBlock\":{\"_primaryNumber\":8,\"_secondaryNumber\":1},\"_catBlock\":{\"_catId\":\"\",\"_yarn\":0,\"_isSpecial\":false},\"_sumBlock\":{\"_sumValue\":0},\"_jigsawBlock\":{\"_jigsawIndex\":0,\"_jigsawPuzzleObjectId\":\"\"},\"_galaxyBlock\":{\"_primaryNumber\":0,\"_secondaryNumber\":0}}]}],\"_maxStackHeight\":8,\"_numMovesMade\":0,\"_isSpecial\":false}";
            //generatedPuzzleInfo.Puzzle = JungleFlavoredPuzzleBuilder.BuildFromModel(JsonUtility.FromJson<JungleFlavoredPuzzleModel>(puzzleJsonData), (JungleFlavoredPuzzleArea)generatedPuzzleInfo.PuzzleArea, GameManager.Instance.Cats);
            //generatedPuzzleInfo.Puzzle.IsEditMode = true;
            //return;
            #endregion

            float difficulty = generatedPuzzleInfo.ObjectiveDifficulty;
            JungleFlavoredPuzzle puzzle = generatedPuzzleInfo.Puzzle;

            if (generatedPuzzleInfo.ObjectiveDifficulty >= 70)
            {
                generatedPuzzleInfo.NumStacks = 6;
                generatedPuzzleInfo.MaxBlocks = 10;
            }
            else
            {
                generatedPuzzleInfo.NumStacks = 5;
                generatedPuzzleInfo.MaxBlocks = 8;
            }

            for (int i = 0; i < generatedPuzzleInfo.NumStacks; i++)
            {
                Stack stack = puzzle.AddNewStack();
                stack.MaxBlocks = generatedPuzzleInfo.MaxBlocks;
            }
            puzzle.MaxStackHeight = generatedPuzzleInfo.MaxBlocks;

            int numJigsawBlocks = 4 + Mathf.FloorToInt(12.0f / 100.0f * difficulty);
            int numSumBlocks = Mathf.FloorToInt(4.0f / 100.0f * difficulty);
            int maxPuzzleBlocks = puzzle.MaxBlocks - numJigsawBlocks - numSumBlocks;
            int numPuzzleBlocks = (5 + Mathf.FloorToInt((maxPuzzleBlocks - 5) / 100.0f * difficulty)) / 2;

            // Designate lowest stack
            Stack lowStack = puzzle.Stacks[Random.Range(0, puzzle.Stacks.Count)];
            int numBlocks = Random.Range(0, 2);
            for (int i = 0; i < numBlocks; i++)
            {
                PuzzleBuilder.AddRandomPuzzleBlockToTop(puzzle, lowStack, 7, 9);
            }

            List<Stack> usableStacks = new List<Stack>(puzzle.Stacks);
            usableStacks.Remove(lowStack);
            bool jigsawStackRemoved = false;
            while (usableStacks.Count > 0 && (numJigsawBlocks > 0 || numPuzzleBlocks > 0))
            {
                Stack stack = usableStacks[Random.Range(0, usableStacks.Count)];
                int blockTypeNumber = Random.Range(0, 2);
                if (blockTypeNumber == 0 && puzzle.GetNumBlocksPlaceableOnStack(stack) >= 2) // Jigsaw object
                {
                    int jigsawBlocksAdded = AddRandomJigsawObjectBlocksToTop(generatedPuzzleInfo, stack, numJigsawBlocks);
                    numJigsawBlocks -= jigsawBlocksAdded;
                    if (numJigsawBlocks == 1) numJigsawBlocks = 0;
                    if(!jigsawStackRemoved)
                    {
                        usableStacks.Remove(stack);
                        jigsawStackRemoved = true;
                    }
                }
                else // Puzzle block
                {
                    int numPuzzleBlocksToPlace = Random.Range(1, 4);
                    if (numPuzzleBlocksToPlace > numPuzzleBlocks) numPuzzleBlocksToPlace = numPuzzleBlocks;
                    for (int i = 0; i < numPuzzleBlocksToPlace; i++)
                    {
                        PuzzleBuilder.AddRandomPuzzleBlockToTop(puzzle, stack, 7, 9);
                    }
                    numPuzzleBlocks -= numPuzzleBlocksToPlace;
                }

                if (puzzle.GetNumBlocksPlaceableOnStack(stack) == 0) usableStacks.Remove(stack);
            }
        }

        private static int AddRandomJigsawObjectBlocksToTop(GeneratedPuzzleInfo<JungleFlavoredPuzzleArea, JungleFlavoredPuzzle> generatedPuzzleInfo, Stack stack, int limitSize = 5)
        {
            JungleFlavoredPuzzleArea puzzleArea = generatedPuzzleInfo.PuzzleArea;
            JungleFlavoredPuzzle puzzle = generatedPuzzleInfo.Puzzle;

            int maxJigsawSize = generatedPuzzleInfo.MaxBlocks - stack.Blocks.Count;
            if (maxJigsawSize > limitSize) maxJigsawSize = limitSize;
            List<JigsawPuzzleObject> usableJigsawPuzzleObjects = puzzleArea.JigsawPuzzleObjects.List.Where(jo => jo.JigsawSprites.Count <= maxJigsawSize).ToList();
            int jigsawsCount = usableJigsawPuzzleObjects.Count;

            if (usableJigsawPuzzleObjects.Count == 0) return 0;

            JigsawPuzzleObject jigsawPuzzleObject = usableJigsawPuzzleObjects[Random.Range(0, jigsawsCount)];

            for (int i = 0; i < jigsawPuzzleObject.JigsawSprites.Count; i++)
            {
                puzzle.AddNewJigsawBlock(stack, jigsawPuzzleObject, i);
            }

            return jigsawPuzzleObject.JigsawSprites.Count;
        }
    }
}
