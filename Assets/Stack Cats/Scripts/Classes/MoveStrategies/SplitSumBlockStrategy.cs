using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

namespace Tofuwu.StackCats.Procedural
{
    public class SplitSumBlocksStrategy<TPuzzleArea, TPuzzle> : IMoveStrategy<TPuzzleArea, TPuzzle> where TPuzzleArea : PuzzleArea where TPuzzle : Puzzle
    {
        public MoveStrategyResult PerformMoveStrategy(PuzzleGenerator<TPuzzleArea, TPuzzle> puzzleGenerator, GeneratedPuzzleInfo<TPuzzleArea, TPuzzle> generatedPuzzleInfo)
        {
            TPuzzle puzzle = generatedPuzzleInfo.Puzzle;

            if (!CanPerformMoveStrategy(puzzle))
            {
                return new MoveStrategyResult { succeeded = false };
            }

            int lowestMovableBlockIndex = puzzle.GetLowestMovableIndex();

            // If no blocks are movable, there's no action to be performed.
            if (lowestMovableBlockIndex == -1) return new MoveStrategyResult { succeeded = false };

            List<Stack> prospectiveDestinationStacks = puzzle.Stacks.Where(s => puzzle.MaxMovableStackHeight - s.Blocks.Count > 2).ToList();
            while (prospectiveDestinationStacks.Count > 0)
            {
                Stack destinationStack = puzzleGenerator.GetPreferredDestinationStack(puzzle, prospectiveDestinationStacks);

                // Factor in adding cat block and sum block
                int availableStackSpace = puzzle.GetNumBlocksPlaceableOnStack(destinationStack) - 2;

                //Pick source
                List<Stack> prospectiveSourceStacks = new List<Stack>();
                int numStacksWithPlaceableBlocks = puzzle.Stacks.Count(s => puzzle.GetNumBlocksPlaceableOnStack(s) > 1);
                if (numStacksWithPlaceableBlocks == 2)
                {
                    prospectiveSourceStacks = puzzle.Stacks.Where(
                        s => s.IsAtMaxCapacity &&
                        s.Blocks.Count >= 2 &&
                        s.TopBlock.GetComponent<PuzzleBlock>() &&
                        s.TopBlock.HasBlockBelow<PuzzleBlock>()).ToList();
                }
                else
                {
                    prospectiveSourceStacks = puzzle.GetStacksByMovableCountRange(1, 10);
                }
                prospectiveSourceStacks.Remove(destinationStack);
                while (prospectiveSourceStacks.Count > 0)
                {
                    // Move to random movable stack (enough free space)
                    Stack sourceStack = prospectiveSourceStacks[UnityEngine.Random.Range(0, prospectiveSourceStacks.Count)];
                    int numMovableBlocks = puzzle.GetNumMovableBlocksInStack(sourceStack);
                    if (numMovableBlocks > 1)
                    {
                        int maxBlocksToMove = availableStackSpace < numMovableBlocks ? availableStackSpace : numMovableBlocks;
                        //int minBlocksToMove = maxBlocksToMove > 1 ? GetPaddedNum
                        int numBlocksToMove = UnityEngine.Random.Range(1, maxBlocksToMove);
                        Block blockToMove = sourceStack.Blocks[sourceStack.Blocks.Count - numBlocksToMove];
                        if (PuzzleBuilder.AddMoveCatBlock(puzzle, sourceStack, blockToMove, destinationStack))
                        {
                            bool canUseDifference = numMovableBlocks % 2 == 0;
                            Tuple<int, int> sumValues = GetRandomSumValueForStacks(sourceStack, destinationStack, numMovableBlocks);

                            // AddMoveSumBlock on source stack
                            List<Stack> prospectiveSourceDestinationStacks = puzzle.Stacks.Where(s =>
                                s != sourceStack &&
                                s != destinationStack &&
                                (!s.TopBlock || !s.TopBlock.GetComponent<SumBlock>()) &&
                                puzzle.GetNumBlocksPlaceableOnStack(s) > 0).ToList();
                            Stack sourceDestinationStack = prospectiveSourceDestinationStacks.SelectRandom();
                            if (puzzle.GetNumBlocksPlaceableOnStack(sourceDestinationStack) > 1 && !sourceDestinationStack.HasBlockComponent<CatBlock>())
                            {
                                puzzle.AddNewCatBlock(sourceDestinationStack);
                            }
                            puzzle.AddNewSumBlock(sourceDestinationStack, sumValues.Item1);
                            for (int i = sourceStack.Blocks.Count - 1; i >= 0 && sourceStack.Blocks[i].GetComponent<PuzzleBlock>(); i--)
                            {
                                sourceStack.Blocks[i].GetComponent<PuzzleBlock>().PrimaryNumber -= sumValues.Item1;
                            }

                            // AddMoveSumBlock on destination stack
                            List<Stack> prospectiveDestinationDestinationStacks = puzzle.Stacks.Where(s =>
                                s != destinationStack &&
                                (!s.TopBlock || !s.TopBlock.GetComponent<SumBlock>()) &&
                                puzzle.GetNumBlocksPlaceableOnStack(s) > 0).ToList();
                            Stack destinationDestinationStack = PuzzleGeneratorHelpers.SelectStackWithLeastishMovableBlocks(puzzle, prospectiveDestinationDestinationStacks);
                            // TODO: This threw a null object error, look into linq selection of stacks, maybe prereqs to stack selection as well
                            try
                            {
                                if (puzzle.GetNumBlocksPlaceableOnStack(destinationDestinationStack) > 1 && !destinationDestinationStack.HasBlockComponent<CatBlock>())
                                {
                                    puzzle.AddNewCatBlock(destinationDestinationStack);
                                }
                            }
                            catch
                            {
                                Debug.Log("prospectiveDestinationDestinationStacks count:" + prospectiveDestinationDestinationStacks.Count);
                                Debug.Log("source stack:" + puzzle.Stacks.IndexOf(sourceStack));
                                Debug.Log("destination stack:" + puzzle.Stacks.IndexOf(destinationStack));
                                Debug.Log("sdestination stack:" + puzzle.Stacks.IndexOf(sourceDestinationStack));
                                Debug.Log("ddestination stack:" + puzzle.Stacks.IndexOf(destinationDestinationStack));
                                throw;
                            }
                            puzzle.AddNewSumBlock(destinationDestinationStack, sumValues.Item2);
                            for (int i = destinationStack.Blocks.Count - 1; i >= 0 && destinationStack.Blocks[i].GetComponent<PuzzleBlock>(); i--)
                            {
                                destinationStack.Blocks[i].GetComponent<PuzzleBlock>().PrimaryNumber -= sumValues.Item2;
                            }

                            return new MoveStrategyResult { succeeded = true, numMovesMade = 3 };
                        }
                    }

                    prospectiveSourceStacks.Remove(sourceStack);
                }

                //Lowest Count
                prospectiveDestinationStacks.Remove(destinationStack);
            }

            return new MoveStrategyResult { succeeded = false };
        }

        private bool CanPerformMoveStrategy(TPuzzle puzzle)
        {
            IEnumerable<Stack> stacksWithPuzzleBlocks = puzzle.Stacks.Where(s =>
                s.Blocks.Count >= 2 &&
                s.TopBlock.GetComponent<PuzzleBlock>() &&
                s.TopBlock.HasBlockBelow<PuzzleBlock>()).ToList();

            return stacksWithPuzzleBlocks.Any(pbs => puzzle.Stacks.Count(s1 =>
                s1 != pbs &&
                (!s1.TopBlock || !s1.TopBlock.GetComponent<SumBlock>()) &&
                puzzle.GetNumBlocksPlaceableOnStack(s1) > 0 &&
                s1.IsImmovableInTopNumBlocks(3)) >= 2);
        }

        protected PuzzleBlock GetLowestMovablePuzzleBlock(PuzzleBlock topPuzzleBlock)
        {
            if (!topPuzzleBlock) return null;

            PuzzleBlock puzzleBlock = topPuzzleBlock;
            Block blockBelow = puzzleBlock.Block.GetBlockBelow();
            PuzzleBlock puzzleBlockBelow = blockBelow ? blockBelow.GetComponent<PuzzleBlock>() : null;
            while (puzzleBlockBelow)
            {
                puzzleBlock = puzzleBlockBelow;

                blockBelow = puzzleBlock.Block.GetBlockBelow();
                puzzleBlockBelow = blockBelow ? blockBelow.GetComponent<PuzzleBlock>() : null;
            }

            return puzzleBlock;
        }

        private Tuple<int, int> GetRandomSumValueForStacks(Stack sourceStack, Stack destinationStack, int preSplitBlockCount)
        {
            PuzzleBlock sourceTopPuzzleBlock = sourceStack.TopBlock.GetComponent<PuzzleBlock>();
            int sourceMinValue = 1 - GetLowestMovablePuzzleBlock(sourceTopPuzzleBlock).PrimaryNumber;
            int sourceMaxValue = 9 - sourceTopPuzzleBlock.PrimaryNumber;

            PuzzleBlock destinationTopPuzzleBlock = destinationStack.TopBlock.GetComponent<PuzzleBlock>();
            int destinationMinValue = 1 - GetLowestMovablePuzzleBlock(destinationTopPuzzleBlock).PrimaryNumber;
            int destinationMaxValue = 9 - destinationTopPuzzleBlock.PrimaryNumber;

            bool canUseDiffOfBlocks = preSplitBlockCount % 2 == 0;
            if (canUseDiffOfBlocks)
            {
                int diffDestinationMinValue = sourceMinValue + preSplitBlockCount;
                if (diffDestinationMinValue == 0) diffDestinationMinValue = 1;
                int diffSourceMaxValue = destinationMaxValue - preSplitBlockCount;
                if (diffSourceMaxValue == 0) diffSourceMaxValue = -1;

                int usedMinValue = sourceMinValue < diffDestinationMinValue ? diffDestinationMinValue : sourceMinValue;
                int usedMaxValue = sourceMaxValue > diffSourceMaxValue ? diffSourceMaxValue : sourceMaxValue;

                if (destinationMaxValue >= diffDestinationMinValue && usedMinValue <= diffSourceMaxValue)
                {
                    int sourceStackDiffSumBlockValue = SelectRandomNumberFromExclusiveRange(usedMinValue, usedMaxValue, new List<int> { 0, -preSplitBlockCount });
                    int destinationStackDiffSumBlockValue = sourceStackDiffSumBlockValue + preSplitBlockCount;
                    return new Tuple<int, int>(-sourceStackDiffSumBlockValue, -destinationStackDiffSumBlockValue);
                }
            }

            int maxMinValue = sourceMinValue > destinationMinValue ? sourceMinValue : destinationMinValue;
            if (maxMinValue == 0) maxMinValue = 1;
            int minMaxValue = sourceMaxValue < destinationMaxValue ? sourceMaxValue : destinationMaxValue;
            if (minMaxValue == 0) minMaxValue = -1;
            int sourceStackSumBlockValue = UnityEngine.Random.value > 0.1f ?
                SelectLargestAbsValueNumberFromRange(maxMinValue, maxMinValue) :
                SelectRandomNonZeroNumberFromRange(maxMinValue, minMaxValue);
            int destinationStackSumBlockValue = sourceStackSumBlockValue;

            return new Tuple<int, int>(-sourceStackSumBlockValue, -destinationStackSumBlockValue);
        }

        private int SelectRandomNumberFromExclusiveRange(int minValue, int maxValue, List<int> excludedNumbers)
        {
            int selectedValue;
            do
            {
                selectedValue = UnityEngine.Random.Range(minValue, maxValue + 1);
            }
            while (excludedNumbers.Contains(selectedValue));

            return selectedValue;
        }

        private int SelectRandomNonZeroNumberFromRange(int minValue, int maxValue)
        {
            int selectedValue;
            do
            {
                selectedValue = UnityEngine.Random.Range(minValue, maxValue + 1);
            }
            while (selectedValue == 0);

            return selectedValue;
        }

        private int SelectLargestAbsValueNumberFromRange(int minValue, int maxValue)
        {
            return Math.Abs(minValue) > Math.Abs(maxValue) ? minValue : maxValue;
        }
    }
}
