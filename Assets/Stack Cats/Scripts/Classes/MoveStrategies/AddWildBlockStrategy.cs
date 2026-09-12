using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tofuwu.StackCats.Procedural
{
    public class AddWildBlockStrategy<TPuzzleArea, TPuzzle> : IMoveStrategy<TPuzzleArea, TPuzzle> where TPuzzleArea : PuzzleArea where TPuzzle : Puzzle
    {
        private readonly int _maxWildBlocks;

        public AddWildBlockStrategy(int maxWildBlocks)
        {
            _maxWildBlocks = maxWildBlocks;
        }

        public MoveStrategyResult PerformMoveStrategy(PuzzleGenerator<TPuzzleArea, TPuzzle> puzzleGenerator, GeneratedPuzzleInfo<TPuzzleArea, TPuzzle> generatedPuzzleInfo)
        {
            TPuzzle puzzle = generatedPuzzleInfo.Puzzle;

            int lowestMovableBlockIndex = GetLowestMovableBlockIndex(puzzle);

            // If no blocks are movable, there's no action to be performed.
            if (lowestMovableBlockIndex == -1) return new MoveStrategyResult { succeeded = false };

            List<Stack> prospectiveDestinationStacks = puzzle.Stacks.Where(s => puzzle.MaxStackHeight - s.Blocks.Count > 1).ToList();
            while (prospectiveDestinationStacks.Count > 0)
            {
                Stack destinationStack = puzzleGenerator.GetPreferredDestinationStack(puzzle, prospectiveDestinationStacks);
                int availableStackSpace = puzzle.MaxStackHeight - destinationStack.Blocks.Count;
                // TODO: Factor in adding cat block
                --availableStackSpace;

                //Pick source
                List<Stack> prospectiveSourceStacks = new List<Stack>();
                prospectiveSourceStacks = puzzle.GetStacksByMovableCountRange();
                prospectiveSourceStacks.Remove(destinationStack);
                while (prospectiveSourceStacks.Count > 0)
                {
                    // Move to random movable stack (enough free space)
                    Stack sourceStack = prospectiveSourceStacks[Random.Range(0, prospectiveSourceStacks.Count)];
                    
                    if(!sourceStack.TopBlock.GetComponent<PuzzleBlock>())
                    {
                        prospectiveSourceStacks.Remove(sourceStack);
                        continue;
                    }

                    int numMovableBlocks = puzzle.GetNumMovableBlocksInStack(sourceStack);
                    if (numMovableBlocks > 0)
                    {
                        Block blockToMove = sourceStack.TopBlock;
                        if (PuzzleBuilder.AddMoveCatBlock(puzzle, sourceStack, blockToMove, destinationStack))
                        {
                            destinationStack.RemoveBlock(destinationStack.TopBlock, true);
                            puzzle.AddNewWildBlock(destinationStack);

                            return new MoveStrategyResult
                            {
                                succeeded = true,
                                shouldRemoveStrategy = puzzle.GetBlockComponentCount<WildBlock>() >= _maxWildBlocks,
                                estimatedDifficulty = 0.5f,
                                numMovesMade = 1
                            };
                        }
                    }

                    prospectiveSourceStacks.Remove(sourceStack);
                }

                //Lowest Count
                prospectiveDestinationStacks.Remove(destinationStack);
            }

            return new MoveStrategyResult { succeeded = false };
        }

        private int GetLowestMovableBlockIndex(Puzzle puzzle)
        {
            int lowestMovableIndex = -1;
            foreach (Stack stack in puzzle.Stacks)
            {
                Block lowestBlock = stack.Blocks.FirstOrDefault(block =>
                  puzzle.IsMovable(stack, block) &&
                  block.GetComponent<PuzzleBlock>() &&
                  block.HasBlockBelow<PuzzleBlock>());

                int lowestBlockIndex = stack.Blocks.IndexOf(lowestBlock);
                if (lowestBlock && (lowestMovableIndex == -1 || lowestBlockIndex < lowestMovableIndex))
                {
                    lowestMovableIndex = lowestBlockIndex;
                }
            }

            return lowestMovableIndex;
        }
    }
}
