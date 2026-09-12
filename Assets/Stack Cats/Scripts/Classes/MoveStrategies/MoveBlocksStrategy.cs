using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tofuwu.StackCats.Procedural
{
    public class MoveBlocksStrategy<TPuzzleArea, TPuzzle> : IMoveStrategy<TPuzzleArea, TPuzzle> where TPuzzleArea : PuzzleArea where TPuzzle : Puzzle
    {
        public MoveStrategyResult PerformMoveStrategy(PuzzleGenerator<TPuzzleArea, TPuzzle> puzzleGenerator, GeneratedPuzzleInfo<TPuzzleArea, TPuzzle> generatedPuzzleInfo)
        {
            TPuzzle puzzle = generatedPuzzleInfo.Puzzle;

            int blockToMoveIndex = GetBlockToMoveIndex(generatedPuzzleInfo);

            // If no blocks are movable, there's no action to be performed.
            if (blockToMoveIndex == -1) return new MoveStrategyResult { succeeded = false };

            List<Stack> prospectiveDestinationStacks = puzzle.Stacks.Where(s => puzzle.GetNumBlocksPlaceableOnStack(s) > 1).ToList();
            while (prospectiveDestinationStacks.Count > 0)
            {
                Stack destinationStack = puzzleGenerator.GetPreferredDestinationStack(puzzle, prospectiveDestinationStacks);

                int availableStackSpace = puzzle.MaxMovableStackHeight - destinationStack.Blocks.Count;
                // TODO: Factor in adding cat block
                --availableStackSpace;

                //Pick source
                List<Stack> prospectiveSourceStacks = new List<Stack>();
                prospectiveSourceStacks = puzzle.GetStacksByMovableCountRange(1, 10);
                prospectiveSourceStacks.Remove(destinationStack);
                while (prospectiveSourceStacks.Count > 0)
                {
                    // Move to random movable stack (enough free space)
                    Stack sourceStack = prospectiveSourceStacks[Random.Range(0, prospectiveSourceStacks.Count)];
                    int numMovableBlocks = puzzle.GetNumMovableBlocksInStack(sourceStack);
                    if (numMovableBlocks > 0)
                    {
                        int maxBlocksToMove = availableStackSpace < numMovableBlocks ? availableStackSpace : numMovableBlocks;
                        //int minBlocksToMove = maxBlocksToMove > 1 ? GetPaddedNum
                        int numBlocksToMove = maxBlocksToMove; //Random.Range(1, maxBlocksToMove);
                        Block blockToMove = sourceStack.Blocks[sourceStack.Blocks.Count - numBlocksToMove];
                        if (PuzzleBuilder.AddMoveCatBlock(puzzle, sourceStack, blockToMove, destinationStack))
                        {
                            return new MoveStrategyResult { succeeded = true, estimatedDifficulty = 0.5f, numMovesMade = 1 };
                        }
                    }

                    prospectiveSourceStacks.Remove(sourceStack);
                }

                //Lowest Count
                prospectiveDestinationStacks.Remove(destinationStack);
            }

            return new MoveStrategyResult { succeeded = false };
        }

        private int GetBlockToMoveIndex(GeneratedPuzzleInfo<TPuzzleArea, TPuzzle> generatedPuzzleInfo)
        {
            return generatedPuzzleInfo.Puzzle.GetLowestMovableIndex();
        }
    }
}
