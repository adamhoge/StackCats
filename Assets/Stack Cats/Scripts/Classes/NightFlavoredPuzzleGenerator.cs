using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tofuwu.StackCats.Procedural
{
    public class NightFlavoredPuzzleGenerator : PuzzleGenerator<PuzzleArea, NightFlavoredPuzzle>
    {
        public NightFlavoredPuzzleGenerator(PuzzleArea puzzleArea) : base(puzzleArea) { }

        public override GeneratedPuzzleInfo<PuzzleArea, NightFlavoredPuzzle> Generate(int difficulty)
        {
            GeneratedPuzzleInfo<PuzzleArea, NightFlavoredPuzzle> generatedPuzzleInfo = base.Generate(difficulty);

            generatedPuzzleInfo.Puzzle.Curtain.Raise(false);

            return generatedPuzzleInfo;
        }

        public override Stack GetPreferredDestinationStack(NightFlavoredPuzzle puzzle, List<Stack> usableStacks)
        {
            int minMovableBlocks = usableStacks.Min(s => puzzle.GetNumMovableBlocksInStack(s));
            return usableStacks.First(s => puzzle.GetNumMovableBlocksInStack(s) == minMovableBlocks);
        }

        public override Stack GetPreferredSourceStack(NightFlavoredPuzzle puzzle, List<Stack> usableStacks)
        {
            throw new System.NotImplementedException();
        }

        protected override MoveStrategyResult PerformMoveStrategy(
            IMoveStrategy<PuzzleArea, NightFlavoredPuzzle> moveStrategy,
            GeneratedPuzzleInfo<PuzzleArea, NightFlavoredPuzzle> generatedPuzzleInfo)
        {
            NightFlavoredPuzzle puzzle = generatedPuzzleInfo.Puzzle;
            if (puzzle.Curtain.Height == 0 && puzzle.CurtainTurnsRemaining == puzzle.CurtainDropInterval)
            {
                return new MoveStrategyResult() { shouldRemoveStrategy = true, succeeded = false };
            }

            MoveStrategyResult moveStrategyResult = moveStrategy.PerformMoveStrategy(this, generatedPuzzleInfo);

            if (moveStrategyResult.succeeded)
            {
                int numMovesMade = generatedPuzzleInfo.NumMovesMade;
                for (int i = 0; i < moveStrategyResult.numMovesMade; i++)
                {
                    if ((numMovesMade + i) % puzzle.CurtainDropInterval == 0)
                    {
                        puzzle.Curtain.Raise(false);
                    }
                }

                puzzle.CurtainTurnsRemaining = puzzle.CurtainDropInterval - generatedPuzzleInfo.NumMovesMade % puzzle.CurtainDropInterval;
            }

            return moveStrategyResult;
        }

        protected override void CreateFinishedPuzzle(GeneratedPuzzleInfo<PuzzleArea, NightFlavoredPuzzle> generatedPuzzleInfo)
        {
            NightFlavoredPuzzle puzzle = generatedPuzzleInfo.Puzzle;

            if (generatedPuzzleInfo.ObjectiveDifficulty >= 25)
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
            }

            puzzle.CurtainDropInterval = Random.Range(3, 5);
            puzzle.Curtain.Drop(puzzle.MaxStackHeight - avgStackSize - 1, false);
        }

        protected override Dictionary<IMoveStrategy<PuzzleArea, NightFlavoredPuzzle>, float> GetMoveStrategies(int difficulty)
        {
            Dictionary<IMoveStrategy<PuzzleArea, NightFlavoredPuzzle>, float> moveStrategies = new Dictionary<IMoveStrategy<PuzzleArea, NightFlavoredPuzzle>, float>();

            MoveBlocksStrategy<PuzzleArea, NightFlavoredPuzzle> moveBlocksStrategy = new MoveBlocksStrategy<PuzzleArea, NightFlavoredPuzzle>();
            moveStrategies.Add(moveBlocksStrategy, 0.0f);

            int maxSumBlocks = Mathf.CeilToInt((difficulty - 25.0f) / (MAX_DIFFICULTY - 25.0f) * 4);
            AddSumBlockStrategy<PuzzleArea, NightFlavoredPuzzle> addSumBlockStrategy = new AddSumBlockStrategy<PuzzleArea, NightFlavoredPuzzle>(maxSumBlocks);
            AddRequiredSumBlockStrategy<PuzzleArea, NightFlavoredPuzzle> addRequiredSumBlockStrategy = new AddRequiredSumBlockStrategy<PuzzleArea, NightFlavoredPuzzle>(maxSumBlocks);
            moveStrategies.Add(addSumBlockStrategy, 0.0f);
            moveStrategies.Add(addRequiredSumBlockStrategy, 0.0f);

            int maxWildBlocks = Mathf.CeilToInt((difficulty - 50.0f) / (MAX_DIFFICULTY - 50.0f) * 4);
            AddWildBlockStrategy<PuzzleArea, NightFlavoredPuzzle> addWildBlockStategy = new AddWildBlockStrategy<PuzzleArea, NightFlavoredPuzzle>(maxWildBlocks);
            moveStrategies.Add(addWildBlockStategy, 0.0f);

            float strategyPercentageRemaining = 1.0f;

            if (difficulty > 80)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                //moveStrategies[addRequiredSumBlockStrategy] += usedPercent / 2; // There is an issue with this that creates stray cat blocks on top of stacks
                moveStrategies[addSumBlockStrategy] += usedPercent / 2;
                moveStrategies[addWildBlockStategy] += usedPercent / 2;
            }

            if (difficulty > 70)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[moveBlocksStrategy] += usedPercent / 4;
               // moveStrategies[addRequiredSumBlockStrategy] += usedPercent / 2; // There is an issue with this that creates stray cat blocks on top of stacks
                moveStrategies[addSumBlockStrategy] += usedPercent / 2;
                moveStrategies[addWildBlockStategy] += usedPercent / 4;
            }

            if (difficulty > 50)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[moveBlocksStrategy] += usedPercent / 2;
                moveStrategies[addSumBlockStrategy] += usedPercent / 4;
                // moveStrategies[addRequiredSumBlockStrategy] += usedPercent / 4; // There is an issue with this that creates stray cat blocks on top of stacks
                moveStrategies[addWildBlockStategy] += usedPercent / 4;
            }

            if (difficulty > 30)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[moveBlocksStrategy] += usedPercent / 2;
                moveStrategies[addSumBlockStrategy] += usedPercent / 2;
                //   moveStrategies[addRequiredSumBlockStrategy] += usedPercent / 2; // There is an issue with this that creates stray cat blocks on top of stacks
            }

            if (difficulty > 10)
            {
                float usedPercent = MathHelpers.TakePercent(ref strategyPercentageRemaining, 0.25f);
                moveStrategies[moveBlocksStrategy] += usedPercent / 2;
                moveStrategies[addSumBlockStrategy] += usedPercent / 2;
            }

            moveStrategies[moveBlocksStrategy] += strategyPercentageRemaining;

            return moveStrategies;
        }
    }
}
