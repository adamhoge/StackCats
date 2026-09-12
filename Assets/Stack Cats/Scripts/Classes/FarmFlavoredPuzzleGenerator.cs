using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Tofuwu.StackCats.Procedural
{
    public class FarmFlavoredPuzzleGenerator : PuzzleGenerator<PuzzleArea, FarmFlavoredPuzzle>
    {
        public FarmFlavoredPuzzleGenerator(PuzzleArea puzzleArea) : base(puzzleArea) { }

        public override Stack GetPreferredDestinationStack(FarmFlavoredPuzzle puzzle, List<Stack> usableStacks)
        {
            int minMovableBlocks = usableStacks.Min(s => puzzle.GetNumMovableBlocksInStack(s));
            return usableStacks.First(s => puzzle.GetNumMovableBlocksInStack(s) == minMovableBlocks);
        }

        public override Stack GetPreferredSourceStack(FarmFlavoredPuzzle puzzle, List<Stack> usableStacks)
        {
            throw new System.NotImplementedException();
        }

        protected override void CreateFinishedPuzzle(GeneratedPuzzleInfo<PuzzleArea, FarmFlavoredPuzzle> generatedPuzzleInfo)
        {
            FarmFlavoredPuzzle puzzle = generatedPuzzleInfo.Puzzle;

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

            // Add puzzle blocks. Ensure that one has a maximum of three blocks.
            Stack lowStack = puzzle.Stacks[Random.Range(0, puzzle.Stacks.Count)];
            int lowStackSize = generatedPuzzleInfo.ObjectiveDifficulty >= 50 ? 1 : Random.Range(0, 3);
            int avgStackSize = generatedPuzzleInfo.ObjectiveDifficulty / 20 + 2;
            if (avgStackSize > generatedPuzzleInfo.MaxBlocks / 2) avgStackSize = generatedPuzzleInfo.MaxBlocks / 2;
            foreach (Stack stack in puzzle.Stacks)
            {
                int stackSize = stack == lowStack ? lowStackSize : avgStackSize + Random.Range(-1, 2);

                for (int i = 0; i < stackSize; i++)
                {
                    PuzzleBuilder.AddRandomPuzzleBlockToTop(puzzle, stack, 4, 9);
                }
            }
        }

        protected override Dictionary<IMoveStrategy<PuzzleArea, FarmFlavoredPuzzle>, float> GetMoveStrategies(int difficulty)
        {
            Dictionary<IMoveStrategy<PuzzleArea, FarmFlavoredPuzzle>, float> moveStrategies = new Dictionary<IMoveStrategy<PuzzleArea, FarmFlavoredPuzzle>, float>();
            
            MoveBlocksStrategy<PuzzleArea, FarmFlavoredPuzzle> moveBlocksStrategy = new MoveBlocksStrategy<PuzzleArea, FarmFlavoredPuzzle>();
            moveStrategies.Add(moveBlocksStrategy, 0.0f);

            int maxSumBlocks = Mathf.CeilToInt((difficulty - 25.0f) / (MAX_DIFFICULTY - 25.0f) * 4);
            AddSumBlockStrategy<PuzzleArea, FarmFlavoredPuzzle> addSumBlockStrategy = new AddSumBlockStrategy<PuzzleArea, FarmFlavoredPuzzle>(maxSumBlocks);
            AddRequiredSumBlockStrategy<PuzzleArea, FarmFlavoredPuzzle> addRequiredSumBlockStrategy = new AddRequiredSumBlockStrategy<PuzzleArea, FarmFlavoredPuzzle>(maxSumBlocks);
            moveStrategies.Add(addSumBlockStrategy, 0.0f);
            moveStrategies.Add(addRequiredSumBlockStrategy, 0.0f);

            SplitSumBlocksStrategy<PuzzleArea, FarmFlavoredPuzzle> splitSumBlockStrategy = new SplitSumBlocksStrategy<PuzzleArea, FarmFlavoredPuzzle>();
            moveStrategies.Add(splitSumBlockStrategy, 0.0f);

            int maxWildBlocks = Mathf.CeilToInt((difficulty - 50.0f) / (MAX_DIFFICULTY - 50.0f) * 4);
            AddWildBlockStrategy<PuzzleArea, FarmFlavoredPuzzle> addWildBlockStategy = new AddWildBlockStrategy<PuzzleArea, FarmFlavoredPuzzle>(maxWildBlocks);
            moveStrategies.Add(addWildBlockStategy, 0.0f);

            float strategyPercentageRemaining = 1.0f;

            if (difficulty > 80)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[addRequiredSumBlockStrategy] += usedPercent / 2;
                moveStrategies[splitSumBlockStrategy] += usedPercent / 2;
            }

            if (difficulty > 70)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[splitSumBlockStrategy] += usedPercent / 2;
                moveStrategies[addWildBlockStategy] += usedPercent / 2;
            }

            if (difficulty > 60)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[splitSumBlockStrategy] += usedPercent / 2;
                moveStrategies[addWildBlockStategy] += usedPercent / 2;
            }

            if (difficulty > 50)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[splitSumBlockStrategy] += usedPercent / 2;
                moveStrategies[addWildBlockStategy] += usedPercent / 2;
            }

            if (difficulty > 40)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[addRequiredSumBlockStrategy] += usedPercent;
            }

            if (difficulty > 30)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                if (difficulty > 60)
                {
                    moveStrategies[addRequiredSumBlockStrategy] += usedPercent / 2;
                }
                else
                {
                    moveStrategies[addSumBlockStrategy] += usedPercent / 2;
                }
                moveStrategies[addWildBlockStategy] += usedPercent / 2;
            }

            if (difficulty > 20)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                if (difficulty > 60)
                {
                    moveStrategies[addRequiredSumBlockStrategy] += usedPercent;
                }
                else
                {
                    moveStrategies[addSumBlockStrategy] += usedPercent;
                }
            }
            moveStrategies[moveBlocksStrategy] += strategyPercentageRemaining;

            return moveStrategies;
        }
    }
}
