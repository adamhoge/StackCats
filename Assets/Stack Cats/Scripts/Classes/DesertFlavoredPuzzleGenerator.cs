using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tofuwu.StackCats.Procedural
{
    public class DesertFlavoredPuzzleGenerator : PuzzleGenerator<PuzzleArea, DesertFlavoredPuzzle>
    {
        public DesertFlavoredPuzzleGenerator(PuzzleArea puzzleArea) : base(puzzleArea) { }

        public override Stack GetPreferredDestinationStack(DesertFlavoredPuzzle puzzle, List<Stack> usableStacks)
        {
            int minMovableBlocks = usableStacks.Min(s => puzzle.GetNumMovableBlocksInStack(s));
            return usableStacks.First(s => puzzle.GetNumMovableBlocksInStack(s) == minMovableBlocks);
        }

        public override Stack GetPreferredSourceStack(DesertFlavoredPuzzle puzzle, List<Stack> usableStacks)
        {
            throw new System.NotImplementedException();
        }

        protected override void CreateFinishedPuzzle(GeneratedPuzzleInfo<PuzzleArea, DesertFlavoredPuzzle> generatedPuzzleInfo)
        {
            DesertFlavoredPuzzle puzzle = generatedPuzzleInfo.Puzzle;

            if (generatedPuzzleInfo.ObjectiveDifficulty >= 50)
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

            Stack lowStack = puzzle.Stacks[Random.Range(0, puzzle.Stacks.Count)];
            int lowStackMinSize = generatedPuzzleInfo.ObjectiveDifficulty >= 30 ? 1 : 0;
            int lowStackSize = Random.Range(lowStackMinSize, 3);
            int avgStackSize = Mathf.CeilToInt(generatedPuzzleInfo.ObjectiveDifficulty / 30.0f + 1);
            if (avgStackSize > generatedPuzzleInfo.MaxBlocks / 2) avgStackSize = generatedPuzzleInfo.MaxBlocks / 2;
            foreach (Stack stack in puzzle.Stacks)
            {
                int stackSize = stack == lowStack ? lowStackSize : avgStackSize + Random.Range(-1, 2);

                for (int i = 0; i < stackSize; i++)
                {
                    PuzzleBuilder.AddRandomPuzzleBlockToTop(puzzle, stack, 5, 9);
                }

                puzzle.StackHeightRequirements[puzzle.Stacks.IndexOf(stack)] = stack.Blocks.Count;
            }

        }

        protected override Dictionary<IMoveStrategy<PuzzleArea, DesertFlavoredPuzzle>, float> GetMoveStrategies(int difficulty)
        {
            Dictionary<IMoveStrategy<PuzzleArea, DesertFlavoredPuzzle>, float> moveStrategies = new Dictionary<IMoveStrategy<PuzzleArea, DesertFlavoredPuzzle>, float>();

            MoveBlocksStrategy<PuzzleArea, DesertFlavoredPuzzle> moveBlocksStrategy = new MoveBlocksStrategy<PuzzleArea, DesertFlavoredPuzzle>();
            moveStrategies.Add(moveBlocksStrategy, 0.0f);

            float difficultyMultiplier = difficulty / MAX_DIFFICULTY;

            int maxSumBlocks = Mathf.CeilToInt(difficultyMultiplier * difficultyMultiplier * 6.0f);
            AddSumBlockStrategy<PuzzleArea, DesertFlavoredPuzzle> addSumBlockStrategy = new AddSumBlockStrategy<PuzzleArea, DesertFlavoredPuzzle>(maxSumBlocks);
            moveStrategies.Add(addSumBlockStrategy, 0.0f);

            int maxWildBlocks = Mathf.CeilToInt(difficultyMultiplier * difficultyMultiplier * 6.0f);
            AddWildBlockStrategy<PuzzleArea, DesertFlavoredPuzzle> addWildBlockStategy = new AddWildBlockStrategy<PuzzleArea, DesertFlavoredPuzzle>(maxWildBlocks);
            moveStrategies.Add(addWildBlockStategy, 0.0f);

            float strategyPercentageRemaining = 1.0f;

            if (difficulty > 80)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[addWildBlockStategy] += usedPercent;
            }

            if (difficulty > 70)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[moveBlocksStrategy] += usedPercent / 2;
                moveStrategies[addSumBlockStrategy] += usedPercent / 2;
            }

            if (difficulty > 60)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[moveBlocksStrategy] += usedPercent / 2;
                moveStrategies[addSumBlockStrategy] += usedPercent / 2;
            }

            if (difficulty > 50)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[addSumBlockStrategy] += usedPercent;
                moveStrategies[addWildBlockStategy] += usedPercent;
            }

            if (difficulty > 40)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[addSumBlockStrategy] += usedPercent;
            }

            if (difficulty > 30)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[addSumBlockStrategy] += usedPercent;
            }

            if (difficulty > 20)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[addSumBlockStrategy] += usedPercent;
            }

            moveStrategies[moveBlocksStrategy] += strategyPercentageRemaining;

            return moveStrategies;
        }
    }
}
